using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LX29_Twitch.Api.Video
{
    public enum VideoInfoError
    {
        None,
        Offline,
        Error
    }

    public class VideoInfo
    {
        //BANDWIDTH=3748000,RESOLUTION=1920x1080,CODECS="avc1.640028,mp4a.40.2",VIDEO="chunked"
        public readonly int Bitrate;

        public readonly string Codec;
        public readonly int Height;
        public readonly string Quality;
        public readonly string URL;
        public readonly int Width;

        public VideoInfo(string url, string info)
        {
            URL = url;
            int bitrate = 1000;
            int x = 0;
            int y = 0;
            string codec = "";
            string quali = "";

            string[] infs = info.ReplaceAll("", "\"").Substring(info.IndexOf(",")).Split(
                           new string[] { ",BANDWIDTH=", ",RESOLUTION=", ",CODECS=", ",VIDEO=" }, StringSplitOptions.RemoveEmptyEntries);
            if (infs.Length >= 4)
            {
                int.TryParse(infs[0], out bitrate);
                quali = infs[3].Replace("chunked", "source").ToLower();
                codec = infs[2];

                string[] resarr = infs[1].Trim().Split('x');
                if (resarr.Length >= 2)
                {
                    int.TryParse(resarr[0], out x);
                    int.TryParse(resarr[1], out y);
                }
                else
                {
                }
            }
            else
            {
                int.TryParse(infs[0], out bitrate);
                quali = infs[2].Replace("chunked", "source").ToUpper();
                codec = infs[1];
            }
            Codec = codec;
            Quality = quali;
            Width = x;
            Height = y;
            Bitrate = bitrate / 1000;
        }

        public static bool IsVideoInfoString(string input)
        {
            return input.ContainsAny("EXT-X-STREAM-INF", ",BANDWIDTH=", ",RESOLUTION=", ",CODECS=", ",VIDEO=");
        }

        public override string ToString()
        {
            string size = (Width > 0) ? (Width.ToString("N0") + "x" + Height.ToString("N0") + " @ ") : "";
            return Quality + "\n" + size + Bitrate.ToString("N0") + "kb/s\n" + Codec;
        }
    }

    public class VideoInfoCollection
    {
        private const string TOKEN_API = "http://api.twitch.tv/api/channels/{channel}/access_token";

        private const string USHER_API = "http://usher.twitch.tv/api/channel/hls/{channel}.m3u8?player=twitchweb&token={token}&sig={sig}&allow_audio_only=true&allow_source=true&type=any&p={random}";

        public VideoInfoCollection()
        {
            videoInfos = new Dictionary<string, VideoInfo>();
        }

        public int Count
        {
            get { return videoInfos.Count; }
        }

        public bool IsEmpty
        {
            get { return videoInfos.Count == 0; }
        }

        public string[] KeysUpper
        {
            get { return videoInfos.Keys.Select(s => s.ToUpper()).ToArray<string>(); }
        }

        private Dictionary<string, VideoInfo> videoInfos { get; set; }

        public VideoInfo this[string quality]
        {
            get
            {
                try
                {
                    var qual = quality.ToLower();
                    if (videoInfos.ContainsKey(qual))
                    {
                        return videoInfos[qual];
                    }
                }
                catch { }
                return null;
            }
        }

        public bool Contains(string quality)
        {
            quality = quality.ToLower();
            return videoInfos.ContainsKey(quality);
        }

        public VideoInfoError LoadVideoInfos(string channel)
        {
            WebClient wc = new WebClient();
            wc.Proxy = null;
            wc.Encoding = Encoding.UTF8;
            videoInfos = new Dictionary<string, VideoInfo>();// new VideoInfoCollection();

            string t = "";
            try
            {
                t = wc.DownloadString(TOKEN_API.Replace("{channel}", channel.ToLower()) + "?client_id=" + TwitchApi.CLIENT_ID);
            }
            catch (WebException x)
            {
                var res = x.Response as HttpWebResponse;
                if (res.StatusCode == HttpStatusCode.NotFound)
                {
                    return VideoInfoError.Offline;
                }
                return VideoInfoError.Error;
            }
            string token = t.GetBetween("\"token\":\"", "\",\"sig\":");

            token = token.Replace("\\\"", "\"");

            string sig = t.GetBetween("\"sig\":\"", "\",");
            string rand = new Random().Next(Int16.MaxValue, Int32.MaxValue).ToString().Substring(0, 6);

            string req = USHER_API.Replace("{channel}", channel.ToLower())
                .Replace("{token}", token).Replace("{sig}", sig).Replace("{random}", rand);

            string vids = "";
            try
            {
                vids = wc.DownloadString(req);
            }
            catch (WebException x)
            {
                var res = x.Response as HttpWebResponse;
                if (res.StatusCode == HttpStatusCode.NotFound)
                {
                    return VideoInfoError.Offline;
                }
                return VideoInfoError.Error;
            }
            finally
            {
                wc.Dispose();
            }
            string[] sa = vids.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 2; i < sa.Length; i++)
            {
                string url = sa[i];
                if (url.StartsWith("http"))
                {
                    VideoInfo vi = new VideoInfo(url, sa[i - 1]);
                    if (!videoInfos.ContainsKey(vi.Quality.ToLower()))
                    {
                        videoInfos.Add(vi.Quality.ToLower(), vi);
                    }
                }
            }
            return VideoInfoError.None;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in videoInfos)
            {
                sb.AppendLine(v.Value.ToString());
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}