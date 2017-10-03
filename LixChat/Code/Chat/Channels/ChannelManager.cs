using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LX29_ChatClient
{
    public partial class ChatClient
    {
        public static bool LoggedIn = false;

        // public static DateTime now = DateTime.Now;
        // public static System.Text.StringBuilder sb = new System.Text.StringBuilder();
        private static Dictionary<string, ChannelInfo> channels = new Dictionary<string, ChannelInfo>();

        private static bool isSnycing = false;

        private static string ChannelSave
        {
            get { return Settings._dataDir + "Channels_" + SelfUserName + ".txt"; }
        }

        public static AddError AddChannel(string s)
        {
            try
            {
                s = GetOnlyName(s.ToLower());
                if (!channels.ContainsKey(s))
                {
                    ApiResult so = TwitchApi.GetUserID(s);
                    var sa = TwitchApi.GetStreamOrChannel(so.ID.ToString());
                    if (sa.Count > 0)
                    {
                        so = sa[0];
                        ChannelInfo si = new ChannelInfo(so);
                        channels.Add(s, si);

                        Emotes.LoadChannelEmotesAndBadges(si);

                        ListLoaded(channels.Count, channels.Count, "Loaded Channel: " + s);
                        SaveChannels();
                        return AddError.None;
                    }
                    return AddError.NotExist;
                }
                return AddError.Exists;
            }
            catch (Exception x)
            {
                return new AddError(AddErrorInfo.Error, x.Message);
            }
            //return AddError.Error;
        }

        public static System.Reflection.PropertyInfo[] GetMemberNames(object target, bool dynamicOnly = false)
        {
            return target.GetType().GetProperties();
        }

        public static string GetOnlyName(string input)
        {
            input = input.Trim();
            if (input.Any(t => char.IsWhiteSpace(t)))
            {
                return input.Substring(0, input.IndexOf(' '));
            }
            return input;
        }

        public static void INITIALIZE(System.Windows.Forms.Form Main)
        {
            try
            {
                int size = Enum.GetNames(typeof(SortMode)).Length;
                sortArr = new SortMode[size];
                for (int x = 0; x < size; x++)
                {
                    sortArr[x] = (SortMode)x;
                }
                TwitchUsers.Load(Main, new Action(SyncFollows));
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case System.Windows.Forms.MessageBoxResult.Retry:
                        INITIALIZE(Main);
                        return;

                    case System.Windows.Forms.MessageBoxResult.Abort:
                        System.Windows.Forms.Application.Exit();
                        return;
                }
            }
        }

        //public static void LoadChatLog()
        //{
        //    try
        //    {
        //        foreach (ChannelInfo ci in channels.Values)
        //        {
        //            string path = Settings.chatLogDir + ci.Name + ".log";
        //            if (File.Exists(path))
        //            {
        //                var sa = File.ReadAllLines(path);
        //                foreach (var s in sa)
        //                {
        //                    TryParseRawMessage(s);
        //                }
        //                messages.Add(ci.Name, "End of Log.");
        //            }
        //        }
        //    }
        //    catch (Exception x) { x.Handle("", false); }
        //    AutoActions.EnableActions = true;
        //}

        public static void RemoveChannel(string channel)
        {
            channel = GetOnlyName(channel.ToLower());
            if (channels.ContainsKey(channel))
            {
                channels.Remove(channel);
                if (ListLoaded != null)
                    ListLoaded(channels.Count, channels.Count, "Removed Channel");
            }
        }

        public static void ReSyncFollows(bool onlyOnline)
        {
            if (isSnycing) return;
            isSnycing = true;
            if (ListLoaded != null)
                ListLoaded(0, channels.Count, "Loading Channels");
            var follows = TwitchApi.GetFollowedStreams();
            List<ChannelInfo> list = new List<ChannelInfo>();
            foreach (var channel in follows)
            {
                if (onlyOnline && !channel.IsOnline) continue;
                ChannelInfo ci = new ChannelInfo(channel);
                if (!channels.ContainsKey(ci.Name))
                {
                    channels.Add(ci.Name, ci);
                    list.Add(ci);
                }
            }
            Task.Run(() => Emotes.FetchEmotes(list, true));

            SaveChannels();

            isSnycing = false;

            if (ListLoaded != null)
                ListLoaded(channels.Count, channels.Count, "Loaded Channels");
        }

        public static void SaveChannels()
        {
            try
            {
                lock (lockChannels)
                {
                    using (StreamWriter sw = new StreamWriter(ChannelSave, false))
                    {
                        foreach (var chan in channels.Values)
                        {
                            string obj = Newtonsoft.Json.JsonConvert.SerializeObject(chan);
                            sw.WriteLine(obj);
                        }
                    }
                }
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        public static void SyncFollows()
        {
            try
            {
                if (isSnycing) return;
                isSnycing = true;
                if (SelfUserToken.Length == 0)
                    return;

                if (ListLoaded != null)
                    ListLoaded(0, channels.Count, "Loading Channels");

                Task.Run(() => LoadChatHighlightWords());
                Task.Run(() => AutoActions.Load());
                Task.Run(() => LX29_ChatClient.Addons.Scripts.ScriptClassCollection.LoadScripts());

                var follows = TwitchApi.GetFollowedStreams();

                var setts = LoadChannels();

                var rest = new Dictionary<int, ChannelInfo>(setts);
                List<string> chatLogins = new List<string>();
                foreach (var channel in follows)
                {
                    ChannelInfo ci = new ChannelInfo(channel);
                    if (setts != null && setts.Count > 0)
                    {
                        if (!channels.ContainsKey(ci.Name))
                        {
                            if (setts.ContainsKey(ci.ID))
                            {
                                ci.Load(setts[ci.ID]);
                            }
                            channels.Add(ci.Name, ci);
                            rest.Remove(ci.ID);
                        }
                    }
                    else
                    {
                        if (!channels.ContainsKey(ci.Name))
                        {
                            channels.Add(ci.Name, ci);
                        }
                    }

                    Task.Run(() =>
                    {
                        if (ci.AutoLoginChat)
                        {
                            addChannel(ci.Name);
                            ChatClient.TryConnect(ci.Name);
                        }
                    });
                }

                if (rest.Count > 0)
                {
                    var restResults = TwitchApi.GetStreamOrChannel(rest.Keys.Select(t => t.ToString()).ToArray());
                    foreach (var result in restResults)
                    {
                        ChannelInfo ci = new ChannelInfo(result);
                        if (!channels.ContainsKey(ci.Name))
                        {
                            if (rest.ContainsKey(ci.ID))
                            {
                                var r = rest[ci.ID];
                                ci.Load(r);
                            }
                            channels.Add(ci.Name, ci);
                            Task.Run(() =>
                            {
                                if (ci.AutoLoginChat)
                                {
                                    addChannel(ci.Name);
                                    ChatClient.TryConnect(ci.Name);
                                }
                            });
                        }
                    }
                }

                LoadStandardStreams();
                SaveChannels();

                isSnycing = false;

                if (ListLoaded != null)
                    ListLoaded(channels.Count, channels.Count, "Loaded " + channels.Count + " Channels");

                Task.Run(() => Emotes.FetchEmotes(channels.Values.ToList(), false));

                startRefresher();
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case System.Windows.Forms.MessageBoxResult.Retry:
                        SyncFollows();
                        return;

                    case System.Windows.Forms.MessageBoxResult.Abort:
                        System.Windows.Forms.Application.Exit();
                        return;
                }
            }
        }

        public static void UpdateChannels()
        {
            try
            {
                var streams = TwitchApi.GetStreams(channels.Select(t => t.Value.ApiResult));

                foreach (var channel in channels.Values)
                {
                    var stream = streams.FirstOrDefault(t => t.ID.Equals(channel.ID));
                    if (stream != null)
                    {
                        channel.ApiResult = stream;
                        Task.Run(() =>
                        {
                            //Emotes.LoadChannelEmotes(channel);
                            FetchChatUsers(channel.Name);
                        });
                    }
                    else
                    {
                        channel.ResetStreamStatus();
                    }
                    channel.GetMpvWindow();
                }
                Emotes.Values.CheckLoadingTime();
                SaveChannels();
                ListLoaded(channels.Count, channels.Count, "Refreshed Channels");
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case System.Windows.Forms.MessageBoxResult.Retry:
                        UpdateChannels();
                        return;

                    case System.Windows.Forms.MessageBoxResult.Abort:
                        System.Windows.Forms.Application.Exit();
                        return;
                }
            }
        }

        private static Dictionary<int, ChannelInfo> LoadChannels()
        {
            try
            {
                if (File.Exists(ChannelSave))
                {
                    var input = File.ReadAllLines(ChannelSave);
                    Dictionary<int, ChannelInfo> dict = new Dictionary<int, ChannelInfo>();
                    foreach (var s in input)
                    {
                        var b = Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelInfo>(s);
                        dict.Add(b.ID, b);
                    }
                    return dict;
                    //var values = CustomSettings.LoadList(input);

                    //Dictionary<int, Dictionary<string, string>> dict = new Dictionary<int, Dictionary<string, string>>();
                    //foreach (var vals in values)
                    //{
                    //    int ID = (int)vals["ID"];
                    //    dict.Add(ID, new Dictionary<string, string>());
                    //    foreach (var item in vals)
                    //    {
                    //        dict[ID].Add(item.Key, item.Value);
                    //    }
                    //}

                    //if (dict.Count == 0) File.Delete(ChannelSave);
                    //return dict;
                }
            }
            catch { }
            return new Dictionary<int, ChannelInfo>();
        }

        private static void LoadStandardStreams()
        {
            if (!channels.ContainsKey("lukix29"))
            {
                ApiResult result = TwitchApi.GetStreamOrChannel("79328905")[0];
                ChannelInfo ci = new ChannelInfo(result, true, true);

                channels.Add(ci.Name, ci);
                Task.Run(() =>
                {
                    if (ci.AutoLoginChat)
                    {
                        addChannel(ci.Name);
                        ChatClient.TryConnect(ci.Name);
                    }
                });
            }
            else
            {
                channels["lukix29"] = new ChannelInfo(channels["lukix29"], true, true);
            }
        }

        //private static void logInChats()
        //{
        //    var list = channels.Where(
        //        t => t.Value.AutoLoginChat).Select(t => t.Value)
        //        .OrderBy(t => t.IsFixed).Select(t => t.Name).ToList();
        //    foreach (var s in list)
        //    {
        //        ChatClient.TryConnect(s);
        //    }
        //    //TryConnect("lx29_tcvc");
        //    ListUpdated();
        //}

        private static void startRefresher()
        {
            LXTimer o = new LXTimer(new Action<LXTimer>(updateChannels), (int)Settings.UpdateInterval, System.Threading.Timeout.Infinite);
        }

        private static void updateChannels(LXTimer obj)
        {
            LXTimer o = obj;
            UpdateChannels();
            if (LX29_Tools.HasInternetConnection)
            {
                o.Change((int)Settings.UpdateInterval, System.Threading.Timeout.Infinite);
            }
            else
            {
                o.Change(10000, System.Threading.Timeout.Infinite);
            }
        }

        //public struct temp
        //{
        //    public string name;
        //    public TimeSpan span;

        //    public temp(string Name, TimeSpan Span)
        //    {
        //        name = Name;
        //        span = Span;
        //    }
        //}
    }
}