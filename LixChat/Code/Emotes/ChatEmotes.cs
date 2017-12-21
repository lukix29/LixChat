using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LX29_ChatClient.Emotes
{
    public enum EmoteOrigin : int
    {
        Twitch_Global = 0,
        BTTV_Global = 1,
        FFZ_Global = 2,
        Twitch = 3,
        BTTV = 4,
        FFZ = 5,
        Emoji = 6,
        Badge = 7
    }

    public interface IEmoteBase : IEqualityComparer<IEmoteBase>, IDisposable
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

        [JsonIgnore]
        bool IsEmpty
        {
            get;
        }

        [JsonIgnore]
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

        SizeF CalcSize(float height);

        EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height, bool grayOut = false);
    }

    public class Emote : IEmoteBase
    {
        public const int EmoteHeight = 32;
        private static readonly SolidBrush GrayOutBrush = new SolidBrush(Color.FromArgb(200, LX29_ChatClient.UserColors.ChatBackground));

        public Emote(string id, string name, string channel, string emoteSet, EmoteOrigin origin, string channelName)
        {//IEnumerable<string> urls,
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

            Image = new EmoteImage(origin, id, Name);
        }

        [JsonRequired]
        public int _origin
        {
            get { return (int)Origin; }
        }

        public string Channel
        {
            get;
            set;
        }

        public string ChannelName
        {
            get;
            set;
        }

        public string ID
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(Name) || Image == null; }
        }

        [JsonIgnore]
        public bool IsGif
        {
            get { return Image.IsGif; }
        }

        [JsonIgnore]
        public TimeSpan LoadedTime
        {
            get { return DateTime.Now.Subtract(Image.LoadTime); }
        }

        public string Name
        {
            get;
            set;
        }

        [JsonIgnore]
        public EmoteOrigin Origin
        {
            get;
            set;
        }

        public string Set
        {
            get;
            set;
        }

        //[JsonIgnore]
        //public IEnumerable<string> URLs
        //{
        //    get { return Image.URLs.Values; }
        //}

        [JsonIgnore]
        private EmoteImage Image
        {
            get;
            set;
        }

        public SizeF CalcSize(float height)
        {
            return Image.CalcSize(height);
        }

        public void Dispose()
        {
            Image.Dispose();
        }

        //public void DownloadImages()
        //{
        //    Image.DownloadImages();
        //}

        public EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height, bool grayOut = false)
        {
            var res = Image.Draw(g, X, Y, Width, Height);
            if (grayOut)
            {
                g.FillRectangle(GrayOutBrush, X, Y, Width, Height);
                g.DrawLine(Pens.DarkGray, X, Y, X + Width, Y + Height);
            }
            return res;
        }

        public bool Equals(IEmoteBase obj, IEmoteBase obj1)
        {
            return obj.ID.Equals(obj1.ID);
        }

        public new bool Equals(object obj)
        {
            return ((IEmoteBase)obj).ID.Equals(ID);
        }

        public new int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public int GetHashCode(IEmoteBase b)
        {
            return b.GetHashCode();
        }

        public override string ToString()
        {
            return ID + " " + Name + " " + Channel + " " + Set + " " + Enum.GetName(typeof(EmoteOrigin), Origin) + " " + ChannelName;
        }
    }
}