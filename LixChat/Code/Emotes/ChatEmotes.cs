using System;
using System.Collections.Generic;
using System.Linq;

namespace LX29_ChatClient.Emotes
{
    public enum EmoteOrigin
    {
        Twitch_Global,
        BTTV_Global,
        FFZ_Global,
        Twitch,
        BTTV,
        FFZ
    }

    public class Emote : IEquatable<Emote>
    {
        public const int EmoteHeight = 32;
        public static readonly Emote Empty = new Emote();

        //private readonly string hashcode = "";

        public Emote()
        {
            Name = "";
            Channel = "";
            ChannelName = "";
        }

        public Emote(string id, string name, IEnumerable<string> urls, string channel, string emoteSet, EmoteOrigin origin, string channelName)
        {
            Name = name.Trim();
            Channel = channel.Trim();
            ChannelName = channelName;
            Set = emoteSet;
            Origin = origin;
            ID = id;
            if (EmoteCollection.StandardEmotes.ContainsKey(id))
            {
                Name = EmoteCollection.StandardEmotes[id];
            }
            //hashcode = id + Name.HashCode();

            var arr = urls.Select((v, i) => new { url = v, idx = i }).ToArray();
            if (arr.Length > 0)
            {
                var dict = arr.ToDictionary(k => k.idx.ToString(), v => (v.url.StartsWith("http") ? v.url : "http://" + v.url));
                Image = new EmoteImage(dict, Name);
            }
            else
            {
            }
        }

        public string Channel
        {
            get;
            private set;
        }

        public string ChannelName
        {
            get;
            private set;
        }

        public string ID
        {
            get;
            private set;
        }

        public EmoteImage Image
        {
            get;
            private set;
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(Name) || Image == null; }
        }

        public string Name
        {
            get;
            private set;
        }

        public EmoteOrigin Origin
        {
            get;
            private set;
        }

        public string Set
        {
            get;
            private set;
        }

        //public static Emote Parse(string input)
        //{
        //    string[] sarr = input.Split(" ");
        //    string id = sarr[0].Trim();
        //    string name = sarr[1].Trim();
        //    string channel = sarr[2].Trim();
        //    string url = sarr[3].Trim();
        //    string set = sarr[4].Trim();
        //    EmoteOrigin origin = (EmoteOrigin)Enum.Parse(typeof(EmoteOrigin), sarr[4].Trim());

        //    return new Emote(id, name, url, channel, set, origin);
        //}

        public bool Equals(Emote obj)
        {
            return ID.Equals(obj.ID);
        }

        //public bool Equals(ChatWord obj)
        //{
        //    return ID.Equals(obj.Emote_ID);
        //}

        public override string ToString()
        {
            return ID + " " + Name + " " + Channel + " " + Set + " " + Enum.GetName(typeof(EmoteOrigin), Origin);
        }
    }
}