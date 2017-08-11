﻿using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using LX29_Twitch.JSON_Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LX29_ChatClient.Emotes
{
    public partial class EmoteCollection
    {
        private static readonly string[] BTTV_EMOTE_BASE_URL = new string[] { "https://cdn.betterttv.net/emote/{id}/1x", "https://cdn.betterttv.net/emote/{id}/2x", "https://cdn.betterttv.net/emote/{id}/3x" };
        private static readonly string[] FFZ_EMOTE_BASE_URL = new string[] { "https://cdn.frankerfacez.com/emoticon/{id}/1", "https://cdn.frankerfacez.com/emoticon/{id}/2", "https://cdn.frankerfacez.com/emoticon/{id}/4" };
        private static readonly string[] TWITCH_EMOTE_BASE_URL = new string[] { "https://static-cdn.jtvnw.net/emoticons/v1/{id}/1.0", "https://static-cdn.jtvnw.net/emoticons/v1/{id}/2.0", "https://static-cdn.jtvnw.net/emoticons/v1/{id}/3.0" };
        private readonly object locjo = new object();

        private Emoteionary emotess = new Emoteionary();

        public Emoteionary Values
        {
            get { return emotess; }
        }

        public void LoadChannelEmotes(IEnumerable<ChannelInfo> channels, bool showprogress = false)
        {
            try
            {
                int max = channels.Count();
                List<Task<string>> tasks = new List<Task<string>>();
                foreach (var channel in channels)
                {
                    tasks.Add(Task<string>.Run(() => { parse_BTTV_Channel(channel.Name); return channel.Name; }));
                    tasks.Add(Task<string>.Run(() => { parse_FFZ_Channel(channel.Name); return channel.Name; }));
                }
                while (tasks.Count > 0)
                {
                    var index = Task.WaitAny(tasks.ToArray());
                    var name = tasks[index].Result;
                    if (showprogress) _loaded_channel(null, (max * 2) - tasks.Count, max * 2, "Loading BTTV/FFZ Channel");
                    tasks.RemoveAt(index);
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        LoadChannelEmotes(channels, showprogress);
                        break;
                }
            }
        }

        public void LoadChannelEmotesAndBadges(ChannelInfo channel, bool showprogress = false)
        {
            var chans = new ChannelInfo[] { channel };
            LoadChannelEmotes(chans, showprogress);
            LoadChannelBadges(chans, showprogress);
        }

        private Dictionary<string, string> _load_EmoteSet_IDs(WebClient wc)
        {
            try
            {
                Dictionary<string, string> _emotesetename = new Dictionary<string, string>();

                #region FromFile

                ////if (File.Exists(Settings.dataDir + "emote_sets.txt"))
                ////{
                ////    FileInfo fi = new FileInfo(Settings.dataDir + "emote_sets.txt");
                ////    if (fi.Length < 1024)
                ////    {
                ////        File.Delete(Settings.dataDir + "emote_sets.txt");
                ////        return _load_EmoteSet_IDs(wc);
                ////    }
                ////    try
                ////    {
                ////        using (JsonTextReader reader = new JsonTextReader(new StreamReader(File.OpenRead(Settings.dataDir + "emote_sets.txt"))))
                ////        {
                ////            string channel = "";
                ////            string setid = "";
                ////            int cnt = 0;
                ////            while (true)
                ////            {
                ////                if (!reader.Read())
                ////                    break;
                ////                if (reader.Value != null)
                ////                {
                ////                    JProperty jt = JProperty.Load(reader);
                ////                    if (jt.HasValues)
                ////                    {
                ////                        channel = jt.Name;
                ////                        setid = jt.Value.ToString();
                ////                    }
                ////                    if (!setid.IsEmpty() && !channel.IsEmpty())
                ////                    {
                ////                        _emotesetename.Add(setid, channel);
                ////                        setid = "";
                ////                        channel = "";
                ////                        if (cnt % 1000 == 0)
                ////                        {
                ////                            _loaded_channel(null, cnt, Int32.MaxValue / 10000, "Loading Emote Set/Channel");
                ////                        }
                ////                        cnt++;
                ////                    }
                ////                }
                ////            }
                ////        }
                ////    }
                ////    catch
                ////    {
                ////    }
                ////    return _emotesetename;
                ////}
                ////else
                //// {

                #endregion FromFile

                //   JsonWriter sw = new JsonTextWriter(new StreamWriter(Settings.dataDir + "emote_sets.txt", false));
                //  sw.WriteStartObject();
                using (JsonTextReader reader = new JsonTextReader(new StreamReader(wc.OpenRead("https://twitchemotes.com/api_cache/v3/sets.json"))))
                {
                    int cnt = 0;
                    JsonSerializer jss = new JsonSerializer();
                    string setid = "";
                    string channel_name = "";
                    while (true)
                    {
                        if (!reader.Read())
                            break;
                        if (reader.Value != null)
                        {
                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                var temp = reader.Value.ToString();
                                if (temp.Equals("channel_name") && !setid.IsEmpty())
                                {
                                    if (!reader.Read())
                                        break;
                                    channel_name = reader.Value.ToString();

                                    //   sw.WritePropertyName(setid);
                                    //   sw.WriteValue(channel_name);

                                    _emotesetename.Add(setid, channel_name);
                                    if (cnt % 1000 == 0)
                                    {
                                        _loaded_channel(null, cnt, Int32.MaxValue / 10000, "Loading Emote Set/Channel");
                                    }
                                    cnt++;
                                    channel_name = "";
                                    setid = "";
                                }
                                else if (temp.Equals("channel_id"))
                                {
                                    if (!reader.Read())
                                        break;
                                    //var channel_id = reader.Value.ToString();
                                }
                                else
                                {
                                    setid = reader.Value.ToString();
                                }
                            }
                        }
                    }
                }
                // sw.WriteEndObject();
                // sw.Close();

                return _emotesetename;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        return _load_EmoteSet_IDs(wc);
                }
                //File.Delete(Settings.dataDir + "emote_sets.txt");
            }
            return new Dictionary<string, string>();
        }

        private void _loaded_channel(ChannelInfo ci, int count, int max, string info)
        {
            if (OnChannelLoaded != null)
                OnChannelLoaded(ci, (int)count, (int)max, info.Trim(':'));
        }

        private void LoadChannelBadges(IEnumerable<ChannelInfo> channels, bool showprogress = false)
        {
            try
            {
                int max = channels.Count();
                List<Task<string>> tasks = new List<Task<string>>();
                foreach (var channel in channels)
                {
                    tasks.Add(Task<string>.Run(() => { Badges.Fetch_Channel_Badges(channel); return channel.Name; }));
                    while (tasks.Count > 6)
                    {
                        var index = Task.WaitAny(tasks.ToArray());
                        var name = tasks[index].Result;
                        if (showprogress) _loaded_channel(null, (max * 2) - tasks.Count, max * 2, "Loading BTTV/FFZ Channel");
                        tasks.RemoveAt(index);
                    }
                }
                while (tasks.Count > 0)
                {
                    var index = Task.WaitAny(tasks.ToArray());
                    var name = tasks[index].Result;
                    if (showprogress) _loaded_channel(null, (max * 2) - tasks.Count, max * 2, "Loading BTTV/FFZ Channel");
                    tasks.RemoveAt(index);
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        // loadedChannels.RemoveWhere(t => channels.Any(n => n.Equals(t, StringComparison.OrdinalIgnoreCase)));
                        LoadChannelBadges(channels, showprogress);
                        break;
                }
            }
        }

        private bool LoadEmotes(bool loadNew)
        {
            if (loadNew) return true;

            try
            {
                if (!File.Exists(Settings.dataDir + "EmoteCache.txt")) return true;

                using (JsonTextReader reader = new JsonTextReader(new StreamReader(File.OpenRead(Settings.dataDir + "EmoteCache.txt"))))
                {
                    bool emoteStart = false;
                    int cnt = 0;
                    int max = 0;
                    JObject jo;
                    while (true)
                    {
                        if (!reader.Read())
                            return false;
                        if (reader.Value != null)
                        {
                            if (reader.Value.Equals("created"))
                            {
                                if (!reader.Read())
                                    return false;
                                DateTime dt = (DateTime)reader.Value;
                                if (DateTime.Now.Subtract(dt).TotalDays > 3)
                                {
                                    return true;
                                }
                            }
                            else if (reader.Value.Equals("count"))
                            {
                                if (!reader.Read())
                                    return false;
                                max = (int)(long)reader.Value;
                            }
                            else if (reader.Value.Equals("emotes"))
                            {
                                emoteStart = true;
                            }
                        }
                        else if (emoteStart)
                        {
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                jo = JObject.Load(reader);
                                var ChannelName = jo.GetValue("ChannelName").Value<string>(); ;
                                var Channel = jo.GetValue("Channel").Value<string>(); ;
                                var ID = jo.GetValue("ID").Value<string>(); ;
                                var Name = jo.GetValue("Name").Value<string>(); ;
                                var Origin = (EmoteOrigin)jo.GetValue("_origin").Value<int>(); ;
                                var Set = jo.GetValue("Set").Value<string>(); ;

                                Emote em = new Emote(ID, Name, Channel, Set, Origin, ChannelName);
                                emotess.Add(em);
                                if (cnt % 2000 == 0)
                                {
                                    _loaded_channel(null, cnt, max, "Loading Emote Cache");
                                }
                                cnt++;
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
            }
            return true;
        }

        private void parse_BTTV(string bttv_api_url = "https://api.betterttv.net/2/emotes")
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                string bttv = wc.DownloadString(bttv_api_url);
                wc.Dispose();

                var emotes = JSON.ParseBttvEmotes(bttv);
                foreach (var emote in emotes.emotes)
                {
                    string id = emote.id;
                    //   string url = "https:" + emotes.urlTemplate.Replace("{{id}}", id).Replace("{{image}}", "1x");
                    //var urls = BTTV_EMOTE_BASE_URL.Select(t => t.Replace("{id}", id));

                    var emorig = (emote.channel == null) ? EmoteOrigin.BTTV_Global : EmoteOrigin.BTTV;
                    string channel = (EmoteOrigin.BTTV_Global == emorig) ? "Global_BTTV" : emote.channel;

                    string origChannel = channel;
                    if (emote.channel != null)
                    {
                        channel = bttv_api_url.LastLine("/");
                    }
                    Add(id, emote.code, channel, emorig, origChannel);
                }
            }
            catch (Exception x)
            {
                if (x is WebException)
                {
                    var res = (HttpWebResponse)(((WebException)x).Response);
                    int code = (int)res.StatusCode;
                    if (code == (int)HttpStatusCode.GatewayTimeout ||
                        code == (int)HttpStatusCode.RequestTimeout)
                    {
                        parse_BTTV(bttv_api_url);
                        return;
                    }
                    else if (code == 404)
                    {
                        return;
                    }
                }
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        parse_BTTV(bttv_api_url);
                        break;
                }
            }
        }

        private void parse_BTTV_Channel(string channel)
        {
            parse_BTTV("https://api.betterttv.net/2/channels/" + channel);
        }

        private void parse_FFZ(string uri = "https://api.frankerfacez.com/v1/set/global")
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                string json = wc.DownloadString(uri);

                wc.Dispose();

                var emotes = JSON.ParseFFZEmotes(json);
                foreach (var emote in emotes)
                {
                    string name = emote.name;

                    //var urls = emote.urls.Select(t => "https:" + t.Value);// FFZ_EMOTE_BASE_URL.Select(t => t.Replace("{id}", emote.id.ToString())).ToList();

                    //tring url = "http:" + urls.Split(",")[0].GetBetween(":", "");

                    string id = (emote.id + Int32.MinValue).ToString();

                    var emorig = (uri.EndsWith("global")) ? EmoteOrigin.FFZ_Global : EmoteOrigin.FFZ;
                    string owner = (emorig == EmoteOrigin.FFZ_Global) ? "Global_FFZ" : emote.owner.display_name;
                    string channel = uri.GetBetween("room/", "");
                    if (channel.IsEmpty())
                    {
                        channel = owner;
                    }

                    Add(id, name, channel, emorig, owner);
                }
            }
            catch (Exception x)
            {
                if (x is WebException)
                {
                    var xw = x as WebException;
                    if (xw.Status != WebExceptionStatus.ProtocolError)
                    {
                        switch (x.Handle())
                        {
                            case MessageBoxResult.Retry:
                                parse_FFZ(uri);
                                break;
                        }
                    }
                }
                else
                {
                    switch (x.Handle())
                    {
                        case MessageBoxResult.Retry:
                            parse_FFZ(uri);
                            break;
                    }
                }
            }
        }

        private void parse_FFZ_Channel(string Channel)
        {
            string uri = "https://api.frankerfacez.com/v1/room/" + Channel;
            parse_FFZ(uri);
        }

        private void parse_Twitch_EmoteList()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("Client-ID", TwitchApi.CLIENT_ID);

                    var _emotesetename = _load_EmoteSet_IDs(wc);
                    string url = "https://api.twitch.tv/kraken/chat/emoticon_images";

                    // string propname = "";
                    // string value = "";
                    using (JsonTextReader reader = new JsonTextReader(new StreamReader(wc.OpenRead(url))))
                    {
                        while (true)
                        {
                            reader.Read();
                            if (reader.Value != null)
                            {
                                break;
                            }
                        }
                        int cnt = 0;
                        JsonSerializer js = new JsonSerializer();
                        while (true)
                        {
                            try
                            {
                                if (!reader.Read())
                                    break;
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    JSON.Twitch_Api.Emoticon obj = js.Deserialize<JSON.Twitch_Api.Emoticon>(reader);
                                    if (obj != null)
                                        if (string.IsNullOrEmpty(obj.id))
                                            continue;

                                    string set = obj.emoticon_set;

                                    string channel = "Twitch_Global";
                                    var emorig = EmoteOrigin.Twitch_Global;

                                    if (!set.Equals("0"))
                                    {
                                        if (_emotesetename.ContainsKey(obj.id))
                                        {
                                            channel = _emotesetename[obj.id];
                                            emorig = EmoteOrigin.Twitch;
                                        }
                                    }

                                    string Name = "";
                                    if (EmoteCollection.StandardEmotes.ContainsKey(obj.id))
                                    {
                                        Name = EmoteCollection.StandardEmotes[obj.id];
                                    }
                                    else
                                    {
                                        Name = obj.code;// list["code"];
                                    }
                                    Emote em = new Emote(obj.id, Name, channel, set, emorig, channel);
                                    Add(em);

                                    if (cnt % 1000 == 0)
                                    {
                                        _loaded_channel(null, cnt, Int32.MaxValue / 10000, "Loading Emotes");
                                    }
                                    cnt++;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        parse_Twitch_EmoteList();
                        break;
                }
            }
        }

        private void Save()
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();

                using (JsonWriter writer = new JsonTextWriter(new StreamWriter(Settings.dataDir + "EmoteCache.txt")))
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("created");
                    writer.WriteValue(DateTime.Now);

                    int max = emotess.All.Count();
                    writer.WritePropertyName("count");
                    writer.WriteValue(max);

                    writer.WritePropertyName("emotes");
                    writer.WriteStartArray();
                    int cnt = 0;
                    foreach (var item in emotess.All)
                    {
                        serializer.Serialize(writer, item);
                        // writer.WriteValue(json);
                        if (cnt % 2000 == 0)
                        {
                            _loaded_channel(null, cnt, max, "Saving Emotes");
                        }
                        cnt++;
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Save();
                        break;
                }
            }
        }

        //public class SetChannel
        //{
        //    public string channel_id { get; set; }
        //    public string channel_name { get; set; }
        //}

        //public class SetInfo
        //{
        //    [JsonIgnore]
        //    public bool IsEmpty
        //    {
        //        get { return set.IsEmpty() || value.channel_name.IsEmpty(); }
        //    }

        //    // public string id { get; set; }
        //    public string set { get; set; }

        //    //public SetInfo(string _name, string _set)
        //    //{
        //    //    // id = _id;
        //    //    channel = _name;
        //    //    set = _set;
        //    //}
        //    public SetChannel value { get; set; }
        //}

        //private List<Emote> FindEmotes()
        //{
        //    if (!Finished) return null;
        //    string id = null;

        //        var emi = userEmotes.LastOrDefault(t => t.name.Contains(name));
        //        if (emi != null)
        //        {
        //            id = emi.id.ToString();
        //        }
        //    var em = emotess[id, name];
        //    if (em != null)
        //    {
        //        if (!(((em.Origin == EmoteOrigin.FFZ) || (em.Origin == EmoteOrigin.BTTV)) &&
        //            ((!em.Channel.Contains(channel)) && (!em.Channel.ToLower().Contains("global")))))
        //        {
        //            return em;
        //        }
        //        else
        //        {
        //        }
        //    }
        //    return null;
        //}
    }

    public partial class EmoteCollection
    {
        public static readonly Dictionary<string, string> StandardEmotes = new Dictionary<string, string>()
        {
        {"1", ":)"},
        {"2", ":("},
        {"3", ":D"},
        {"4", ";("},
        {"5", ":z"},
        {"6", "o_o"},
        {"7", "B)"},
        {"8", ":o"},
        {"10",":/"},
        {"11",";)"},
        {"12",":p"},
        {"13",";p"},
        {"14","R)"},
        };

        public int MaxEmoteFetchCount = 6;

        public EmoteCollection()
        {
            //Task.Run(loadStdEmotes);
        }

        public delegate void ChannelLoaded(ChannelInfo ci, int count, int max, string info);

        public static event ChannelLoaded OnChannelLoaded;

        public BadgeCollection Badges
        {
            get;
            private set;
        }

        public bool Finished
        {
            get;
            private set;
        }

        public EmoteBase Add(string id, string name, string channel, EmoteOrigin origin, string channelName)
        {
            string emoteSet = "";
            //var emSet = emotess.EmoteSets.FirstOrDefault(t => t.Name.Equals(channel, StringComparison.OrdinalIgnoreCase));
            //if (emSet != null)
            //{
            //    emoteSet = emSet.Set;
            //}
            if (StandardEmotes.ContainsKey(id))
            {
                name = StandardEmotes[id];
            }
            Emote em = new Emote(id, name, channel, emoteSet, origin, channelName);
            return Add(em);
        }

        public EmoteBase Add(Emote e)
        {
            if (e.IsEmpty) return null;

            if (!emotess.Contains(e))
            {
                lock (locjo)
                {
                    emotess.Add(e);
                }
                return e;
                //_append_to_save(e);
            }
            else
            {
                return emotess[e];
            }
        }

        public void DownloadAllEmotes()
        {
            if (!Finished) return;
            int cnt = 0;
            int length = emotess.All.Count();
            foreach (var emote in emotess.All)
            {
                emote.DownloadImages();
                cnt++;
                if (OnChannelLoaded != null)
                    OnChannelLoaded(null, cnt, length, emote.Name);
            }
        }

        public void FetchEmotes(List<ChannelInfo> Channels, bool loadnew)
        {
            try
            {
                Finished = false;
                emotess = new Emoteionary();
                emotess.Dispose();
                Badges = new BadgeCollection();
                //loadedChannels = new HashSet<string>();
                if (loadnew)
                {
                    try
                    {
                        Directory.Delete(Settings.emoteDir, true);
                    }
                    catch
                    {
                    }
                }
                if (!Directory.Exists(Settings.emoteDir))
                {
                    Directory.CreateDirectory(Settings.emoteDir);
                }

                _loaded_channel(null, 0, emotess.UserEmotes.Count, "Loading User Emotes");
                emotess.UserEmotes = LX29_Twitch.Api.TwitchApi.GetUserEmotes().ToList();

                //_loaded_channel(null, emotess.UserEmotes.Count, emotess.UserEmotes.Count, "Loading User Emotes");

                var tb0 = Task.Run(() => { Badges.Fetch_Channel_Badges(); LoadChannelBadges(Channels.ToArray(), false); });
                var tb1 = Task.Run(() => Badges.Parse_FFZ_Badges());
                var tb2 = Task.Run(() => Badges.Parse_FFZ_Addon_Badges());

                _loaded_channel(null, 0, emotess.UserEmotes.Count, "Loading Badges");

                if (LoadEmotes(loadnew))
                {
                    _loaded_channel(null, 0, 1, "Loading Global Emotes");
                    var t0 = Task.Run(() => parse_FFZ());
                    var t1 = Task.Run(() => parse_BTTV());

                    parse_Twitch_EmoteList();

                    var chans = Channels.Where(s => s.IsOnline ||
                            s.AutoLoginChat ||
                            s.IsFavorited ||
                            s.SubInfo.IsSub).ToArray();
                    LoadChannelEmotes(chans, true);
                    Task.Run(() =>
                    {
                        var restchans = Channels.Except(chans).ToArray();
                        LoadChannelEmotes(restchans, false);
                        Save();
                    });
                    Task.WaitAll(t0, t1);
                }
                else
                {
                    _loaded_channel(null, 1, 1, "Loaded Emote Cache.");
                }

                try
                {
                    Task.WaitAll(new Task[] { tb1, tb2 }, Int32.MaxValue);
                }
                catch
                {
                }
                _loaded_channel(null, 0, 2666, "Loading Emoji's");
                var emojicnt = emotess.LoadEmojis();
                _loaded_channel(null, emojicnt, emojicnt, "Loading Emoji's");

                Finished = true;

                Task.Run(() =>
                {
                    foreach (var msg in ChatClient.Messages.Messages)
                    {
                        msg.Value.ForEach(t => t.ReloadEmotes());
                    }
                });

                _loaded_channel(null, emotess.Count, emotess.Count,
                    "Finished Loading of " + emotess.Count + " Emotes && " + Badges.Count + " Badges");
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        FetchEmotes(Channels, loadnew);
                        break;
                }
            }
        }

        public IEnumerable<EmoteBase> GetEmote(string name, string channel, bool outgoing = false)
        {
            if (!Finished) return null;

            if (!string.IsNullOrEmpty(name))
            {
                if (name.StartsWith(":"))// && _emoji_names.ContainsKey(Name))
                {
                    return emotess._emoji_unicodes.Values.SkipWhile(t => t.Name.Equals(name)).Take(1);// _emoji_names[Name];
                }
                else
                {
                    try
                    {
                        List<EmoteBase> unis = new List<EmoteBase>();
                        for (int i = 0; i < name.Length - 2; i += 2)
                        {
                            //string ch = name.Substring(i, 2);
                            int uni = char.ConvertToUtf32(name[i], name[i + 1]);
                            if (emotess._emoji_unicodes.ContainsKey(uni))
                            {
                                var emoj = emotess._emoji_unicodes[uni];
                                unis.Add(emoj);
                            }
                        }
                        if (unis.Count > 0)
                            return unis;
                    }
                    catch
                    {
                    }
                }
            }

            string id = null;
            if (outgoing)
            {
                var emi = emotess.UserEmotes.LastOrDefault(t => t.code.Equals(name));
                if (emi != null)
                {
                    id = emi.id.ToString();
                }
            }
            var em = emotess[id, name];
            if (em != null)
            {
                if (!(((em.Origin == EmoteOrigin.FFZ) || (em.Origin == EmoteOrigin.BTTV)) &&
                    ((!em.Channel.Contains(channel)) && (!em.Channel.ToLower().Contains("global")))))
                {
                    return new EmoteBase[] { em };
                }
                else
                {
                }
            }
            return null;
        }

        public IEnumerable<EmoteBase> GetEmotes(string channel)
        {
            if (!Finished) return null;
            return emotess.GetEmotes(channel);
        }

        //public void LoadChannelEmotes(params ChannelInfo[] channels)
        //{
        //    if (Finished)
        //    {
        //        LoadChannelEmotesBadges(false, channels);
        //    }
        //}

        public List<ChatWord> ParseEmoteFromMessage(Dictionary<irc_params, string> parameters, string message, string channel, HashSet<MsgType> typeList)
        {
            try
            {
                if (!message.IsEmpty())
                {
                    Dictionary<int, ChatWord.TempWord> list = new Dictionary<int, ChatWord.TempWord>();
                    if (Finished && parameters != null && parameters.ContainsKey(irc_params.emotes))
                    {
                        string[] sem;
                        string[] sa;

                        string s = parameters[irc_params.emotes];
                        if (!s.IsEmpty())
                        {
                            try
                            {
                                sem = s.Split("/");

                                foreach (string em in sem)
                                {
                                    sa = em.Split(":");
                                    if (sa.Length > 1)
                                    {
                                        string id = (sa[0].Trim());
                                        var posarr = sa[1].Split(",");
                                        foreach (var pos in posarr)
                                        {
                                            var si = pos.Split("-");
                                            int i0 = int.Parse(si[0]);
                                            int i1 = int.Parse(si[1]);
                                            string nme = message.GetBetween(i0, i1).Trim();

                                            var channelName = channel;
                                            var emorig = EmoteOrigin.Twitch_Global;
                                            //var setid = emotess.EmoteSets.FirstOrDefault(t => t.ID.Equals(id));
                                            //if (setid != null)
                                            //{
                                            //    channel = setid.Name;
                                            //    emorig = EmoteOrigin.Twitch;
                                            //}
                                            //else
                                            //{
                                            //    channel = "NlC";
                                            //}

                                            var urls = EmoteCollection.TWITCH_EMOTE_BASE_URL.Select(t => t.Replace("{id}", id));
                                            var addedEmote = ChatClient.Emotes.Add(id, nme, channel, emorig, channelName);

                                            list.Add(i0, new ChatWord.TempWord(i0, i1, addedEmote));// nme, id, set));
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    var ChatWords = new List<ChatWord>();
                    int index = 0;
                    string word = "";
                    while (true)
                    {
                        char c = message[index];
                        word += c;
                        if (char.IsWhiteSpace(c) || index == message.Length - 1)
                        {
                            if (word.Length > 0)
                            {
                                if (list.Count > 0 && list.ContainsKey(index - (word.Length - 1)))
                                {
                                    var item = list[index - (word.Length - 1)];
                                    ChatWords.Add(new ChatWord(item));
                                    index = item.End;
                                }
                                else
                                {
                                    word = word.Trim();
                                    ChatWords.Add(new ChatWord(word, channel, typeList.Contains(MsgType.Outgoing)));
                                }
                                word = string.Empty;
                            }
                        }
                        index++;
                        if (index == message.Length) break;
                    }
                    return ChatWords;
                }
            }
            catch
            {
            }
            return new List<ChatWord>();
        }
    }
}