using LX29_ChatClient.Forms;
using LX29_Helpers;
using LX29_Twitch.Api;
using LX29_Twitch.Api.Video;
using LX29_Twitch.Forms;
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

    public class ChannelInfo : CustomSettings<ChannelInfo, ChannelSettings>, IDisposable, IEqualityComparer<ChannelInfo>, IEquatable<ChannelInfo>
    {
        public readonly bool IsFixed = false;
        public readonly object LockObject = new object();
        public readonly MPV_Wrapper MPV;
        private static int fetchCnt = 0;
        private bool _AutoLoginChat = false;
        private Bitmap[] _previewImages = new Bitmap[2];
        private FormChat chatForm = null;

        //public void DownloadBitmap()
        //{
        //    if (_previewImages[(IsOnline ? 1 : 0)] == null)
        //    {
        //        _downloadBitmap(IsOnline);
        //    }
        //}
        private DateTime lastCall = DateTime.Now;

        //private Task dwnldr = new Task(() => { });
        private FormPlayer playerForm = null;

        private ApiResult result;

        private VideoInfoCollection streamInfos = new VideoInfoCollection();

        private SubResult subInfo = null;

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

            Modes = new ChannelModes();

            MPV = new MPV_Wrapper(Name);

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

        //public string OnlineTimeString
        //{
        //    get { return OnlineTime.ToString((OnlineTime.TotalDays >= 1.0) ? @"%d'd 'hh':'mm':'ss" : @"hh\:mm\:ss"); }
        //}
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

        //public Actions.AutoActions AutoActions
        //{
        //    get;
        //    private set;
        //}
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

        public FormChat ChatForm
        {
            get { return chatForm; }
        }

        public Rectangle ChatPosition
        {
            get;
            set;
        }

        public string DisplayName
        {
            get { return GetValue<string>(ApiInfo.display_name); }
        }

        public bool Followed
        {
            get { return result.Followed; }
        }

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

        public string ID
        {
            get { return GetValue<string>(ApiInfo._id); }
        }

        public string Infos
        {
            get { return result.Infos; }
        }

        public bool IsChatConnected
        {
            get { return ChatClient.HasJoined(Name); }
        }

        public bool IsChatOpen
        {
            get { return chatForm != null; }
        }

        public bool IsFavorited
        {
            get;// { return Settings.GetValue<bool>(ChannelSettings.IsFavorited); }
            set;// { Settings.SetValue(ChannelSettings.IsFavorited, value); }
        }

        public bool IsOnline
        {
            get { return ApiResult.IsOnline; }
        }

        public bool IsViewing
        {
            get { return MPV.IsRunning; }
            //private set;
        }

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

        public ChannelModes Modes
        {
            get;
            private set;
        }

        public string Name
        {
            get { return GetValue<string>(ApiInfo.name); }
        }

        public Rectangle PlayerPosition
        {
            get;
            set;
        }

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
                return LX29_LixChat.Properties.Resources.loading;
            }
        }

        public int SlowMode
        {
            get;
            set;
        }

        public StreamType StreamType
        {
            get { return result.GetValue<StreamType>(ApiInfo.stream_type); }
        }

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

        public SubResult SubInfo
        {
            get
            {
                if (subInfo == null)
                {
                    return SubResult.Empty;
                }
                else
                {
                    return subInfo;
                }
            }
        }

        //public ChannelSettings<ChannelSettings> Settings
        //{
        //    get;
        //    set;
        //}
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
                if (proc.MainWindowTitle.Contains(MPV_Wrapper.WindowIdentifier))
                {
                    if (proc.MainWindowTitle.ToLower().StartsWith(Name))
                    {
                        MPV.SetProcess(proc);
                    }
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

                if (!s.IsEmpty())
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
            return int.Parse(info.ID);
        }

        public T GetValue<T>(ApiInfo type)
        {
            return result.GetValue<T>(type);
        }

        public void Load(Dictionary<string, string> list)
        {
            if (!IsFixed)
            {
                Load(list, this);
            }
        }

        public void ResetStreamStatus()
        {
            result.ResetStreamStatus();
            streamInfos = new VideoInfoCollection();
        }

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

        #region mpv

        private bool isStartingStream = false;

        public string[] MPV_Stats
        {
            get
            {
                if (MPV.IsRunning)
                {
                    var arr = new MPV_Property[] { MPV_Property.demuxer_cache_duration, MPV_Property.cache, MPV_Property.paused_for_cache, MPV_Property.video_bitrate, MPV_Property.audio_bitrate };
                    // var names = Enum.GetNames(typeof(MPV_Property)).Where(t => t.Contains("cache") || t.Contains("demuxer") || t.Contains("bitrate"));
                    List<string> values = new List<string>();
                    foreach (var prop in arr)
                    {
                        var s = Enum.GetName(typeof(MPV_Property), prop);
                        object value = MPV.GetProperty(prop);
                        if (value == null)
                            continue;
                        if (s.Contains("bitrate"))
                        {
                            if (value is float)
                            {
                                float val = (float)value;
                                value = val.SizeSuffix(2) + "/s";
                            }
                        }
                        values.Add(s + ": " + value);
                    }
                    return values.ToArray();
                }
                return new string[0];
            }
        }

        public void ShowVideoPlayer(string quality, bool external = false, Action<int, int, string> a = null)
        {
            if (isStartingStream) return;
            isStartingStream = true;

            if (!MPV_Downloader.MPV_Exists)
            {
                MPV_Downloader.DownloadMPV(a, () => ShowVideoPlayer(quality, external, a));
                return;
            }

            if (external)
            {
                Task.Run(() => Start(quality, a));
            }
            else
            {
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
            }
        }

        private async void Start(string quali, Action<int, int, string> a)
        {
            try
            {
                int cnt = 0;
                int maxtry = 3;
                int msretry = 10000;
                bool firstTry = true;
                while (true)
                {
                    if (StartMpvExternal(quali, a))
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
                        Start(quali, a);
                        break;
                }
            }
            isStartingStream = false;
        }

        private bool StartMpvExternal(string quali, Action<int, int, string> a)
        {
            var sdf = this.StreamURLS;
            if (!sdf.IsEmpty)
            {
                string url = sdf[quali].URL;
                MPV.Start(this.Name, url, (int)Settings.MpvBufferBytes, (int)Settings.MpvBufferSeconds, this.PlayerPosition);
                //MessageBox.Show(MPV.GetProperty(MPV_Property.ca).ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion mpv

        public override string ToString()
        {
            return Save(this);
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
    }

    public class ChannelModes
    {
        private Dictionary<msg_ids, object> modes;

        public ChannelModes()
        {
            modes = new Dictionary<msg_ids, object>();
        }

        public T GetMode<T>(msg_ids id)
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

        public void setMode(msg_ids id, object value)
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

        public void SetMode(msg_ids id, string value)
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