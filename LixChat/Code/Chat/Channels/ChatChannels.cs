using LX29_ChatClient.Forms;
using LX29_MPV;
using LX29_Twitch.Api;
using LX29_Twitch.Api.Video;
using LX29_Twitch.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Channels
{
    public enum ChannelSettings
    {
        ID,
        Name,
        IsFavorited,
        AutoLoginChat,
        LogChat,
        ChatPosition,
        PlayerPosition
    }

    //Add Whsiper Form (like chatForm)
    //Add Control for Settings
    // fg;

    public class ChannelInfo : IEqualityComparer<ChannelInfo>, IEquatable<ChannelInfo>, IDisposable
    {
        [JsonIgnore]
        public readonly bool IsFixed = false;

        [JsonIgnore]
        public readonly object LockObject = new object();

        [JsonIgnore]
        public readonly MpvLib MPV;

        private static int fetchCnt = 0;
        private bool _AutoLoginChat = false;
        private Bitmap[] _previewImages = new Bitmap[2];
        private FormChat chatForm = null;

        private DateTime lastCall = DateTime.Now;

        //private Task dwnldr = new Task(() => { });
        private FormPlayer playerForm = null;

        private ApiResult result;

        private VideoInfoCollection streamInfos = new VideoInfoCollection();

        private SubResult subInfo = null;

        [JsonConstructor]
        public ChannelInfo()
        {
        }

        public ChannelInfo(ChannelInfo info, bool Fixed = false, bool AutoChatLogin = false)
            : this(info.result, Fixed, AutoChatLogin)
        {
            IsFavorited = info.IsFavorited;
            LogChat = info.LogChat;
            Modes = new ChannelModes();
        }

        public ChannelInfo(ApiResult result, bool Fixed = false, bool AutoChatLogin = false)
        {
            IsFixed = Fixed;

            _AutoLoginChat = AutoChatLogin;

            ApiResult = result;

            ID = result.GetValue<int>(ApiInfo._id);
            Name = result.GetValue<string>(ApiInfo.name);

            Modes = new ChannelModes();

            MPV = new MpvLib();

            Task.Run(async () =>
            {
                while (fetchCnt > 10) await Task.Delay(50);
                fetchCnt++;
                subInfo = TwitchApi.GetSubscription(this.ID);

                CheckExisting_MPV_Instance();

                fetchCnt--;
            });

            //Settings = new ChannelSettings<ChannelSettings>();
            //AutoActions = new Actions.AutoActions();
        }

        [JsonIgnore]
        public ApiResult ApiResult
        {
            get { return result; }
            set
            {
                result = value;
                streamInfos = new VideoInfoCollection();
                // if (result.IsOnline) DownloadBitmap(true);
            }
        }

        public bool AutoLoginChat
        {
            get { return _AutoLoginChat; }// { return Settings.GetValue<bool>(ChannelSettings.AutoLoginChat); }
            set
            {
                if (!IsFixed)
                {
                    _AutoLoginChat = value;
                }
            }// { Settings.SetValue(ChannelSettings.AutoLoginChat, value); }
        }

        [JsonIgnore]
        public FormChat ChatForm
        {
            get { return chatForm; }
        }

        public Rectangle ChatPosition
        {
            get;
            set;
        }

        [JsonIgnore]
        public string DisplayName
        {
            get { return GetValue<string>(ApiInfo.display_name); }
        }

        [JsonIgnore]
        public bool Followed
        {
            get { return result.Followed; }
        }

        [JsonIgnore]
        public bool HasSlowMode
        {
            get
            {
                var user = ChatClient.Users.Get(ChatClient.SelfUserName, Name);
                if (user.Types.Any(t => (((int)t) >= (int)UserType.moderator)))
                {
                    return false;
                }
                return (SlowMode > 0);
            }
        }

        public int ID
        {
            get;// { return GetValue<int>(ApiInfo._id); }
            set;
        }

        [JsonIgnore]
        public string Infos
        {
            get { return result.Infos; }
        }

        [JsonIgnore]
        public bool IsChatConnected
        {
            get { return ChatClient.HasJoined(ID); }
        }

        [JsonIgnore]
        public bool IsChatOpen
        {
            get { return chatForm != null; }
        }

        public bool IsFavorited
        {
            get;// { return Settings.GetValue<bool>(ChannelSettings.IsFavorited); }
            set;// { Settings.SetValue(ChannelSettings.IsFavorited, value); }
        }

        [JsonIgnore]
        public bool IsOnline
        {
            get { return ApiResult.IsOnline; }
        }

        [JsonIgnore]
        public bool IsViewing
        {
            get { return MPV.IsRunning; }
            //private set;
        }

        [JsonIgnore]
        public DateTime LastSendMessageTime
        {
            get;
            set;
        }

        public bool LogChat
        {
            get;// { return Settings.GetValue<bool>(ChannelSettings.LogChat); }
            set;// { Settings.SetValue(ChannelSettings.LogChat, value); }
        }

        [JsonIgnore]
        public ChannelModes Modes
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public Rectangle PlayerPosition
        {
            get;
            set;
        }

        [JsonIgnore]
        public Bitmap PreviewImage
        {
            get
            {
                DownloadBitmap();
                if (result != null && !result.IsEmpty)
                {
                    var img = _previewImages[(result.IsOnline ? 1 : 0)];
                    if (img != null)
                        return img;
                }
                return null;
            }
        }

        [JsonIgnore]
        public int SlowMode
        {
            get;
            set;
        }

        [JsonIgnore]
        public StreamType StreamType
        {
            get { return result.GetValue<StreamType>(ApiInfo.stream_type); }
        }

        [JsonIgnore]
        public VideoInfoCollection StreamURLS
        {
            get
            {
                if (streamInfos.IsEmpty)
                {
                    if (streamInfos.LoadVideoInfos(Name) == VideoInfoError.Offline)
                    {
                        result.ResetStreamStatus();
                        streamInfos = new VideoInfoCollection();
                    }
                }
                return streamInfos;
            }
        }

        [JsonIgnore]
        public SubResult SubInfo
        {
            get
            {
                if (subInfo == null)
                {
                    return SubResult.NoSubProgram;
                }
                else
                {
                    return subInfo;
                }
            }
        }

        public static Dictionary<string, List<string>> ParseSavedChannels(IEnumerable<string> input)
        {
            try
            {
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                string id = string.Empty;
                foreach (var line in input)
                {
                    if (line.StartsWith("#"))
                    {
                        id = line.GetBetween("#", "(");
                        dict.Add(id, new List<string>());
                    }
                    else
                    {
                        dict[id].Add(line);
                    }
                }
                return dict;
                //Dictionary<string, ChannelSettings<ChannelSettings>> output = new Dictionary<string, ChannelSettings<ChannelSettings>>();
                //foreach (var kvp in dict)
                //{
                //    output.Add(kvp.Key, new ChannelSettings<ChannelSettings>(kvp.Value));
                //}
                //return output;
            }
            catch
            {
            }
            return null;
        }

        public void CheckExisting_MPV_Instance()
        {
            var procs = System.Diagnostics.Process.GetProcessesByName("mpv");
            foreach (var proc in procs)
            {
                if (proc.MainWindowTitle.Equals(Name + " | MPV-Frontend"))
                {
                    MPV.SetProcess(proc);
                }
            }
        }

        public void CloseChat()
        {
            if (chatForm != null)
            {
                chatForm.FormClosed -= chatForm_FormClosed;
                chatForm.LocationChanged -= chatForm_LocationChanged;
                chatForm.Close();
                chatForm.Dispose();
                chatForm = null;
            }
        }

        public void CloseVideoPlayer()
        {
            if (playerForm != null)
            {
                //ChatClient.StartAnimationLoop();
                playerForm.FormClosed -= playerForm_FormClosed;
                playerForm.LocationChanged -= playerForm_LocationChanged;
                playerForm.Close();
                playerForm.Dispose();
                playerForm = null;
                //IsViewing = false;
            }
        }

        public void Dispose()
        {
            bool b = this.Dispose(true);
        }

        public bool Dispose(bool dispose)
        {
            if (chatForm != null) chatForm.Dispose();
            if (playerForm != null) playerForm.Dispose();
            if (dispose) GC.SuppressFinalize(this);
            return dispose;
        }

        public void DownloadBitmap()
        {
            if (DateTime.Now.Subtract(lastCall).TotalMilliseconds >= 500)
            {
                Task.Run(() => DownloadBitmap(IsOnline));
            }
            lastCall = DateTime.Now;
        }

        public void DownloadBitmap(bool online)
        {
            try
            {
                int index = (online ? 1 : 0);
                if (!online && _previewImages[index] != null)
                {
                    return;
                }
                string s = (online == true ? result.GetValue<string>(ApiInfo.large) : result.GetValue<string>(ApiInfo.video_banner));

                if (!string.IsNullOrEmpty(s))
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.Proxy = null;
                        byte[] ba = wc.DownloadData(s);
                        using (MemoryStream ms = new MemoryStream(ba))
                        {
                            _previewImages[index] = (Bitmap)Bitmap.FromStream(ms);
                        }
                    }
                    ChatClient.ListUpdated();
                }
            }
            catch { }
        }

        public bool Equals(ChannelInfo obj)
        {
            return this.ID.Equals(obj.ID);
        }

        public bool Equals(ChannelInfo obj, ChannelInfo obj1)
        {
            return obj1.ID.Equals(obj.ID);
        }

        public int GetHashCode(ChannelInfo info)
        {
            return info.ID;
        }

        public T GetValue<T>(ApiInfo type)
        {
            return result.GetValue<T>(type);
        }

        public void Load(ChannelInfo basis)
        {
            if (!IsFixed)
            {
                IsFavorited = basis.IsFavorited;
                AutoLoginChat = basis.AutoLoginChat;
                LogChat = basis.LogChat;
                ChatPosition = basis.ChatPosition;
                PlayerPosition = basis.PlayerPosition;
                // Load(list, this);
            }
        }

        public void ResetStreamStatus()
        {
            result.ResetStreamStatus();
            streamInfos = new VideoInfoCollection();
        }

        //public override string ToString()
        //{
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(this);// Save(this);
        //}

        public void ShowChat()
        {
            if (chatForm == null)
            {
                //ChatClient.StartAnimationLoop();
                chatForm = new FormChat(this);
                chatForm.FormClosed += chatForm_FormClosed;

                chatForm.Show();

                if (!ChatPosition.IsEmpty)
                {
                    chatForm.Location = ChatPosition.Location;
                    chatForm.Size = ChatPosition.Size;
                }
                chatForm.LocationChanged += chatForm_LocationChanged;
            }
        }

        #region MPV Player Methods

        private bool isStartingStream = false;

        public enum PlayerType
        {
            Cinema,
            ExternalMPV,
            Record_ShowMPV,
        }

        //[JsonIgnore]
        //public string[] MPV_Stats
        //{
        //    get
        //    {
        //        if (MPV.IsRunning)
        //        {
        //            var arr = new MPV_Property[] { MPV_Property.demuxer_cache_duration, MPV_Property.cache, MPV_Property.paused_for_cache, MPV_Property.video_bitrate, MPV_Property.audio_bitrate };
        //            // var names = Enum.GetNames(typeof(MPV_Property)).Where(t => t.Contains("cache") || t.Contains("demuxer") || t.Contains("bitrate"));
        //            List<string> values = new List<string>();
        //            foreach (var prop in arr)
        //            {
        //                var s = Enum.GetName(typeof(MPV_Property), prop);
        //                object value = MPV.GetProperty(prop);
        //                if (value == null)
        //                    continue;
        //                if (s.Contains("bitrate"))
        //                {
        //                    if (value is float)
        //                    {
        //                        float val = (float)value;
        //                        value = val.SizeSuffix(2) + "/s";
        //                    }
        //                }
        //                values.Add(s + ": " + value);
        //            }
        //            return values.ToArray();
        //        }
        //        return new string[0];
        //    }
        //}

        public void GetMpvWindow()
        {
            if (MPV != null && MPV.IsRunning)
            {
                PlayerPosition = MPV.Position;
            }
        }

        public void ShowVideoPlayer(string quality, PlayerType external = PlayerType.Cinema, Action<int, int, string> a = null)
        {
            if (isStartingStream) return;
            isStartingStream = true;

            if (external == PlayerType.Record_ShowMPV && !File.Exists(Settings._pluginDir + "\\ffmpeg.exe"))
            {
                FormDownloader fd = new FormDownloader();
                fd.ShowDialog("FFMPEG ist needed for Recording.\r\nDownload now?",
                "https://github.com/lukix29/LixChat/raw/master/LixChat/Resources/ffmpeg.7z",
                Settings._pluginDir + "\\ffmpeg.7z");
            }
            //if (!File.Exists(".\\mpv-1.dll"))
            //{
            //    FormDownloader fd = new FormDownloader();
            //    fd.ShowDialog("MPV ist needed for watching Streams.\r\nDownload now?",
            //      "https://github.com/lukix29/LixChat/raw/master/LixChat/Resources/mpv.7z",
            //    Settings._pluginDir + "\\MPV\\mpv.7z");
            //}
            switch (external)
            {
                case PlayerType.Record_ShowMPV:
                case PlayerType.ExternalMPV:
                    Task.Run(() => Start(external, quality, a));
                    break;

                case PlayerType.Cinema:
                    if (playerForm == null)
                    {
                        playerForm = new FormPlayer();
                        playerForm.FormClosed += playerForm_FormClosed;

                        playerForm.Show(this, quality);

                        if (!PlayerPosition.IsEmpty)
                        {
                            playerForm.Location = PlayerPosition.Location;
                            playerForm.Size = PlayerPosition.Size;
                        }
                        playerForm.LocationChanged += playerForm_LocationChanged;
                        isStartingStream = false;
                        //IsViewing = true;
                    }
                    break;
            }
        }

        private void playerForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            playerForm.Dispose();
            playerForm = null;
            //IsViewing = false;
        }

        private void playerForm_LocationChanged(object sender, EventArgs e)
        {
            PlayerPosition = new Rectangle(playerForm.Location, playerForm.Size).ScreenSafe();
        }

        private async void Start(PlayerType type, string quali, Action<int, int, string> a)
        {
            try
            {
                int cnt = 0;
                int maxtry = 3;
                int msretry = 10000;
                bool firstTry = true;
                while (true)
                {
                    if (StartMpvExternal(type, quali, a))
                    {
                        break;
                    }
                    else
                    {
                        if (cnt == 0 && firstTry)
                        {
                            //auch auswählen wie oft 10x3 oder so
                            firstTry = false;
                            var res = LX29_MessageBox.Show("Stream is Offline. Retry Starting it?",
                                true, "10x3", "Seconds x Try's (every x second for n try's):", "", MessageBoxButtons.RetryCancel, MessageBoxIcon.Asterisk);
                            if (res.Result == MessageBoxResult.Cancel)
                            {
                                break;
                            }
                            else
                            {
                                var sa = res.Value.ToLower().Split('x');
                                if (int.TryParse(sa[0], out msretry))
                                {
                                }
                                if (sa.Length > 1)
                                {
                                    if (int.TryParse(sa[1], out maxtry))
                                    {
                                    }
                                    maxtry = Math.Max(1, Math.Min(100, maxtry));
                                }
                                msretry = Math.Max(10000, Math.Min(60000, msretry));
                            }
                        }
                        cnt++;
                        if (cnt >= maxtry)
                        {
                            int sec = (msretry / 1000);
                            var res = LX29_MessageBox.Show("Tryed to start Stream " + maxtry + "x. (" + sec + "s = " + (sec * maxtry) +
                                "s)\r\nRetry again?", "", MessageBoxButtons.RetryCancel, MessageBoxIcon.Asterisk);
                            if (res == MessageBoxResult.Cancel)
                            {
                                break;
                            }
                            cnt = 0;
                        }
                        await Task.Delay(msretry);
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Start(type, quali, a);
                        break;
                }
            }
            isStartingStream = false;
        }

        private bool StartMpvExternal(PlayerType type, string quali, Action<int, int, string> a)
        {
            var sdf = this.StreamURLS;
            if (!sdf.IsEmpty)
            {
                string url = sdf[quali].URL;
                bool b = false;
                switch (type)
                {
                    case PlayerType.Record_ShowMPV:
                        b = MpvLib.Record(this.Name, url);
                        break;

                    case PlayerType.ExternalMPV:
                        b = MPV.StartExternal(this.Name, url, 100, (int)Settings.MpvBufferBytes, (int)Settings.MpvBufferSeconds, this.PlayerPosition);
                        break;
                }
                return b;
                //MessageBox.Show(MPV.GetProperty(MPV_Property.ca).ToString());
                //return true;
            }
            else
            {
                return false;
            }
        }

        #endregion MPV Player Methods

        public override string ToString()
        {
            return ID + " - " + Name;
        }

        private void chatForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            chatForm.Dispose();
            chatForm = null;
        }

        private void chatForm_LocationChanged(object sender, EventArgs e)
        {
            ChatPosition = new Rectangle(chatForm.Location, chatForm.Size).ScreenSafe();
        }
    }

    //public interface ChannelInfoBase
    //{
    //    public bool AutoLoginChat { get; set; }
    //    public Rectangle ChatPosition { get; set; }
    //    public int ID { get; set; }
    //    public bool IsFavorited { get; set; }
    //    public bool LogChat { get; set; }
    //    public string Name { get; set; }
    //    public Rectangle PlayerPosition { get; set; }
    //}

    public class ChannelModes
    {
        private Dictionary<channel_mode, object> modes;

        public ChannelModes()
        {
            modes = new Dictionary<channel_mode, object>();
        }

        public T GetMode<T>(channel_mode id)
        {
            try
            {
                if (modes.ContainsKey(id))
                {
                    return (T)modes[id];
                }
            }
            catch { }
            return default(T);
        }

        public void SetMode(channel_mode id, string value)
        {
            bool b = false;
            int i = 0;
            if (bool.TryParse(value, out b))
            {
                setMode(id, b);
            }
            else if (int.TryParse(value, out i))
            {
                setMode(id, i);
            }
            else
            {
                setMode(id, value);
            }
        }

        private void setMode(channel_mode id, object value)
        {
            if (modes.ContainsKey(id))
            {
                modes[id] = value;
            }
            else
            {
                modes.Add(id, value);
            }
        }
    }

    //public class ChatSettings
    //{
    //    public ChatSettings()
    //    {
    //    }

    //    public ChatSettings(Dictionary<ChannelChatSettings, string> input)
    //    {
    //        if (input.Count > 0)
    //        {
    //            foreach (var type in input)
    //            {
    //                var key = Enum.GetName(typeof(ChannelChatSettings), type.Key);
    //                var prop = this.GetType().GetProperty(key);
    //                try
    //                {
    //                    if (null != prop && prop.CanWrite)
    //                    {
    //                        var value = Convert.ChangeType(type.Value, prop.PropertyType);
    //                        prop.SetValue(this, value);
    //                    }
    //                }
    //                catch
    //                {
    //                }
    //            }
    //        }
    //    }

    //    public bool AutoLoginChat
    //    {
    //        get;
    //        set;
    //    }

    //    public bool IsFavorited
    //    {
    //        get;
    //        set;
    //    }

    //    public static Dictionary<string, ChatSettings> ParseSavedChannels(IEnumerable<string> input)
    //    {
    //        try
    //        {
    //            var dict = new Dictionary<string, Dictionary<ChannelChatSettings, string>>();
    //            string id = string.Empty;
    //            foreach (var line in input)
    //            {
    //                if (line.StartsWith("#"))
    //                {
    //                    id = line.GetBetween("#", "(");
    //                    dict.Add(id, new Dictionary<ChannelChatSettings, string>());
    //                }
    //                else
    //                {
    //                    ChannelChatSettings type =
    //                        (ChannelChatSettings)Enum.Parse(typeof(ChannelChatSettings),
    //                        line.GetBetween("", ": "));
    //                    dict[id].Add(type, line.GetBetween(": ", ""));
    //                }
    //            }
    //            Dictionary<string, ChatSettings> output = new Dictionary<string, ChatSettings>();
    //            foreach (var kvp in dict)
    //            {
    //                output.Add(kvp.Key, new ChatSettings(kvp.Value));
    //            }
    //            return output;
    //        }
    //        catch
    //        {
    //        }
    //        return null;
    //    }
    //}
}