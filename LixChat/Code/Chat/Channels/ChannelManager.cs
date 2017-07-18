﻿using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LX29_ChatClient
{
    public partial class ChatClient
    {
        public static bool LoggedIn = false;

        //public static TwitchPubSub pubsub = new TwitchPubSub();
        private static Dictionary<string, ChannelInfo> channels = new Dictionary<string, ChannelInfo>();

        private static bool isSnycing = false;

        private static string ChannelSave
        {
            get { return Settings.dataDir + "Channels_" + SelfUserName + ".txt"; }
        }

        public static AddError AddChannel(string s)
        {
            try
            {
                s = GetOnlyName(s.ToLower());
                if (!channels.ContainsKey(s))
                {
                    ApiResult so = TwitchApi.GetUserID(s);
                    var sa = TwitchApi.GetStreamOrChannel(so.ID);
                    if (sa.Count > 0)
                    {
                        so = sa[0];
                        ChannelInfo si = new ChannelInfo(so);
                        channels.Add(s, si);
                        //if (si.IsOnline)
                        //{
                        //    TryConnect(s);
                        //}
                        Emotes.LoadChannelEmotes(si.Name);
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

        public static void LoadChatLog()
        {
            try
            {
                foreach (ChannelInfo ci in channels.Values)
                {
                    string path = Settings.chatLogDir + ci.Name + ".log";
                    if (File.Exists(path))
                    {
                        var sa = File.ReadAllLines(path);
                        foreach (var s in sa)
                        {
                            TryParseRawMessage(s);
                        }
                        messages.Add(ci.Name, "End of Log.");
                    }
                }
            }
            catch (Exception x) { x.Handle("", false); }
            AutoActions.EnableActions = true;
        }

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
            var follows = TwitchApi.GetFollowedStreams(SelfApiResult.ID, SelfUserToken);
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
            using (StreamWriter sw = new StreamWriter(ChannelSave, false))
            {
                var chans = channels.Values.OrderByDescending(t => t.IsOnline);
                foreach (var channel in chans)
                {
                    sw.WriteLine(channel.ToString());
                    sw.WriteLine();
                }
            }
        }

        public static void SyncFollows()
        {
            try
            {
                //DateTime now = DateTime.Now;
                // var spans = new List<temp>();

                if (isSnycing) return;
                isSnycing = true;
                if (SelfUserToken.Length == 0)
                    return;

                if (ListLoaded != null)
                    ListLoaded(0, channels.Count, "Loading Channels");

                var follows = TwitchApi.GetFollowedStreams(SelfApiResult.ID, SelfUserToken);

                //spans.Add(new temp("follows", DateTime.Now.Subtract(now)));
                // now = DateTime.Now;
                var setts = LoadChannels();
                // spans.Add(new temp("load Channels", DateTime.Now.Subtract(now)));
                //now = DateTime.Now;
                var rest = new Dictionary<string, List<string>>(setts);
                List<string> chatLogins = new List<string>();
                //now = DateTime.Now;
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
                }
                // spans.Add(new temp("parse channels", DateTime.Now.Subtract(now)));
                // now = DateTime.Now;
                if (rest.Count > 0)
                {
                    var restResults = TwitchApi.GetStreamOrChannel(rest.Keys.ToArray());
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
                        }
                    }
                }
                //spans.Add(new temp("parse rest", DateTime.Now.Subtract(now)));
                // now = DateTime.Now;
                LoadStandardStreams();
                // spans.Add(new temp("standard streams", DateTime.Now.Subtract(now)));
                // now = DateTime.Now;
                SaveChannels();

                isSnycing = false;

                if (ListLoaded != null)
                    ListLoaded(channels.Count, channels.Count, "Loaded " + channels.Count + " Channels");

                // spans.Add(new temp("Save channels", DateTime.Now.Subtract(now)));
                // now = DateTime.Now;
                List<Task> list = new List<Task>();
                list.Add(Task.Run(() => LoadChatLog()));

                list.Add(Task.Run(() => LoadChatHighlightWords()));

                Task.WaitAll(list.ToArray());
                // spans.Add(new temp("Wait Tasks", DateTime.Now.Subtract(now)));
                // now = DateTime.Now;
                Task.Run(() =>
                {
                    logInChats();

                    LX29_ChatClient.Addons.Scripts.ScriptClassCollection.LoadScripts();

                    AutoActions.Load();
                });
                //  spans.Add(new temp("dingens alle", DateTime.Now.Subtract(now)));
                //  now = DateTime.Now;

                if (ListLoaded != null)
                    ListLoaded(channels.Count, channels.Count, "Loaded " + channels.Count + " Channels");

                //var time = DateTime.Now.Subtract(System.Diagnostics.Process.GetCurrentProcess().StartTime);
                //System.Windows.Forms.MessageBox.Show(time.ToString());

                Task.Run(() => Emotes.FetchEmotes(channels.Values.ToList()));
                // spans.Add(new temp("start fetch emotes", DateTime.Now.Subtract(now)));
                //  now = DateTime.Now;
                startRefresher();
                // spans.Add(new temp("start refresher", DateTime.Now.Subtract(now)));
                // now = DateTime.Now;
                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //foreach (var s in spans)
                //{
                //    sb.AppendLine(s.name + ": " + s.span.TotalMilliseconds.ToString("F0") + "ms");
                //}
                //System.Windows.Forms.MessageBox.Show(sb.ToString());
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
                            Emotes.LoadChannelEmotes(channel.Name);
                            FetchChatUsers(channel.Name);
                        });
                    }
                    else
                    {
                        channel.ResetStreamStatus();
                    }
                }
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

        private static Dictionary<string, List<string>> LoadChannels()
        {
            var setts = new Dictionary<string, List<string>>();
            try
            {
                if (File.Exists(ChannelSave))
                {
                    setts = ChannelInfo.ParseSavedChannels(File.ReadAllLines(ChannelSave));
                    if (setts.Count == 0) File.Delete(ChannelSave);
                    return setts;
                }
            }
            catch { }
            if (File.Exists(Settings.dataDir + "Channels.txt"))
            {
                setts = ChannelInfo.ParseSavedChannels(File.ReadAllLines(Settings.dataDir + "Channels.txt"));
                File.Delete(Settings.dataDir + "Channels.txt");
            }
            return setts;
        }

        private static void LoadStandardStreams()
        {
            if (!channels.ContainsKey("lukix29"))
            {
                ApiResult result = TwitchApi.GetStreamOrChannel("79328905")[0];
                ChannelInfo ci = new ChannelInfo(result, true, true);

                channels.Add(ci.Name, ci);
            }
            else
            {
                var chan = channels["lukix29"] = new ChannelInfo(channels["lukix29"], true, true);
            }
        }

        private static void logInChats()
        {
            var list = channels.Where(
                t => t.Value.AutoLoginChat).Select(t => t.Value)
                .OrderBy(t => t.IsFixed).Select(t => t.Name).ToList();
            foreach (var s in list)
            {
                ChatClient.TryConnect(s);
            }
            //TryConnect("lx29_tcvc");
            ListUpdated();
        }

        private static void startRefresher()
        {
            TTData o = new TTData();
            System.Threading.Timer t =
                new System.Threading.Timer(new TimerCallback(updateChannels), o, Settings.UpdateInterval, System.Threading.Timeout.Infinite);
            o.t = t;
        }

        private static void updateChannels(object obj)
        {
            TTData o = (TTData)obj;
            UpdateChannels();
            if (LX29_Tools.HasInternetConnection)
            {
                o.t.Change(Settings.UpdateInterval, System.Threading.Timeout.Infinite);
            }
            else
            {
                o.t.Change(10000, System.Threading.Timeout.Infinite);
            }
        }

        public struct temp
        {
            public string name;
            public TimeSpan span;

            public temp(string Name, TimeSpan Span)
            {
                name = Name;
                span = Span;
            }
        }

        private class TTData
        {
            public System.Threading.Timer t;
        }
    }
}