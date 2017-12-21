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

    //public enum EmoteImageSize : int
    //{
    //    Small = 1,
    //    Medium = 2,
    //    Large = 3
    //}

    public class EmoteImage : IDisposable
    {
        public static readonly EmoteImage Wait =
            new EmoteImage("WAITING");

        private static readonly string BTTV_EMOTE_BASE_URL =// new string[] {
            "https://cdn.betterttv.net/emote/{id}/3x";//, "https://cdn.betterttv.net/emote/{id}/2x", "https://cdn.betterttv.net/emote/{id}/3x" };

        private static readonly string FFZ_EMOTE_BASE_URL =// new string[] {
            "https://cdn.frankerfacez.com/emoticon/{id}/1";//, "https://cdn.frankerfacez.com/emoticon/{id}/2", "https://cdn.frankerfacez.com/emoticon/{id}/4" };

        private static readonly string TWITCH_EMOTE_BASE_URL =// new string[] {
            "https://static-cdn.jtvnw.net/emoticons/v1/{id}/3.0";//, "https://static-cdn.jtvnw.net/emoticons/v1/{id}/2.0", "https://static-cdn.jtvnw.net/emoticons/v1/{id}/3.0" };

        //private readonly object LockObject = new object();

        private readonly string Url = null;
        private Bitmap[] _images = null;
        private Size _sizes = new Size();
        private System.Diagnostics.Stopwatch gifWatch = new System.Diagnostics.Stopwatch();
        private bool isDownloading = false;

        public EmoteImage(string url, string name, EmoteOrigin origin)
        {
            Name = name.Trim();
            this.Origin = origin;
            Url = url;//.Values.Last();
            //.Select((u, i) => new { url = u, idx = i })
            //.ToDictionary(t => (EmoteImageSize)Math.Min(3, t.idx + 1), t0 => t0.url);
            //_urls = urls;
            //EmoteImage(badge.urls, Name)
        }

        public EmoteImage(string name, Bitmap image)
        {
            //FilePaths = new Dictionary<EmoteImageSize, string>();
            //URLs = new Dictionary<EmoteImageSize, string>();
            Name = name;
            Type = image.RawFormat;

            SetImage(image);
            //if (!SetGif(image, EmoteImageSize.Small))
            //{
            //    _images.Add(EmoteImageSize.Small, new Bitmap(image).Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppPArgb));
            //    _sizes.Add(EmoteImageSize.Small, image.Size);
            //}
            LoadTime = DateTime.MaxValue;
        }

        /// <summary>
        /// Only for Waiting Gif
        /// </summary>
        /// <param name="name"></param>
        public EmoteImage(string name)
        {
            if (name.Equals("WAITING"))
            {
                //FilePaths = new Dictionary<EmoteImageSize, string>();
                //URLs = new Dictionary<EmoteImageSize, string>();
                Name = name;
                Type = LX29_LixChat.Properties.Resources.loading.RawFormat;
                IsGif = true;
                SetImage(LX29_LixChat.Properties.Resources.loading);
                LoadTime = DateTime.MaxValue;
            }
        }

        public EmoteImage(EmoteOrigin Origin, string ID, string name)
        {
            //Caller = caller;
            Name = name.Trim();
            //this.URLs = new Dictionary<EmoteImageSize, string>();
            //this.FilePaths = new Dictionary<EmoteImageSize, string>();

            this.Origin = Origin;
            this.ID = ID;

            if (Origin == EmoteOrigin.BTTV || Origin == EmoteOrigin.BTTV_Global)
            {
                Url = BTTV_EMOTE_BASE_URL.Replace("{id}", ID);
            }
            else if (Origin == EmoteOrigin.FFZ || Origin == EmoteOrigin.FFZ_Global)
            {
                Url = FFZ_EMOTE_BASE_URL.Replace("{id}", ID);
            }
            else if (Origin == EmoteOrigin.Twitch || Origin == EmoteOrigin.Twitch_Global)
            {
                Url = TWITCH_EMOTE_BASE_URL.Replace("{id}", ID);
            }

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
        //public Bitmap Bitmap
        //{
        //    get
        //    {
        //        var img = GetImage(EmoteImageSize.Large);
        //        if (img != null)
        //            return img;
        //        else return new Bitmap(1, 1);
        //    }
        //}

        public int Delay
        {
            get;
            private set;
        }

        public string FilePaths
        {
            get
            {
                return Path.GetFullPath(GetLocalfileName(Url));
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

        public bool IsLoaded
        {
            get { return !_sizes.IsEmpty; }
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

        //public string URLs
        //{
        //    get
        //    {
        //        if (Origin == EmoteOrigin.BTTV || Origin == EmoteOrigin.BTTV_Global)
        //        {
        //            return BTTV_EMOTE_BASE_URL
        //                .Select((u, i) => new { url = u.Replace("{id}", ID), idx = i })
        //                .ToDictionary(t => (EmoteImageSize)Math.Min(3, t.idx + 1), t0 => t0.url);
        //        }
        //        else if (Origin == EmoteOrigin.FFZ || Origin == EmoteOrigin.FFZ_Global)
        //        {
        //            return FFZ_EMOTE_BASE_URL
        //                .Select((u, i) => new { url = u.Replace("{id}", ID), idx = i })
        //                .ToDictionary(t => (EmoteImageSize)Math.Min(3, t.idx + 1), t0 => t0.url);
        //        }
        //        else if (Origin == EmoteOrigin.Twitch || Origin == EmoteOrigin.Twitch_Global)
        //        {
        //            return TWITCH_EMOTE_BASE_URL
        //                .Select((u, i) => new { url = u.Replace("{id}", ID), idx = i })
        //                .ToDictionary(t => (EmoteImageSize)Math.Min(3, t.idx + 1), t0 => t0.url);
        //        }
        //        else if (Origin == EmoteOrigin.Badge)
        //        {
        //            return _urls;
        //        }
        //        return "";
        //    }
        //}

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

        public static Bitmap ResizeBitmap(Image input, float width, float height)
        {
            return ResizeBitmap(input, new SizeF(width, height));
        }

        public static Bitmap ResizeBitmap(Image input, SizeF newSize)
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

        public SizeF CalcSize(float height)
        {
            try
            {
                SizeF emS = _sizes;

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
                gifWatch.Stop();
                foreach (var image in _images)
                {
                    image.Dispose();
                }
            }
            catch { }
            finally
            {
                lock (_images)
                {
                    _images = null;
                }
            }
        }

        //public void DownloadImages()
        //{
        //    foreach (var size in URLs)
        //    {
        //        DownloadImage(size.Key);
        //    }
        //}

        public EmoteImageDrawResult Draw(Graphics g, Rectangle rec)
        {
            return Draw(g, rec.X, rec.Y, rec.Width, rec.Height);
        }

        public EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height)
        {
            var result = EmoteImageDrawResult.Success;

            try
            {
                if (!Name.Equals("WAITING") && _images == null)
                {
                    if (!isDownloading)
                    {
                        isDownloading = true;
                        Task.Run(() => DownloadImage());
                    }
                    Wait.Draw(g, X, Y, Width, Height);
                    result = EmoteImageDrawResult.Downloading;
                }
                else if (_images != null)
                {
                    if (IsGif && Settings.AnimatedEmotes)
                    {
                        long ms = gifWatch.ElapsedMilliseconds;// (long)((DateTime.UtcNow.Ticks - lasTime) / TimeSpan.TicksPerMillisecond);
                        if (ms > Delay)
                        {
                            // ImageAnimator.UpdateFrames(image);
                            //activeFrame =
                            if (FrameIndex + 1 >= FrameCount)
                            {
                                FrameIndex = 0;
                            }
                            else FrameIndex++;

                            // lasTime = DateTime.UtcNow.Ticks;
                        }
                        result = EmoteImageDrawResult.IsGif;
                    }
                    else
                    {
                        FrameIndex = 0;
                    }
                    //Monitor.Enter(images.SyncRoot);
                    lock (_images[FrameIndex])
                    {
                        g.DrawBitmap(_images[FrameIndex], X, Y, Width, Height, Settings.HwEmoteDrawing);
                    }
                    //Monitor.Exit(images.SyncRoot);
                    LoadTime = DateTime.Now;
                }
            }
            catch
            {
                g.DrawRectangle(Pens.Red, X, Y, Width, Height);
                g.DrawLine(Pens.Red, X, Y, X + Width, Y + Height);
                g.DrawLine(Pens.Red, 0, Y + Height, X + Width, 0);
                //Dispose();
            }
            finally
            {
                if (IsGif)
                {
                    gifWatch.Restart();
                }
                //Monitor.Exit(images.SyncRoot);
            }
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            Dispose();
        }

        private void DownloadImage()
        {
            if (_images != null) return;

            try
            {
                //float height = Emote.EmoteHeight;
                //if (!URLs.ContainsKey(size))
                //{
                //    url = URLs.Last().Value;
                //}
                //else
                //{
                //    url = URLs[size];
                //}

                string FilePath = Path.GetFullPath(Settings._emoteDir + GetLocalfileName(Url));
                //  Image img = null;
                //lock (LockObject)
                {
                    if (Origin == EmoteOrigin.FFZ || Origin == EmoteOrigin.FFZ_Global)
                    {
                    }
                    if (!File.Exists(FilePath))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.Proxy = null;
                            using (var ms = wc.OpenRead(new Uri(Url)))
                            {
                                Bitmap img0 = new Bitmap(ms);

                                SetImage(img0);

                                img0.Save(FilePath);
                            }
                        }
                    }
                    else
                    {
                        Bitmap img0 = new Bitmap(FilePath);

                        SetImage(img0);
                    }
                }
                LoadTime = DateTime.Now;
            }
            catch
            {
                _images = null;
            }
            isDownloading = false;
        }

        private bool Set_Gif(Bitmap img, int cnt = 0)
        {
            try
            {
                if (!ImageFormat.Gif.Equals(this.Type))
                {
                    IsGif = false;
                    return false;
                }
                else
                {
                    //ImageAnimator.Animate(img, OnFrameChanged);
                    IsGif = true;
                    gifWatch.Start();

                    //_images.Add(size, (Bitmap)img);
                    _sizes = img.Size;

                    //Get Frame Delay
                    var val = img.PropertyItems.FirstOrDefault(t => t.Id == 0x5100);

                    FrameIndex = 0;
                    FrameCount = img.GetFrameCount(FrameDimension.Time);

                    for (int i = 0; i < FrameCount; i++)
                    {
                        img.SelectActiveFrame(FrameDimension.Time, i);
                        _images[i] = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppPArgb);
                    }

                    if (val != null)
                    {
                        byte[] ba = val.Value;
                        //Time is in 1 / 10 of a millisecond
                        Delay = Math.Min(100, Math.Max(10, BitConverter.ToInt32(ba, 0) * 10));
                    }
                    else
                    {
                        Delay = 30;
                    }
                    if (!string.IsNullOrEmpty(Name) && Name.Equals("(ditto)"))
                    {
                        Delay = (int)(Delay * 3.5f);
                    }
                }

                #region old Code

                //if (!ImageFormat.Gif.Equals(this.Type)) return false;
                //if (cnt > 10) return false;
                //IsGif = true;
                //FrameDimension dimension = new FrameDimension(img.FrameDimensionsList[0]);
                //FrameCount = img.GetFrameCount(dimension);
                //Bitmap[] list = new Bitmap[FrameCount];
                //int[] delays = new int[FrameCount];
                //for (int i = 0; i < FrameCount; i++)
                //{
                //    img.SelectActiveFrame(dimension, i);
                //    list[i] = new Bitmap(img).Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppPArgb);
                //}

                //_images.Add(size, list.ToArray());
                //_sizes.Add(size, list[0].Size);

                #endregion old Code
            }
            catch (Exception x)
            {
                if (x.Message.Equals("Allgemeiner Fehler in GDI+.", StringComparison.OrdinalIgnoreCase))
                {
                    return Set_Gif(img, cnt + 1);
                }
                FrameCount = 1;
            }
            return IsGif;
        }

        private void SetImage(Bitmap img)
        {
            this.Type = img.RawFormat;

            SizeF emS = img.Size;
            if (!Set_Gif(img))
            {
                if (EmoteCollection.StandardEmotes.Any(t => Name.Contains(t.Value)))
                {
                    img = ResizeBitmap(img, emS.Width * 1f, emS.Height * 1.5f);
                }

                _images = new Bitmap[] {
                       img.Clone(new Rectangle(0, 0, img.Width, img.Height),
                        PixelFormat.Format32bppPArgb)};

                _sizes = img.Size;
            }
        }
    }
}