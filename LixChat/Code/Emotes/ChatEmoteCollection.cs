using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using LX29_Twitch.JSON_Parser;
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

        private Emoteionary emotess = new Emoteionary();
        private HashSet<string> loadedChannels = new HashSet<string>();

        public Emoteionary Values
        {
            get { return emotess; }
        }

        private void _load_EmoteSet_IDs()
        {
            try
            {
                //Comparer<string> comp = Comparer<string>.Create(new Comparison<string>((s0, s1) => int.Parse(s0).CompareTo(int.Parse(s1))));
                emotess.EmoteSets = new List<EmoteApiInfo>();
                WebClient wc = new WebClient();
                wc.Proxy = null;
                string value = wc.DownloadString("https://twitchemotes.com/api_cache/v2/sets.json").Replace("\"", "");
                value = value.GetBetween("sets:{", "}");
                var sets = value.Split(",");
                foreach (var set in sets)
                {
                    var vals = set.Split(":");
                    var set_id = vals[0];
                    string name = vals[1];
                    if (!name.Equals("--hidden--"))
                    {
                        emotess.EmoteSets.Add(new EmoteApiInfo(name, "", set_id));
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        _load_EmoteSet_IDs();
                        break;
                }
            }
        }

        private void _loaded_channel(ChannelInfo ci, int count, int max, string info)
        {
            if (OnChannelLoaded != null)
                OnChannelLoaded(ci, (int)count, (int)max,
                    info.Trim(':') + " (" + count.ToString("N0") + "/" + max.ToString("N0") + ")");
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
                    var urls = BTTV_EMOTE_BASE_URL.Select(t => t.Replace("{id}", id));

                    var emorig = (emote.channel == null) ? EmoteOrigin.BTTV_Global : EmoteOrigin.BTTV;
                    string channel = (EmoteOrigin.BTTV_Global == emorig) ? "Global_BTTV" : emote.channel;

                    string origChannel = channel;
                    if (emote.channel != null)
                    {
                        channel = bttv_api_url.LastLine("/");
                    }
                    Add(id, emote.code, urls, channel, emorig, origChannel);
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

                    var urls = emote.urls.Select(t => "https:" + t.Value);// FFZ_EMOTE_BASE_URL.Select(t => t.Replace("{id}", emote.id.ToString())).ToList();

                    //tring url = "http:" + urls.Split(",")[0].GetBetween(":", "");

                    string id = (emote.id + Int32.MinValue).ToString();

                    var emorig = (uri.EndsWith("global")) ? EmoteOrigin.FFZ_Global : EmoteOrigin.FFZ;
                    string owner = (emorig == EmoteOrigin.FFZ_Global) ? "Global_FFZ" : emote.owner.display_name;
                    string channel = uri.GetBetween("room/", "");
                    if (string.IsNullOrEmpty(channel))
                    {
                        channel = owner;
                    }

                    Add(id, name, urls, channel, emorig, owner);
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

        private void parse_Twitch_Channel(string Emote_Set)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                wc.Headers.Add("Client-ID", TwitchApi.CLIENT_ID);

                //Load Channel Twitch Sub emotes on ChatOpen (?)

                _loaded_channel(null, 2, 2, "Downloading Twitch Emote List");
                string Result = wc.DownloadString(new Uri("https://api.twitch.tv/kraken/chat/emoticon_images?emotesets=" + Emote_Set));
                var list = JSON.ParseTwitchEmotes(Result);
                int ContentLength = list.Count;

                foreach (var emote in list)
                {
                    string set = emote.Set;
                    string channel = "Twitch_Global";
                    var emorig = EmoteOrigin.Twitch_Global;
                    if (!set.Equals("0"))
                    {
                        var setid = emotess.EmoteSets.FirstOrDefault(t => t.Set.Equals(set));
                        if (setid != null)
                        {
                            channel = setid.Name;
                            emorig = EmoteOrigin.Twitch;
                        }
                    }
                    var id = emote.ID.ToString();
                    var urls = TWITCH_EMOTE_BASE_URL.Select(t => t.Replace("{id}", id));

                    Emote em = new Emote(id, emote.Name, urls, channel, set, emorig, channel);
                    Add(em);
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        parse_Twitch_Channel(Emote_Set);
                        break;
                }
            }
        }

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

        public EmoteBase Add(string id, string name, IEnumerable<string> url, string channel, EmoteOrigin origin, string channelName)
        {
            string emoteSet = "";
            var emSet = emotess.EmoteSets.FirstOrDefault(t => t.Name.Equals(channel, StringComparison.OrdinalIgnoreCase));
            if (emSet != null)
            {
                emoteSet = emSet.Set;
            }
            if (StandardEmotes.ContainsKey(id))
            {
                name = StandardEmotes[id];
            }
            Emote em = new Emote(id, name, url, channel, emoteSet, origin, channelName);
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
                loadedChannels = new HashSet<string>();
                try
                {
                    Directory.Delete(Settings.emoteDir, true);
                }
                catch
                {
                }
                if (!Directory.Exists(Settings.emoteDir))
                {
                    Directory.CreateDirectory(Settings.emoteDir);
                }

                emotess.UserEmotes = LX29_Twitch.Api.TwitchApi.GetUserEmotes();
                _loaded_channel(null, 0, emotess.UserEmotes.Count, "Loading Emote Infos");
                emotess.UserEmotes.ForEach((a) =>
                {
                    string id = a.ID.ToString();
                    if (StandardEmotes.ContainsKey(id))
                    {
                        a.Name = StandardEmotes[id];
                    }
                });

                _load_EmoteSet_IDs();

                _loaded_channel(null, emotess.UserEmotes.Count, emotess.UserEmotes.Count, "Loading Emote Infos");

                var tb0 = Task.Run(() => Badges.Fetch_Badges());
                var tb1 = Task.Run(() => Badges.Parse_FFZ_Badges());
                var tb2 = Task.Run(() => Badges.Parse_FFZ_Addon_Badges());

                if (emotess.LoadEmotes(loadnew))
                {
                    _loaded_channel(null, 0, 1, "Loading Global Emotes");
                    var t0 = Task.Run(() => parse_FFZ());
                    var t1 = Task.Run(() => parse_BTTV());

                    var sets = new string(emotess.UserEmotes.Select(t => t.Set).Distinct().SelectMany(t => t + ",").ToArray()).Trim(',');

                    parse_Twitch_Channel(sets);

                    Task.WaitAll(t0, t1);

                    var chans = Channels.Where(s => s.IsOnline || s.AutoLoginChat || s.IsFavorited || s.SubInfo.IsSub).Select(t => t.Name).ToArray();
                    _loadChannelEmotes(chans);
                }
                else
                {
                    _loaded_channel(null, 1, 1, "Loaded Emote Cache.");
                }
                //Task.WaitAll(tb0, tb1, tb2);

                _loaded_channel(null, 0, 2666, "Loading Emoji's");
                var emojicnt = emotess.LoadEmojis();
                _loaded_channel(null, emojicnt, emojicnt, "Loading Emoji's");

                Task.Run(() =>
                {
                    foreach (var msg in ChatClient.Messages.Messages)
                    {
                        msg.Value.ForEach(t => t.ReloadEmotes());
                    }
                });

                Finished = true;

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

        public EmoteBase GetEmote(string name, string channel, bool outgoing = false)
        {
            if (!Finished) return null;
            string id = null;
            if (outgoing)
            {
                var emi = emotess.UserEmotes.LastOrDefault(t => t.Name.Equals(name));
                if (emi != null)
                {
                    id = emi.ID;
                }
            }
            var em = emotess[id, name];
            if (em != null)
            {
                if (!(((em.Origin == EmoteOrigin.FFZ) || (em.Origin == EmoteOrigin.BTTV)) &&
                    ((!em.Channel.Contains(channel)) && (!em.Channel.ToLower().Contains("global")))))
                {
                    return em;
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

        public void LoadChannelEmotes(params string[] channels)
        {
            if (Finished)
            {
                _loadChannelEmotes(channels);
            }
        }

        public List<ChatWord> ParseEmoteFromMessage(Dictionary<irc_params, string> parameters, string message, string channel, HashSet<MsgType> typeList)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    Dictionary<int, ChatWord.TempWord> list = new Dictionary<int, ChatWord.TempWord>();
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

                                            var channelName = channel;
                                            var emorig = EmoteOrigin.Twitch_Global;
                                            var setid = emotess.EmoteSets.FirstOrDefault(t => t.ID.Equals(id));
                                            if (setid != null)
                                            {
                                                channel = setid.Name;
                                                emorig = EmoteOrigin.Twitch;
                                            }
                                            else
                                            {
                                                channel = "NlC";
                                            }

                                            var urls = EmoteCollection.TWITCH_EMOTE_BASE_URL.Select(t => t.Replace("{id}", id));
                                            var addedEmote = ChatClient.Emotes.Add(id, nme, urls, channel, emorig, channelName);

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
                                    ChatWords.Add(new ChatWord(word.Trim(), channel, typeList.Contains(MsgType.Outgoing)));
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

        private void _loadChannelEmotes(params string[] channels)
        {
            try
            {
                int cnt = 0;
                List<Task<string>> tasks = new List<Task<string>>();
                foreach (var channel in channels)
                {
                    if (loadedChannels.Contains(channel))
                        continue;

                    _loaded_channel(null, cnt, channels.Count(), "Loading BTTV/FFZ Emotes && Channel Badges");

                    tasks.Add(Task.Run<string>(() =>
                    {
                        var channelinfo = ChatClient.Channels[channel];
                        Badges.Fetch_Sub_Badges(channelinfo);
                        parse_BTTV_Channel(channel);
                        parse_FFZ_Channel(channel);
                        return channel;
                    }));
                    _loaded_channel(null, cnt, channels.Count(), "Loading BTTV/FFZ Emotes && Channel Badges");
                    if (tasks.Count >= MaxEmoteFetchCount)
                    {
                        var index = Task.WaitAny(tasks.ToArray());
                        var result = tasks[index].Result;
                        tasks.RemoveAt(index);
                        loadedChannels.Add(result);
                        cnt++;
                        _loaded_channel(null, cnt, channels.Count(), "Loading BTTV/FFZ Emotes && Channel Badges");
                    }
                }
                emotess.SaveEmotes();
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        loadedChannels.RemoveWhere(t => channels.Any(n => n.Equals(t, StringComparison.OrdinalIgnoreCase)));
                        _loadChannelEmotes(channels);
                        break;
                }
            }
        }
    }
}