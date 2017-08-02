using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LX29_ChatClient.Emotes
{
    public class Emoji : EmoteBase
    {
        public static readonly Emoji Empty = new Emoji("", "");
        private static readonly SolidBrush GrayOutBrush = new SolidBrush(Color.FromArgb(200, LX29_ChatClient.UserColors.ChatBackground));
        private Image _image = null;

        private DateTime loadTime = DateTime.MaxValue;
        private object LockObject = new object();

        public Emoji(string ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }

        /// <summary>
        /// Always Empty
        /// </summary>
        public string Channel
        {
            get { return string.Empty; }
            set
            {
            }
        }

        public string FilePath
        {
            get { return Settings.emojiDir + ID + ".png"; }
        }

        public string ID
        {
            get;
            set;
        }

        public Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = (Image)Image.FromFile(FilePath).Clone();
                    Size = _image.Size;
                }
                loadTime = DateTime.Now;
                return _image;
            }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(ID); }
        }

        public TimeSpan LoadedTime
        {
            get { return DateTime.Now.Subtract(loadTime); }
        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Always "EmoteOrigin.Emoji".
        /// </summary>
        public EmoteOrigin Origin
        {
            get { return EmoteOrigin.Emoji; }
            set
            {
            }
        }

        public Size Size
        {
            get;
            private set;
        }

        public SizeF CalcSize(float height, EmoteImageSize size)
        {
            float ratio = height / (float)Size.Height;
            float newWidth = (Size.Width * ratio);
            return new SizeF(newWidth, height);
        }

        public void Dispose()
        {
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }
        }

        /// <summary>
        /// Empty.
        /// </summary>
        public void DownloadImages()
        {
        }

        public EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height, EmoteImageSize size, bool grayOut = false)
        {
            lock (LockObject)
            {
                g.DrawImage(Image, X, Y, Width, Height);
            }
            if (grayOut)
            {
                g.FillRectangle(GrayOutBrush, X, Y, Width, Height);
                g.DrawLine(Pens.DarkGray, X, Y, X + Width, Y + Height);
            }
            return EmoteImageDrawResult.Success;
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
    }
}