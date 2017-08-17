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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace LX29_ChatClient.Emotes
{
    public class Emoteionary
    {
        //public Dictionary<string, Emoji> _emoji_names = new Dictionary<string, Emoji>();
        public Dictionary<int, Emoji> _emoji_unicodes = new Dictionary<int, Emoji>();

        private Dictionary<string, EmoteBase> _ffzbttv = new Dictionary<string, EmoteBase>();

        private Dictionary<string, EmoteBase> _twitch = new Dictionary<string, EmoteBase>();

        public Emoteionary()
        {
            _UserEmotes = new Dictionary<string, EmoteBase>();
            _ffzbttv = new Dictionary<string, EmoteBase>();
            _twitch = new Dictionary<string, EmoteBase>();
            _emoji_unicodes = new Dictionary<int, Emoji>();
        }

        public Dictionary<string, EmoteBase> _UserEmotes
        {
            get;
            set;
        }

        public IEnumerable<EmoteBase> All
        {
            get { return _twitch.Values.Concat(_ffzbttv.Values); }
        }

        public int Count
        {
            get { return _ffzbttv.Count + _twitch.Count + _emoji_unicodes.Count; }
        }

        public Dictionary<string, EmoteBase> Twitch
        {
            get { return _twitch; }
        }

        public EmoteBase this[string ID, string Name]
        {
            get
            {
                if (!ID.IsEmpty() && _twitch.ContainsKey(ID))
                {
                    return _twitch[ID];
                }
                if (!Name.IsEmpty() && _ffzbttv.ContainsKey(Name))
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
            foreach (var emoji in _emoji_unicodes.Values)
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
                return _emoji_unicodes.Values.Where(t => t.Name.StartsWith(name)).Select(t => (EmoteBase)t).ToList();
            }
            else
            {
                var em = _twitch.Values.Where((e) =>
                   {
                       if (e.Name.ToLower().StartsWith(name))
                       {
                           return _UserEmotes.Values.Any(t => t.ID.Equals(e.ID));
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
            var ems = _UserEmotes.Values;
            //_twitch.Values
            //    .Where(t => _UserEmotes.Any(t0 => t.ID.Equals(t0.id))).ToList();

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
            _emoji_unicodes = new Dictionary<int, Emoji>();
            using (StringReader sr = new StringReader(LX29_LixChat.Properties.Resources.emojis))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    var arr = line.Split('=');
                    var name = arr[1];
                    var id = arr[0];
                    if (!id.IsEmpty())
                    {
                        Emoji emo = new Emoji(id, name);
                        var arr0 = id.Split("-");
                        foreach (var vak in arr0)
                        {
                            //int val = int.Parse(ID, NumberStyles.HexNumber);
                            var uni = int.Parse(vak, NumberStyles.HexNumber);
                            if (!_emoji_unicodes.ContainsKey(uni))
                            {
                                _emoji_unicodes.Add(uni, emo);
                            }
                        }
                    }
                }
            }
            return _emoji_unicodes.Count;
        }
    }
}