using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using LX29_Twitch.JSON_Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Emotes
{
    public partial class EmoteCollection
    {
        private static readonly string[] BTTV_EMOTE_BASE_URL = new string[] { "https://cdn.betterttv.net/emote/{id}/1x", "https://cdn.betterttv.net/emote/{id}/2x", "https://cdn.betterttv.net/emote/{id}/3x" };
        private static readonly string[] FFZ_EMOTE_BASE_URL = new string[] { "https://cdn.frankerfacez.com/emoticon/{id}/1", "https://cdn.frankerfacez.com/emoticon/{id}/2", "https://cdn.frankerfacez.com/emoticon/{id}/4" };
        private static readonly string[] TWITCH_EMOTE_BASE_URL = new string[] { "https://static-cdn.jtvnw.net/emoticons/v1/{id}/1.0", "https://static-cdn.jtvnw.net/emoticons/v1/{id}/2.0", "https://static-cdn.jtvnw.net/emoticons/v1/{id}/3.0" };
        private readonly object locjo = new object();

        private Emoteionary emotionary = new Emoteionary();

        private int loadingCnt = 0;
        private string loadingInf = "";

        private int loadingMax = 369500;

        public Emoteionary Values
        {
            get { return emotionary; }
        }

        public void LoadChannelEmotes(IEnumerable<ChannelInfo> channels, bool showprogress = false)
        {
            try
            {
                int max = channels.Count() * 2;
                int cnt = 0;

                var timer = new LXTimer((o) => On_Loaded_channel(null, cnt, max, "Loading FFZ/BTTV Channels"), 1000, 500);

                List<Task> tasks = new List<Task>();
                foreach (var channel in channels)
                {
                    var t0 = Task.Run(() =>
                    {
                        parse_BTTV_Channel(channel.Name);
                        cnt++;
                    });
                    var t1 = Task.Run(() =>
                    {
                        parse_FFZ_Channel(channel.Name);
                        cnt++;
                    });
                    tasks.Add(t0);
                    tasks.Add(t1);
                    if (tasks.Count >= 10)
                    {
                        Task.WaitAll(tasks.ToArray());
                        tasks.Clear();
                    }
                }
                timer.Dispose();
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

        public void On_Loaded_channel(ChannelInfo ci, int count, int max, string info)
        {
            if (OnChannelLoaded != null)
                OnChannelLoaded(ci, (int)count, (int)max, info.Trim(':'));
        }

        private Dictionary<string, _emotesetchannelid> _load_EmoteSet_IDs()
        {
            try
            {
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
                ////                    if (!setid ) && !channel ))
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

                Dictionary<string, _emotesetchannelid> _emotesetename = new Dictionary<string, _emotesetchannelid>();

                string url = "https://twitchemotes.com/api_cache/v3/sets.json";

                loadingCnt = 0;
                loadingInf = "Loading Emote Sets";
                JsonSerializer jss = new JsonSerializer();
                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = null;

                    using (JsonTextReader reader = new JsonTextReader(new StreamReader(wc.OpenRead(url))))
                    {
                        while (true)
                        {
                            if (!reader.Read())
                                break;

                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                var val = reader.Value.ToString();
                                if (!reader.Read())
                                    break;

                                var id = jss.Deserialize<_emotesetchannelid>(reader);
                                _emotesetename.Add(val, id);
                                loadingCnt++;
                            }
                        }
                    }
                }
                return _emotesetename;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        return _load_EmoteSet_IDs();
                }
            }
            return new Dictionary<string, _emotesetchannelid>();
        }

        private void LoadChannelBadges(IEnumerable<ChannelInfo> channels, bool showprogress = false)
        {
            try
            {
                int max = channels.Count();
                List<Task<string>> tasks = new List<Task<string>>();
                Parallel.ForEach(channels, new Action<ChannelInfo>((channel) =>
                 {
                     //tasks.Add(Task<string>.Run(() => {
                     Badges.Fetch_Channel_Badges(channel);
                     //return channel.Name; }));
                     //while (tasks.Count > 6)
                     //{
                     //    var index = Task.WaitAny(tasks.ToArray());
                     //    var name = tasks[index].Result;
                     //    if (showprogress) _loaded_channel(null, (max * 2) - tasks.Count, max * 2, "Loading BTTV/FFZ Channel");
                     //    tasks.RemoveAt(index);
                     //}
                 }));
                //while (tasks.Count > 0)
                //{
                //    var index = Task.WaitAny(tasks.ToArray());
                //    var name = tasks[index].Result;
                //    if (showprogress) _loaded_channel(null, (max * 2) - tasks.Count, max * 2, "Loading BTTV/FFZ Channel");
                //    tasks.RemoveAt(index);
                //}
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
                if (!File.Exists(Settings._dataDir + "EmoteCache.txt")) return true;

                using (JsonTextReader reader = new JsonTextReader(new StreamReader(File.OpenRead(Settings._dataDir + "EmoteCache.txt"))))
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
                                emotionary.Add(em);
                                if (cnt % 2000 == 0)
                                {
                                    On_Loaded_channel(null, cnt, max, "Loading Emote Cache");
                                }
                                cnt++;
                            }
                        }
                    }
                }
            }
            catch
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
                    Add_FFZ_BTTV(id, emote.code, channel, emorig, origChannel);
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

                    string id = emote.id.ToString();

                    var emorig = (uri.EndsWith("global")) ? EmoteOrigin.FFZ_Global : EmoteOrigin.FFZ;
                    string owner = (emorig == EmoteOrigin.FFZ_Global) ? "Global_FFZ" : emote.owner.display_name;
                    string channel = uri.GetBetween("room/", "");
                    if (string.IsNullOrEmpty(channel))
                    {
                        channel = owner;
                    }

                    Add_FFZ_BTTV(id, name, channel, emorig, owner);
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
            LXTimer timer = null;
            try
            {
                timer = new LXTimer((o) => On_Loaded_channel(null, loadingCnt, loadingMax, loadingInf), 1000, 500);

                var _emotesetename = _load_EmoteSet_IDs();

                loadingMax = 245500;

                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = null;
                    wc.Headers.Add("Client-ID", TwitchApi.CLIENT_ID);

                    string url = "https://api.twitch.tv/kraken/chat/emoticon_images";

                    loadingCnt = 0;
                    loadingInf = "Loading Emotes";
                    JsonSerializer js = new JsonSerializer();
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
                                        if (_emotesetename.ContainsKey(set))
                                        {
                                            channel = _emotesetename[set].channel_name;
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
                                        Name = obj.code;
                                    }
                                    Emote em = new Emote(obj.id, Name, channel, set, emorig, channel);
                                    //if (set.Equals("33") || set.Equals("42") || set.Equals("0"))
                                    //{
                                    //    File.AppendAllText("emotestwerfgsd.txt", obj.id + " | " + Name + " | " + set + "\r\n");
                                    //}
                                    Add(em);
                                    loadingCnt++;
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
            finally
            {
                timer.Dispose();
            }
        }

        private void Save()
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();

                using (JsonWriter writer = new JsonTextWriter(new StreamWriter(Settings._dataDir + "EmoteCache.txt")))
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("created");
                    writer.WriteValue(DateTime.Now);

                    int max = emotionary.All.Count();
                    writer.WritePropertyName("count");
                    writer.WriteValue(max);

                    writer.WritePropertyName("emotes");
                    writer.WriteStartArray();
                    int cnt = 0;
                    foreach (var item in emotionary.All)
                    {
                        serializer.Serialize(writer, item);
                        // writer.WriteValue(json);
                        if (cnt % 2000 == 0)
                        {
                            On_Loaded_channel(null, cnt, max, "Saving Emotes");
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

        public class _emotesetchannelid
        {
            public string channel_id { get; set; }
            public string channel_name { get; set; }
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
        //        get { return set ) || value.channel_name ); }
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
        #region StandardEMotes

        public static readonly Dictionary<string, string> PrimeEmotes = new Dictionary<string, string>()
        {
        {"432",	">("},
        {"433",	":\\"},
        {"434",	":("},
        {"435",	"R)"},
        {"436",	":o"},
        {"437",	"o_o"},
        {"438",	":p"},
        {"439",	";)"},
        {"440",	":)"},
        {"441",	"B)"},
        {"442",	";p"},
        {"443",	":D"},
        {"444",	":z"},
        {"445",	"<3"},

        {"483",	"<3"},
        {"484",	"R)"},
        {"485",	"#/"},
        {"486",	":>"},
        {"487",	"<]"},
        {"488",	":7"},
        {"489",	":("},
        {"490",	":p"},
        {"491",	";p"},
        {"492",	":o"},
        {"493",	":\\"},
        {"494",	":z"},
        {"495",	":s"},
        {"496",	":D"},
        {"497",	"o_o"},
        {"498",	">("},
        {"499",	":)"},
        {"500",	"B)"},
        {"501",	";)"}
        };

        public static readonly HashSet<string> StandardEmoteID = new HashSet<string>()
        {
            "1","2","3","4","5","6","7","8","9","10","11","12","13","14"
        };

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
        {"10", ":/"},
        {"11", ";)"},
        {"12", ":p"},
        {"13", ";p"},
        {"14", "R)"},

        {"432",	">("},
        {"433",	":\\"},
        {"434",	":("},
        {"435",	"R)"},
        {"436",	":o"},
        {"437",	"o_o"},
        {"438",	":p"},
        {"439",	";)"},
        {"440",	":)"},
        {"441",	"B)"},
        {"442",	";p"},
        {"443",	":D"},
        {"444",	":z"},
        {"445",	"<3"},

        {"483",	"<3"},
        {"484",	"R)"},
        {"485",	"#/"},
        {"486",	":>"},
        {"487",	"<]"},
        {"488",	":7"},
        {"489",	":("},
        {"490",	":p"},
        {"491",	";p"},
        {"492",	":o"},
        {"493",	":\\"},
        {"494",	":z"},
        {"495",	":s"},
        {"496",	":D"},
        {"497",	"o_o"},
        {"498",	">("},
        {"499",	":)"},
        {"500",	"B)"},
        {"501",	";)"}
        };

        #endregion StandardEMotes

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

        public EmoteBase Add(Emote e)
        {
            if (e.IsEmpty) return null;

            lock (locjo)
            {
                if (!emotionary.Contains(e))
                {
                    emotionary.Add(e);
                    return e;
                    //_append_to_save(e);
                }
            }
            return emotionary[e];
        }

        public EmoteBase Add_FFZ_BTTV(string id, string name, string channel, EmoteOrigin origin, string channelName)
        {
            if (StandardEmotes.ContainsKey(id))
            {
                name = StandardEmotes[id];
            }
            Emote em = new Emote(id, name, channel, channel, origin, channelName);
            return Add(em);
        }

        public void DownloadAllEmotes()
        {
            if (!Finished) return;
            int cnt = 0;
            int length = emotionary.All.Count();
            foreach (var emote in emotionary.All)
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
                emotionary = new Emoteionary();
                emotionary.Dispose();
                Badges = new BadgeCollection();
                //loadedChannels = new HashSet<string>();
                if (loadnew)
                {
                    try
                    {
                        Directory.Delete(Settings._emoteDir, true);
                    }
                    catch
                    {
                    }
                }
                if (!Directory.Exists(Settings._emoteDir))
                {
                    Directory.CreateDirectory(Settings._emoteDir);
                }

                On_Loaded_channel(null, 0, 100, "Loading User Emotes");

                //_loaded_channel(null, emotess.UserEmotes.Count, emotess.UserEmotes.Count, "Loading User Emotes");

                var tb0 = Task.Run(() =>
                {
                    Badges.Fetch_Channel_Badges();
                    LoadChannelBadges(Channels.ToArray(), false);
                });
                var tb1 = Task.Run(() => Badges.Parse_FFZ_Badges());
                var tb2 = Task.Run(() => Badges.Parse_FFZ_Addon_Badges());

                On_Loaded_channel(null, 0, 100, "Loading Badges");

                if (LoadEmotes(loadnew))
                {
                    On_Loaded_channel(null, 0, 1, "Loading Global Emotes");
                    var t0 = Task.Run(() => parse_FFZ());
                    var t1 = Task.Run(() => parse_BTTV());

                    parse_Twitch_EmoteList();

                    LoadChannelEmotes(Channels, true);

                    Task.WaitAll(t0, t1);
                    Save();
                }
                else
                {
                    On_Loaded_channel(null, 1, 1, "Loaded Emote Cache.");
                }
                var tb3 = Task.Run(() => emotionary.LoadUserEmotes());

                On_Loaded_channel(null, 0, 2666, "Loading Emoji's");
                var emojicnt = emotionary.LoadEmojis();
                On_Loaded_channel(null, emojicnt, emojicnt, "Loading Emoji's");

                bool success = Task.WaitAll(new Task[] { tb0, tb1, tb2, tb3 }, 60000);
                if (!success) throw new TimeoutException();
                Badges.Load();

                long finish = DateTime.Now.Ticks;
                Finished = true;

                On_Loaded_channel(null, emotionary.Count, emotionary.Count,
                    "Finished Loading of " + emotionary.Count + " Emotes && " + Badges.Count + " Badges");

                if (!Settings.MessageCaching)
                {
                    Task.Run(() =>
                    {
                        int cnt = 0;
                        var all = ChatClient.Messages.Values.AllMessages;
                        int max = all.Count;
                        foreach (var key in all.Keys)
                        {
                            var chan = all[key].Where(t => (t.SendTime.Ticks <= finish));
                            foreach (var msg in chan)
                            {
                                msg.ReloadEmotes();
                                //On_Loaded_channel(null, cnt, max, "Reloading Emotes in Messages (" + key + ")");
                            }
                            cnt++;
                            On_Loaded_channel(null, cnt, max, "Reloading Emotes in Messages (" + key + ")");
                        }
                    });
                }
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

            string id = null;
            if (outgoing)
            {
                var emi = emotionary._UserEmotes.FirstOrDefault(t => t.Name.Equals(name));
                if (emi != null)
                {
                    id = emi.ID;
                }
            }
            var em = emotionary[id, name];
            if (em != null)
            {
                if (!(((em.Origin == EmoteOrigin.FFZ) || (em.Origin == EmoteOrigin.BTTV)) &&
                    ((!em.Channel.Contains(channel)) && (!em.Channel.ToLower().Contains("global")))))
                {
                    return new EmoteBase[] { em };
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                if (name.StartsWith(":"))// && _emoji_names.ContainsKey(Name))
                {
                    return emotionary._emoji_unicodes.Values.SkipWhile(t => t.Name.Equals(name)).Take(1);// _emoji_names[Name];
                }
                else
                {
                    try
                    {
                        if (name.Length % 2 == 0)
                        {
                            List<EmoteBase> unis = new List<EmoteBase>();
                            for (int i = 0; i < name.Length; i += 2)
                            {
                                var ch = name.Substring(i, 2);
                                if (emotionary._emoji_unicodes.ContainsKey(ch))
                                {
                                    var emoj = emotionary._emoji_unicodes[ch];
                                    unis.Add(emoj);
                                }
                            }
                            if (unis.Count > 0)
                                return unis;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        public IEnumerable<EmoteBase> GetEmotes(string channel)
        {
            if (!Finished) return null;
            return emotionary.GetEmotes(channel);
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
                if (!string.IsNullOrEmpty(message))
                {
                    Dictionary<int, ChatWord.TempEmoteWord> list = new Dictionary<int, ChatWord.TempEmoteWord>();
                    if (Finished && parameters != null && parameters.ContainsKey(irc_params.emotes))
                    {
                        string[] sem;
                        string[] sa;

                        string s = parameters[irc_params.emotes];
                        if (!string.IsNullOrEmpty(s))
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

                                            // var channelName = channel;
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

                                            // var urls = EmoteCollection.TWITCH_EMOTE_BASE_URL.Select(t => t.Replace("{id}", id));
                                            var addedEmote = ChatClient.Emotes.Values[id, nme];// ChatClient.Emotes.Add(id, nme, channel, channelName);

                                            list.Add(i0, new ChatWord.TempEmoteWord(i0, i1, addedEmote));// nme, id, set));
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
                                    ChatWords.Add(new ChatWord(item, word));
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