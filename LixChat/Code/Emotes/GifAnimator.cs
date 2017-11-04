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
    public enum EmoteImageDrawResult
    {
        Success,
        IsGif,
        Downloading,
    }

    public enum EmoteImageSize : int
    {
        Small = 1,
        Medium = 2,
        Large = 3
    }

    public class EmoteImage : IDisposable
    {
        public static readonly EmoteImage Wait =
            new EmoteImage("WAITING");

        private static readonly string[] BTTV_EMOTE_BASE_URL = new string[] { "https://cdn.betterttv.net/emote/{id}/1x", "https://cdn.betterttv.net/emote/{id}/2x", "https://cdn.betterttv.net/emote/{id}/3x" };
        private static readonly string[] FFZ_EMOTE_BASE_URL = new string[] { "https://cdn.frankerfacez.com/emoticon/{id}/1", "https://cdn.frankerfacez.com/emoticon/{id}/2", "https://cdn.frankerfacez.com/emoticon/{id}/4" };
        private static readonly string[] TWITCH_EMOTE_BASE_URL = new string[] { "https://static-cdn.jtvnw.net/emoticons/v1/{id}/1.0", "https://static-cdn.jtvnw.net/emoticons/v1/{id}/2.0", "https://static-cdn.jtvnw.net/emoticons/v1/{id}/3.0" };
        private readonly object LockObject = new object();

        private Dictionary<EmoteImageSize, Bitmap[]> _images = new Dictionary<EmoteImageSize, Bitmap[]>();
        private Dictionary<EmoteImageSize, Size> _sizes = new Dictionary<EmoteImageSize, Size>();

        private Dictionary<EmoteImageSize, string> _urls = null;
        private bool isDownloading = false;

        private long lasTime = 0;

        public EmoteImage(Dictionary<string, string> urls, string name, EmoteOrigin origin)
        {
            Name = name.Trim();
            this.Origin = origin;
            _urls = urls.Values
                .Select((u, i) => new { url = u, idx = i })
                .ToDictionary(t => (EmoteImageSize)Math.Min(3, t.idx + 1), t0 => t0.url);
            //_urls = urls;
            //EmoteImage(badge.urls, Name)
        }

        public EmoteImage(string name, Image image)
        {
            //FilePaths = new Dictionary<EmoteImageSize, string>();
            //URLs = new Dictionary<EmoteImageSize, string>();
            Name = name;
            _images = new Dictionary<EmoteImageSize, Bitmap[]>();
            Type = image.RawFormat;

            if (!SetGif(image, EmoteImageSize.Small))
            {
                _images.Add(EmoteImageSize.Small, new Bitmap[] { new Bitmap(image).Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppPArgb) });
                _sizes.Add(EmoteImageSize.Small, image.Size);
            }
            LoadTime = DateTime.MaxValue;
        }

        public EmoteImage(string name)
        {
            if (name.Equals("WAITING"))
            {
                //FilePaths = new Dictionary<EmoteImageSize, string>();
                //URLs = new Dictionary<EmoteImageSize, string>();
                Name = name;
                _images = new Dictionary<EmoteImageSize, Bitmap[]>();
                Type = LX29_LixChat.Properties.Resources.loading.RawFormat;
                IsGif = true;
                SetGif(LX29_LixChat.Properties.Resources.loading, EmoteImageSize.Small);
                LoadTime = DateTime.MaxValue;
            }
        }

        public EmoteImage(EmoteOrigin Origin, string ID, string name)
        {
            //Caller = caller;
            Name = name.Trim();
            //this.URLs = new Dictionary<EmoteImageSize, string>();
            //this.FilePaths = new Dictionary<EmoteImageSize, string>();
            if (Origin == EmoteOrigin.FFZ_Global)
            {
            }
            this.Origin = Origin;
            this.ID = ID;

            //var arr = urls.Select((v, i) => new { url = v, idx = i }).ToArray();
            //if (arr.Length > 0)
            //{
            //var urls = arr.ToDictionary(k => k.idx.ToString(), v => (v.url.StartsWith("http") ? v.url : "http://" + v.url));
            //    Image = new EmoteImage(dict, Name);
            //}
            //else
            //{
            //}

            //for (int i = 0; i < urls.Count(); i++)
            //{
            //    //int i = 1;
            //    //if (int.TryParse(url.Key, out i))
            //    //{
            //    //}
            //    i = Math.Max(1, Math.Min(3, i));
            //    var emimgs = (EmoteImageSize)i;

            //    if (!this.URLs.ContainsKey(emimgs))
            //    {
            //        //string uri = urls[i];
            //        //this.URLs.Add(emimgs, uri);
            //        this.FilePaths.Add(emimgs, Path.GetFullPath(GetLocalfileName(uri)));
            //    }
            //}
            LoadTime = DateTime.MaxValue;
        }

        //private object locko = new object();
        public Bitmap Bitmap
        {
            get
            {
                var img = GetImage(EmoteImageSize.Large);
                if (img != null)
                    return img.First();
                else return new Bitmap(1, 1);
            }
        }

        public int Delay
        {
            get;
            private set;
        }

        public Dictionary<EmoteImageSize, string> FilePaths
        {
            get
            {
                return URLs.ToDictionary(t => t.Key, t0 => Path.GetFullPath(GetLocalfileName(t0.Value)));
            }
        }

        public int FrameCount
        {
            get;
            private set;
        }

        public int FrameIndex
        {
            get;
            private set;
        }

        public string ID
        {
            get;
            private set;
        }

        public bool IsGif
        {
            get;
            private set;
        }

        public DateTime LoadTime
        {
            get;
            private set;
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

        public ImageFormat Type
        {
            get;
            private set;
        }

        public Dictionary<EmoteImageSize, string> URLs
        {
            get
            {
                if (Origin == EmoteOrigin.BTTV || Origin == EmoteOrigin.BTTV_Global)
                {
                    return BTTV_EMOTE_BASE_URL
                        .Select((u, i) => new { url = u.Replace("{id}", ID), idx = i })
                        .ToDictionary(t => (EmoteImageSize)Math.Min(3, t.idx + 1), t0 => t0.url);
                }
                else if (Origin == EmoteOrigin.FFZ || Origin == EmoteOrigin.FFZ_Global)
                {
                    return FFZ_EMOTE_BASE_URL
                        .Select((u, i) => new { url = u.Replace("{id}", ID), idx = i })
                        .ToDictionary(t => (EmoteImageSize)Math.Min(3, t.idx + 1), t0 => t0.url);
                }
                else if (Origin == EmoteOrigin.Twitch || Origin == EmoteOrigin.Twitch_Global)
                {
                    return TWITCH_EMOTE_BASE_URL
                        .Select((u, i) => new { url = u.Replace("{id}", ID), idx = i })
                        .ToDictionary(t => (EmoteImageSize)Math.Min(3, t.idx + 1), t0 => t0.url);
                }
                else if (Origin == EmoteOrigin.Badge)
                {
                    return _urls;
                }
                return new Dictionary<EmoteImageSize, string>();
            }
        }

        public static string GetLocalfileName(string imageUri)
        {
            string retVal = imageUri.Trim();

            string ext = Path.GetExtension(imageUri);
            if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) &&
               ext.Equals(".gif", StringComparison.OrdinalIgnoreCase))
            {
                retVal = retVal.Replace(ext, "");
            }

            List<string> inv = Path.GetInvalidFileNameChars().Select<char, string>(c => c.ToString()).ToList();
            inv.AddRange(new string[] { "https", "http", ".", "-" });
            foreach (var c in inv)
            {
                if (!string.IsNullOrEmpty(c))
                {
                    retVal = retVal.Replace(c, "");
                }
            }

            if (retVal.Length > 64)
            {
                retVal = retVal.Remove(0, Math.Max(0, retVal.Length - 64));
            }

            return retVal;
        }

        public static Image ResizeBitmap(Image input, float width, float height)
        {
            return ResizeBitmap(input, new SizeF(width, height));
        }

        public static Image ResizeBitmap(Image input, SizeF newSize)
        {
            Bitmap b = new Bitmap((int)newSize.Width, (int)newSize.Height, input.PixelFormat);
            Graphics g = Graphics.FromImage(b);
            float x = (newSize.Width - input.Width) / 2f;
            float y = (newSize.Height - input.Height) / 2f;
            g.DrawImage(input, x, y, input.Width, input.Height);
            g.Dispose();
            //b.Save((cnt++) + ".png", ImageFormat.Png);
            return b;
        }

        public SizeF CalcSize(float height, EmoteImageSize size)
        {
            try
            {
                if (_sizes.Count == 0)
                    return Size.Empty;

                SizeF emS = Size.Empty;

                if (_images.ContainsKey(size))
                {
                    emS = _sizes[size];
                }
                else
                {
                    emS = _sizes.Last().Value;
                }

                float ratio = height / emS.Height;
                float newWidth = (emS.Width * ratio);
                //float newHeight = (emS.Height * ratio);
                return new SizeF(newWidth, height);
            }
            catch { }
            return SizeF.Empty;
        }

        public void Dispose()
        {
            try
            {
                foreach (var size in _images)
                {
                    foreach (var image in size.Value)
                    {
                        image.Dispose();
                    }
                }
            }
            catch { }
            finally
            {
                lock (_images)
                {
                    _sizes.Clear();
                    _images.Clear();
                }
            }
        }

        public void DownloadImages()
        {
            foreach (var size in URLs)
            {
                DownloadImage(size.Key);
            }
        }

        public EmoteImageDrawResult Draw(Graphics g, Rectangle rec, EmoteImageSize size)
        {
            return Draw(g, rec.X, rec.Y, rec.Width, rec.Height, size);
        }

        public EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height, EmoteImageSize size)
        {
            var result = EmoteImageDrawResult.Success;
            var images = GetImage(size);
            try
            {
                if (!Name.Equals("WAITING") && images == null)
                {
                    if (!isDownloading)
                    {
                        isDownloading = true;
                        Task.Run(() => DownloadImage(size));
                    }
                    Wait.Draw(g, X, Y, Width, Height, size);
                    result = EmoteImageDrawResult.Downloading;
                }
                else
                {
                    if (IsGif && Settings.AnimatedEmotes)
                    {
                        long ms = (long)((DateTime.UtcNow.Ticks - lasTime) / TimeSpan.TicksPerMillisecond);
                        if (ms > Delay)
                        {
                            if (FrameIndex + 1 >= FrameCount)
                            {
                                FrameIndex = 0;
                            }
                            else FrameIndex++;
                            lasTime = DateTime.UtcNow.Ticks;
                        }
                        result = EmoteImageDrawResult.IsGif;
                    }
                    else
                    {
                        FrameIndex = 0;
                    }
                    //Monitor.Enter(images.SyncRoot);
                    lock (images[FrameIndex])
                    {
                        g.DrawBitmap(images[FrameIndex], X, Y, Width, Height, Settings.HwEmoteDrawing);
                    }
                    //Monitor.Exit(images.SyncRoot);
                    LoadTime = DateTime.Now;
                }
            }
            catch
            {
            }
            //finally
            //{
            //    //Monitor.Exit(images.SyncRoot);
            //}
            return result;
        }

        public Bitmap[] GetImage(EmoteImageSize size)
        {
            //lock (LockObject)
            //{
            if (_images == null)
                return null;
            if (_images.Count == 0)
                return null;

            if (_images.ContainsKey(size))
            {
                return _images[size];
            }
            var max = (EmoteImageSize)_images.Keys.Max(t => (int)t);
            return _images[max];
            //}
        }

        protected virtual void Dispose(bool disposing)
        {
            Dispose();
        }

        private void DownloadImage(EmoteImageSize size)
        {
            if (_images.ContainsKey(size)) return;

            var url = "";
            try
            {
                //float height = Emote.EmoteHeight;
                if (!URLs.ContainsKey(size))
                {
                    url = URLs.Last().Value;
                }
                else
                {
                    url = URLs[size];
                }

                string FilePath = Path.GetFullPath(Settings._emoteDir + GetLocalfileName(url));
                //  Image img = null;
                lock (LockObject)
                {
                    if (!File.Exists(FilePath))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.Proxy = null;
                            using (var ms = wc.OpenRead(new Uri(url)))
                            {
                                using (Image img0 = Image.FromStream(ms))
                                {
                                    SetImageOrGif(img0, size);

                                    img0.Save(FilePath);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (Image img0 = Image.FromFile(FilePath))
                        {
                            SetImageOrGif(img0, size);
                        }
                    }
                }
                LoadTime = DateTime.Now;
            }
            catch
            {
                _images = new Dictionary<EmoteImageSize, Bitmap[]>();
            }
            isDownloading = false;
        }

        private bool SetGif(Image img, EmoteImageSize size, int cnt = 0)
        {
            try
            {
                if (!ImageFormat.Gif.Equals(this.Type)) return false;
                if (cnt > 10) return false;
                IsGif = true;
                FrameDimension dimension = new FrameDimension(img.FrameDimensionsList[0]);
                FrameCount = img.GetFrameCount(dimension);
                Bitmap[] list = new Bitmap[FrameCount];
                int[] delays = new int[FrameCount];
                for (int i = 0; i < FrameCount; i++)
                {
                    img.SelectActiveFrame(dimension, i);
                    list[i] = new Bitmap(img).Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppPArgb);
                }

                var val = img.PropertyItems.FirstOrDefault(t => t.Id == 0x5100);
                if (val != null)
                {
                    byte[] ba = val.Value;
                    // Time is in 1/10 of a millisecond
                    Delay = Math.Min(100, Math.Max(10, BitConverter.ToInt32(ba, 0) * 10));
                }
                else
                {
                    Delay = 30;
                }
                //if (!string.IsNullOrEmpty(Name) && Name.Equals("(ditto)"))
                //{
                //    Delay = (int)(Delay * 3.5f);
                //}

                _images.Add(size, list.ToArray());
                _sizes.Add(size, list[0].Size);
                return true;
            }
            catch (Exception x)
            {
                if (x.Message.Equals("Allgemeiner Fehler in GDI+.", StringComparison.OrdinalIgnoreCase))
                {
                    SetGif(img, size, cnt + 1);
                }
            }
            return false;
        }

        private void SetImageOrGif(Image img, EmoteImageSize size)
        {
            this.Type = img.RawFormat;

            SizeF emS = img.Size;
            if (!SetGif(img, size))
            {
                if (EmoteCollection.StandardEmotes.Any(t => Name.Contains(t.Value)))
                {
                    img = ResizeBitmap(img, emS.Width * 1f, emS.Height * 1.5f);
                }
                if (!_images.ContainsKey(size))
                    _images.Add(size,
                        new Bitmap[] { new Bitmap(img).Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppPArgb) });
                if (!_sizes.ContainsKey(size))
                    _sizes.Add(size, img.Size);
            }
        }
    }
}