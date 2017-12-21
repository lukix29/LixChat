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
        private static Dictionary<int, ChannelInfo> channels = new Dictionary<int, ChannelInfo>();

        private static bool isSnycing = false;

        private static string ChannelSave
        {
            get { return Settings._dataDir + "Channels_" + SelfUserName + ".json"; }
        }

        public static AddError AddChannel(string s)
        {
            try
            {
                s = GetOnlyName(s.ToLower());

                ApiResult so = TwitchApi.GetUserID(s);
                if (!channels.ContainsKey(so.ID))
                {
                    var sa = TwitchApi.GetStreamOrChannel(so.ID);
                    if (sa.Count > 0)
                    {
                        so = sa[0];
                        ChannelInfo si = new ChannelInfo(so);
                        channels.Add(si.ID, si);

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

        public static void AddChannels(Dictionary<int, ChannelInfo> rest, bool save = false)
        {
            var restResults = TwitchApi.GetStreamOrChannel(rest.Keys.ToArray());
            foreach (var result in restResults)
            {
                ChannelInfo ci = new ChannelInfo(result);
                if (!channels.ContainsKey(ci.ID))
                {
                    if (rest.ContainsKey(ci.ID))
                    {
                        var r = rest[ci.ID];
                        ci.Load(r);
                    }
                    channels.Add(ci.ID, ci);
                    Task.Run(() =>
                    {
                        if (ci.AutoLoginChat)
                        {
                            ChatClient.TryConnect(ci.ID);
                        }
                    });
                }
            }
            if (save)
            {
                ListLoaded?.Invoke(channels.Count, channels.Count, "Imported " + channels.Count + " Channels");
                SaveChannels();
            }
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
                messages = new MessageCollection();
                int size = Enum.GetNames(typeof(SortMode)).Length;
                sortArr = new SortMode[size];
                for (int x = 0; x < size; x++)
                {
                    sortArr[x] = (SortMode)x;
                }
                TwitchUserCollection.Load(Main, () => SyncFollows());
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

        public static Dictionary<int, ChannelInfo> LoadChannels(string saveFile)
        {
            try
            {
                if (!File.Exists(saveFile)) saveFile = saveFile.Replace(".json", ".txt");

                if (File.Exists(saveFile))
                {
                    var input = File.ReadAllLines(saveFile);
                    Dictionary<int, ChannelInfo> dict = new Dictionary<int, ChannelInfo>();
                    foreach (var s in input)
                    {
                        var b = Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelInfo>(s);
                        dict.Add(b.ID, b);
                    }
                    return dict;
                }
            }
            catch { }
            return new Dictionary<int, ChannelInfo>();
        }

        public static void RemoveChannel(int channel)
        {
            if (channels.ContainsKey(channel))
            {
                channels.Remove(channel);
                ListLoaded?.Invoke(channels.Count, channels.Count, "Removed Channel");
            }
        }

        public async static void SaveChannels()
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (lockChannels)
                    {
                        string s = "";
                        using (StreamWriter sw = new StreamWriter(ChannelSave, false))
                        {
                            foreach (var chan in channels.Values)
                            {
                                string obj = Newtonsoft.Json.JsonConvert.SerializeObject(chan);
                                sw.WriteLine(obj);
                                s += chan.Name + ",";
                            }
                        }
                    }
                }
                catch (Exception x)
                {
                    x.Handle();
                }
            });
        }

        public static void SyncFollows(bool addAll = false)
        {
            try
            {
                if (isSnycing) return;
                isSnycing = true;

                channels = new Dictionary<int, ChannelInfo>();

                ListLoaded?.Invoke(0, channels.Count, "Loading Channels");

                Task.Run(() => LoadChatHighlightWords());
                //Task.Run(() => AutoActions.Load());

                Task.Run(() =>
                {
                    try
                    {
                        AutoActions = LX29_ChatClient.Addons.AutoActions.Load();
                        QuickText = LX29_ChatClient.Addons.QuickTextCollection.Load();
                        LX29_ChatClient.Addons.Scripts.ScriptClassCollection.LoadScripts();
                    }
                    catch (Exception x)
                    {
                        x.Handle();
                    }
                });

                LoadSelfStream();

                var follows = TwitchApi.GetFollowedStreams();

                var setts = LoadChannels(ChannelSave);

                var rest = new Dictionary<int, ChannelInfo>(setts);
                List<string> chatLogins = new List<string>();
                foreach (var channel in follows)
                {
                    ChannelInfo ci = new ChannelInfo(channel);
                    if (setts != null && setts.Count > 0)
                    {
                        if (!channels.ContainsKey(ci.ID))
                        {
                            bool cont = false;
                            if (setts.ContainsKey(ci.ID))
                            {
                                ci.Load(setts[ci.ID]);
                                cont = true;
                            }
                            if (cont || addAll)
                            {
                                channels.Add(ci.ID, ci);
                                rest.Remove(ci.ID);
                            }
                        }
                    }
                    else
                    {
                        if (!channels.ContainsKey(ci.ID))
                        {
                            channels.Add(ci.ID, ci);
                        }
                    }

                    Task.Run(() =>
                    {
                        if (ci.AutoLoginChat)
                        {
                            ChatClient.TryConnect(ci.ID);
                        }
                    });
                }

                if (rest.Count > 0)
                {
                    AddChannels(rest);
                }

                SaveChannels();

                isSnycing = false;

                ListLoaded?.Invoke(channels.Count, channels.Count, "Loaded " + channels.Count + " Channels");

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
                var streams = TwitchApi.GetFollowedStreams();

                //(channels.Select(t => t.Value.ID.ToString()).ToArray());
                var rr = Channels.Values.Where(t => !streams.Any(t0 => t0.ID.Equals(t.ID))).Select(t => t.ID);

                var rest = TwitchApi.GetStreamOrChannel(rr.ToArray());

                streams.AddRange(rest);

                foreach (var stream in streams)
                {
                    var channel = channels[stream.ID];

                    channel.ApiResult = stream;

                    if (channel.IsOnline)
                    {
                        //Task.Run(() =>
                        // {
                        FetchChatUsers(channel.Name);
                        //});
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

        private static void LoadSelfStream()
        {
            int id = TwitchUserCollection.Selected.ID;
            if (!channels.ContainsKey(id))
            {
                ApiResult result = TwitchApi.GetStreamOrChannel(id)[0];
                ChannelInfo ci = new ChannelInfo(result, true, true);

                channels.Add(ci.ID, ci);
                Task.Run(() =>
                {
                    ChatClient.TryConnect(ci.ID);
                });
            }
            else
            {
                channels[id] = new ChannelInfo(channels[id], true, true);
            }
        }

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
    }
}