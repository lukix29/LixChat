using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using LX29_Twitch.JSON_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                    var res = (x as WebException).Response as HttpWebResponse;
                    var code = (int)res.StatusCode;
                    if (!code.Equals(404))
                    {
                        switch (x.Handle())
                        {
                            case MessageBoxResult.Retry:
                                parse_BTTV(bttv_api_url);
                                break;
                        }
                    }
                }
                else
                {
                    switch (x.Handle())
                    {
                        case MessageBoxResult.Retry:
                            parse_BTTV(bttv_api_url);
                            break;
                    }
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

    public class Emoteionary
    {
        private Dictionary<string, Emote> _ffzbttv = new Dictionary<string, Emote>();

        private Dictionary<string, Emote> _twitch = new Dictionary<string, Emote>();

        public Emoteionary()
        {
            EmoteSets = new List<EmoteApiInfo>();
            UserEmotes = new List<EmoteApiInfo>();
        }

        public IEnumerable<Emote> All
        {
            get { return _twitch.Values.Concat(_ffzbttv.Values); }
        }

        public int Count
        {
            get { return _ffzbttv.Count + _twitch.Count; }
        }

        public List<EmoteApiInfo> EmoteSets
        {
            get;
            set;
        }

        public List<EmoteApiInfo> UserEmotes
        {
            get;
            set;
        }

        public Emote this[string twitch, string ffzbttv]
        {
            get
            {
                if (!string.IsNullOrEmpty(twitch) && _twitch.ContainsKey(twitch))
                {
                    return _twitch[twitch];
                }
                if (!string.IsNullOrEmpty(ffzbttv) && _ffzbttv.ContainsKey(ffzbttv))
                {
                    return _ffzbttv[ffzbttv];
                }
                return null;
            }
        }

        public Emote this[Emote emote]
        {
            get
            {
                if (_twitch.ContainsKey(emote.ID))
                {
                    return _twitch[emote.ID];
                }
                if (_ffzbttv.ContainsKey(emote.Name))
                {
                    return _ffzbttv[emote.Name];
                }
                throw new KeyNotFoundException();
            }
        }

        public void Add(Emote e)
        {
            if (e.Origin == EmoteOrigin.Twitch || e.Origin == EmoteOrigin.Twitch_Global)
            {
                if (!_twitch.ContainsKey(e.ID))
                    _twitch.Add(e.ID, e);
            }
            else
            {
                if (!_ffzbttv.ContainsKey(e.Name))
                    _ffzbttv.Add(e.Name, e);
            }
        }

        //public Emote this[ChatWord.TempWord Key]
        //{
        //    get
        //    {
        //        if (_twitch.ContainsKey(Key.ID))
        //        {
        //            return _twitch[Key.ID];
        //        }
        //        if (_ffzbttv.ContainsKey(Key.Name))
        //        {
        //            return _ffzbttv[Key.Name];
        //        }
        //        return null;
        //    }
        //}
        public bool Contains(Emote Key)
        {
            if (Key.Origin == EmoteOrigin.Twitch || Key.Origin == EmoteOrigin.Twitch_Global)
            {
                return _twitch.ContainsKey(Key.ID);
            }
            else
            {
                return _ffzbttv.ContainsKey(Key.Name);
            }
        }

        public IEnumerable<Emote> Find(string name, string channel)
        {
            var em = _twitch.Values.Where((e) =>
               {
                   if (e.Name.ToLower().StartsWith(name))
                   {
                       return UserEmotes.Any(t => t.ID.Equals(e.ID));
                   }
                   return false;
               });

            var ffz = _ffzbttv.Values.Where((t) =>
                {
                    if (t.Name.ToLower().StartsWith(name))
                    {
                        if ((t.Channel.Contains(channel)) || (t.Origin == EmoteOrigin.FFZ_Global || t.Origin == EmoteOrigin.BTTV_Global))
                        {
                            return true;
                        }
                    }
                    return false;
                });

            em = em.Concat(ffz);
            return em.OrderByDescending(t =>
                (t.Origin == EmoteOrigin.FFZ_Global || t.Origin == EmoteOrigin.BTTV_Global || t.Origin == EmoteOrigin.Twitch_Global))
                .ThenBy(t => t.Name).ToList();
        }

        public IEnumerable<Emote> GetEmotes(string channel)
        {
            var ems = _twitch.Values
                .Where(t => UserEmotes.Any(t0 => t.ID.Equals(t0.ID))).ToList();

            //  int i = null;
            var ffz = _ffzbttv.Values.Where((t) =>
                {
                    if (t.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase)
                        || t.Origin == EmoteOrigin.BTTV_Global || t.Origin == EmoteOrigin.FFZ_Global)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }).ToList();

            return ems.Concat(ffz);
        }
    }
}