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

        private readonly object LockObject = new object();

        private Dictionary<EmoteImageSize, Image[]> _images = new Dictionary<EmoteImageSize, Image[]>();
        private Dictionary<EmoteImageSize, Size> _sizes = new Dictionary<EmoteImageSize, Size>();

        private bool isDownloading = false;

        private long lasTime = 0;

        private object locko = new object();

        public EmoteImage(string name, Image image)
        {
            FilePaths = new Dictionary<EmoteImageSize, string>();
            URLs = new Dictionary<EmoteImageSize, string>();
            Name = name;
            _images = new Dictionary<EmoteImageSize, Image[]>();
            Type = image.RawFormat;

            if (!SetGif(image, EmoteImageSize.Small))
            {
                _images.Add(EmoteImageSize.Small, new Image[] { (Image)image.Clone() });
            }
            LoadTime = DateTime.MaxValue;
        }

        public EmoteImage(string name)
        {
            if (name.Equals("WAITING"))
            {
                FilePaths = new Dictionary<EmoteImageSize, string>();
                URLs = new Dictionary<EmoteImageSize, string>();
                Name = name;
                _images = new Dictionary<EmoteImageSize, Image[]>();
                Type = LX29_LixChat.Properties.Resources.loading.RawFormat;
                IsGif = true;
                SetGif(LX29_LixChat.Properties.Resources.loading, EmoteImageSize.Small);
                LoadTime = DateTime.MaxValue;
            }
        }

        public EmoteImage(Dictionary<string, string> urls, string name)
        {
            //Caller = caller;
            Name = name.Trim();
            this.URLs = new Dictionary<EmoteImageSize, string>();
            this.FilePaths = new Dictionary<EmoteImageSize, string>();

            foreach (var url in urls)
            {
                int i = 1;
                if (int.TryParse(url.Key, out i))
                {
                }
                i = Math.Max(1, Math.Min(3, i));
                var emimgs = (EmoteImageSize)i;

                if (!this.URLs.ContainsKey(emimgs))
                {
                    string uri = url.Value;
                    if (!uri.StartsWith("http"))
                    {
                        uri = "http:" + uri;
                    }
                    this.URLs.Add(emimgs, uri);
                    this.FilePaths.Add(emimgs, Path.GetFullPath(GetLocalfileName(uri)));
                }
            }
            LoadTime = DateTime.MaxValue;
        }

        public int Delay
        {
            get;
            private set;
        }

        public Dictionary<EmoteImageSize, string> FilePaths
        {
            get;
            private set;
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

        public ImageFormat Type
        {
            get;
            private set;
        }

        public Dictionary<EmoteImageSize, string> URLs
        {
            get;
            set;
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
                _images.Clear();
            }
            catch { }
        }

        public void DownloadImages()
        {
            foreach (var size in URLs)
            {
                DownloadImage(size.Key);
            }
        }

        public EmoteImageDrawResult Draw(Graphics g, float X, float Y, float Width, float Height, EmoteImageSize size)
        {
            var result = EmoteImageDrawResult.Success;
            try
            {
                var images = GetImage(size);
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
                    lock (LockObject)
                    {
                        g.DrawImage(images[FrameIndex], X, Y, Width, Height);
                    }
                    LoadTime = DateTime.Now;
                    //g.DrawImage(images[FrameIndex], X, Y, Width, Height);
                }
            }
            catch
            {
            }
            return result;
        }

        public Image[] GetImage(EmoteImageSize size)
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

                string FilePath = Path.GetFullPath(Settings.emoteDir + GetLocalfileName(url));
                //  Image img = null;
                lock (locko)
                {
                    if (!File.Exists(FilePath))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.Proxy = null;
                            using (MemoryStream ms =
                                new MemoryStream(wc.DownloadData(new Uri(url))))
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
                //images = null;
            }
            isDownloading = false;
        }

        //private Image[] getFrames(Image originalImg)
        //{
        //    int numberOfFrames = originalImg.GetFrameCount(FrameDimension.Time);
        //    Image[] frames = new Image[numberOfFrames];

        //    for (int i = 0; i < numberOfFrames; i++)
        //    {
        //        originalImg.SelectActiveFrame(FrameDimension.Time, i);
        //        frames[i] = ((Image)originalImg.Clone());
        //    }

        //    return frames;
        //}

        private bool SetGif(Image img, EmoteImageSize size, int cnt = 0)
        {
            try
            {
                if (!ImageFormat.Gif.Equals(this.Type)) return false;
                if (cnt > 10) return false;
                IsGif = true;
                FrameDimension dimension = new FrameDimension(img.FrameDimensionsList[0]);
                FrameCount = img.GetFrameCount(dimension);
                Image[] list = new Image[FrameCount];
                int[] delays = new int[FrameCount];
                for (int i = 0; i < FrameCount; i++)
                {
                    img.SelectActiveFrame(dimension, i);
                    list[i] = (Image)img.Clone();
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
                    _images.Add(size, new Image[] { (Image)img.Clone() });
                if (!_sizes.ContainsKey(size))
                    _sizes.Add(size, img.Size);
            }
        }
    }
}