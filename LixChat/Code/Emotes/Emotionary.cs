using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using LX29_Twitch.JSON_Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace LX29_ChatClient.Emotes
{
    public class Emoteionary
    {
        private Dictionary<string, Emoji> _emojis = new Dictionary<string, Emoji>();

        private Dictionary<string, EmoteBase> _ffzbttv = new Dictionary<string, EmoteBase>();

        private Dictionary<string, EmoteBase> _twitch = new Dictionary<string, EmoteBase>();

        public Emoteionary()
        {
            EmoteSets = new List<EmoteApiInfo>();
            UserEmotes = new List<EmoteApiInfo>();
        }

        public IEnumerable<EmoteBase> All
        {
            get { return _twitch.Values.Concat(_ffzbttv.Values); }
        }

        public int Count
        {
            get { return _ffzbttv.Count + _twitch.Count + _emojis.Count; }
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

        public EmoteBase this[string ID, string Name]
        {
            get
            {
                if (!string.IsNullOrEmpty(Name) && Name.StartsWith(":") && _emojis.ContainsKey(Name))
                {
                    return _emojis[Name];
                }
                if (!string.IsNullOrEmpty(ID) && _twitch.ContainsKey(ID))
                {
                    return _twitch[ID];
                }
                if (!string.IsNullOrEmpty(Name) && _ffzbttv.ContainsKey(Name))
                {
                    return _ffzbttv[Name];
                }
                return null;
            }
        }

        public EmoteBase this[EmoteBase emote]
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

        public void Add(EmoteBase e)
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

        public void CheckLoadingTime()
        {
            foreach (var em in All)
            {
                if (em.LoadedTime.TotalMinutes > 5.0)
                {
                    em.Dispose();
                }
            }
            foreach (var emoji in _emojis.Values)
            {
                if (emoji.LoadedTime.TotalMinutes > 5.0)
                {
                    emoji.Dispose();
                }
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
        public bool Contains(EmoteBase Key)
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

        public void Dispose()
        {
            foreach (var kvp in All)
            {
                kvp.Dispose();
            }
        }

        public List<EmoteBase> Find(string name, string channel)
        {
            if (name.StartsWith(":"))
            {
                name = name.ToLower();
                return _emojis.Values.Where(t => t.Name.StartsWith(name)).Select(t => (EmoteBase)t).ToList();
            }
            else
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
        }

        public IEnumerable<EmoteBase> GetEmotes(string channel)
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

        public int LoadEmojis()
        {
            string line = "";
            _emojis = new Dictionary<string, Emoji>();
            using (StringReader sr = new StringReader(LX29_LixChat.Properties.Resources.emojis))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    var arr = line.Split('=');
                    var name = arr[1];
                    var id = arr[0];
                    _emojis.Add(name, new Emoji(id, name));
                }
            }
            return _emojis.Count;
        }

        public bool LoadEmotes(bool loadNew)
        {
            if (loadNew) return true;
            try
            {
                var input = File.ReadAllLines(Settings.dataDir + "EmoteCache.txt");
                DateTime dt = new DateTime(long.Parse(input[0]));
                if (DateTime.Now.Subtract(dt).TotalHours < 24.0)
                {
                    var res = JsonConvert.DeserializeObject<List<Emote>>(input[1]);
                    foreach (var em in res)
                    {
                        Emote m = new Emote(em.ID, em.Name, em.URLs, em.Channel, em.Set, em.Origin, em.ChannelName);
                        Add(m);
                    }
                    return false;
                }
            }
            catch
            {
            }
            return true;
        }

        public void SaveEmotes()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            serializer.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;

            using (StreamWriter sw = new StreamWriter(Settings.dataDir + "EmoteCache.txt"))
            {
                sw.WriteLine(DateTime.Now.Ticks);
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.WriteStartArray();
                    foreach (var item in All)
                    {
                        string val = JsonConvert.SerializeObject((item as Emote));
                        writer.WriteRawValue(val);
                    }
                    writer.WriteEndArray();
                }
            }
        }
    }
}