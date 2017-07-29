using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace LX29_ChatClient.Emotes
{
    public enum EmoteOrigin
    {
        Twitch_Global,
        BTTV_Global,
        FFZ_Global,
        Twitch,
        BTTV,
        FFZ,
        Emoji
    }

    public interface EmoteBase : IEquatable<EmoteBase>
    {
        string Channel
        {
            get;
            set;
        }

        string ID
        {
            get;
            set;
        }

        // public static readonly EmoteBase Empty = new EmoteBase();
        bool IsEmpty
        {
            get;
        }

        string Name
        {
            get;
            set;
        }

        EmoteOrigin Origin
        {
            get;
            set;
        }

        SizeF CalcSize(float height, EmoteImageSize size);

        void Dispose();

        void DownloadImages();

        //{
        //    return SizeF.Empty;
        //}

        EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height, EmoteImageSize size, bool grayOut = false);

        //{
        //    if (grayOut)
        //    {
        //        g.FillRectangle(GrayOutBrush, X, Y, Width, Height);
        //        g.DrawLine(Pens.DarkGray, X, Y, X + Width, Y + Height);
        //    }
        //    return EmoteImageDrawResult.Success;
        //}

        //{
        //    return ID.Equals(obj.ID);
        //}

        //public override string ToString()
        //{
        //    return ID + " " + Name + " " + Channel + " " + Set + " " + Enum.GetName(typeof(EmoteOrigin), Origin);
        //}
    }

    public class Emote : EmoteBase
    {
        public const int EmoteHeight = 32;
        private static readonly SolidBrush GrayOutBrush = new SolidBrush(Color.FromArgb(200, LX29_ChatClient.UserColors.ChatBackground));

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
            set;
        }

        public string ChannelName
        {
            get;
            private set;
        }

        public string ID
        {
            get;
            set;
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(Name) || Image == null; }
        }

        //public string ID
        //{
        //    get;
        //    private set;
        //}
        public bool IsGif
        {
            get { return Image.IsGif; }
        }

        public string Name
        {
            get;
            set;
        }

        public EmoteOrigin Origin
        {
            get;
            set;
        }

        //public string Name
        //{
        //    get;
        //    private set;
        //}
        public string Set
        {
            get;
            private set;
        }

        private EmoteImage Image
        {
            get;
            set;
        }

        public SizeF CalcSize(float height, EmoteImageSize size)
        {
            return Image.CalcSize(height, size);
        }

        public void Dispose()
        {
            Image.Dispose();
        }

        public void DownloadImages()
        {
            Image.DownloadImages();
        }

        public EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height, EmoteImageSize size, bool grayOut = false)
        {
            var res = Image.Draw(g, X, Y, Width, Height, size);
            if (grayOut)
            {
                g.FillRectangle(GrayOutBrush, X, Y, Width, Height);
                g.DrawLine(Pens.DarkGray, X, Y, X + Width, Y + Height);
            }
            return res;
        }

        //private readonly string hashcode = "";
        public bool Equals(EmoteBase e)
        {
            return e.ID.Equals(ID);
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

        //public bool Equals(Emote obj)
        //{
        //    return ID.Equals(obj.ID);
        //}

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