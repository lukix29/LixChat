using System;
using System.Collections.Generic;
using System.Drawing;
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
        FFZ,
        Emoji
    }

    public interface EmoteBase : IEqualityComparer<EmoteBase>
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

        TimeSpan LoadedTime
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

        EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height, EmoteImageSize size, bool grayOut = false);
    }

    public class Emote : EmoteBase
    {
        public const int EmoteHeight = 32;

        private static readonly SolidBrush GrayOutBrush = new SolidBrush(Color.FromArgb(200, LX29_ChatClient.UserColors.ChatBackground));

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

        public bool IsGif
        {
            get { return Image.IsGif; }
        }

        public TimeSpan LoadedTime
        {
            get { return DateTime.Now.Subtract(Image.LoadTime); }
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

        public string Set
        {
            get;
            private set;
        }

        public IEnumerable<string> URLs
        {
            get { return Image.URLs.Values; }
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

        public bool Equals(EmoteBase obj, EmoteBase obj1)
        {
            return obj.ID.Equals(obj1.ID);
        }

        public new bool Equals(object obj)
        {
            return ((EmoteBase)obj).ID.Equals(ID);
        }

        public new int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public int GetHashCode(EmoteBase b)
        {
            return b.GetHashCode();
        }

        public override string ToString()
        {
            return ID + " " + Name + " " + Channel + " " + Set + " " + Enum.GetName(typeof(EmoteOrigin), Origin);
        }
    }
}