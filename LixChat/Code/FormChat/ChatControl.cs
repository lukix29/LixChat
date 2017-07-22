using LX29_ChatClient.Channels;
using LX29_ChatClient.Emotes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class ChatView : UserControl
    {
        private ChannelInfo channel;

        private bool loopRunning = false;
        private Keys modifier_Key = Keys.None;

        //private Point mouseLocation = Point.Empty;
        private bool onMouseDown = false;

        private RenderDevice renderer;

        private string selectedText = "";

        public ChatView()
        {
            try
            {
                InitializeComponent();

                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserMouse, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.ResizeRedraw, true);

                this.tSMi_Copy.Click += tSMi_Copy_Click;
                this.tSMi_Search.Click += tSMi_Search_Click;
                this.MouseWheel += ChatView_MouseWheel;

                renderer = new RenderDevice(this);
            }
            catch
            {
            }
        }

        public delegate void BadgeClicked(ChatView sender, Badge user);

        public delegate void EmoteClicked(ChatView sender, Emote emote);

        public delegate void LinkClicked(ChatView sender, string url);

        public delegate void MessageReceived(ChatView sender, ChatMessage message);

        public delegate void UserNameClicked(ChatView sender, ChatUser emote, Point mouse);

        public event BadgeClicked OnBadgeClicked;

        public event EmoteClicked OnEmoteClicked;

        public event LinkClicked OnLinkClicked;

        public event MessageReceived OnMessageReceived;

        public event UserNameClicked OnUserNameClicked;

        [ReadOnly(true)]
        [Browsable(false)]
        public ChannelInfo Channel
        {
            get { return channel; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public new Font Font
        {
            get { return renderer.Font; }
            set { renderer.Font = value; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public int MessageCount
        {
            get { return renderer.ViewStart; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public bool Pause
        {
            get { return renderer.Pause; }
            set { renderer.Pause = value; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public bool ShowEmotes
        {
            get { return renderer.ShowEmotes; }
            set { renderer.ShowEmotes = value; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public bool ShowName
        {
            get { return renderer.ShowName; }
            set { renderer.ShowName = value; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public string UserMessageName
        {
            get { return renderer.WhisperName; }
            //set { renderer.UserMessageName = value; }
        }

        public void SetAllMessages(MsgType type, ChannelInfo ci = null, string name = "")
        {
            if (ci != null) renderer.Channel = ci;
            renderer.SetAllMessages(type, name);
        }

        public void SetChannel(ChannelInfo ci, MsgType type, string name = "")
        {
            ChatClient.TryConnect(ci.Name);
            channel = ci;
            SetAllMessages(type, ci, name);
            Stop();
            ChatClient.OnMessageReceived += ChatClient_MessageReceived;
            ChatClient.OnTimeout += ChatClient_UserHasTimeouted;

            ChatClient.Messages.OnWhisperReceived += ChatClient_OnWhisperReceived;

            if (!loopRunning)
            {
                loopRunning = true;
                Task.Run(new Action(RefreshLoop));
                //Task.Run(() => EmoteLoop());
            }
        }

        public void SetFontSize(bool incement)
        {
            renderer.SetFontSize(incement);
        }

        public void SetFontSize(decimal value)
        {
            renderer.SetFontSize(value);
        }

        public void Stop()
        {
            if (channel != null)
            {
                ChatClient.OnMessageReceived -= ChatClient_MessageReceived;
                ChatClient.OnTimeout -= ChatClient_UserHasTimeouted;
            }
            ChatClient.Messages.OnWhisperReceived -= ChatClient_OnWhisperReceived;
            //stop = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            modifier_Key = e.Modifiers;
            KeysConverter kc = new KeysConverter();
            char keyChar = kc.ConvertToString(e.KeyCode)[0];
            if (e.KeyValue == 187 || e.KeyValue == 189)
            {
                renderer.SetFontSize(e.KeyValue == 187);
            }

            base.OnKeyDown(e);

            e.SuppressKeyPress = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            modifier_Key = Keys.None;
            base.OnKeyUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            renderer.SelectRect.Location = e.Location;
            onMouseDown = true;
            //renderer.AutoScroll = false;

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //mouseLocation = e.Location;
            if (onMouseDown)
            {
                int x0 = (int)renderer.SelectRect.X;
                int x1 = e.X;
                if (x0 > x1)
                {
                    //fgh
                    x0 = e.X;
                    x1 = (int)renderer.SelectRect.X;
                }
                renderer.SelectRect = RectangleF.FromLTRB(x0, renderer.SelectRect.Y, x1, renderer.SelectRect.Y + 1);
            }
            else
            {
                renderer.SelectRect = new RectangleF(e.X, e.Y, 0, 0);
            }
            try
            {
                var list = renderer.ClickableList;
                SLRect curSelected = list.FirstOrDefault(t => t.Contains(e.Location));
                if (curSelected.IsEmpty)
                {
                    Cursor = Cursors.Arrow;
                    //renderer.AutoScroll = true;
                }
                else if (curSelected.Type != RectType.Text)
                {
                    Cursor = Cursors.Hand;
                    //renderer.AutoScroll = false;
                }
                // this.Invalidate();
            }
            catch
            {
                Cursor = Cursors.Arrow;
                // renderer.AutoScroll = true;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {
                if (renderer.SelectRect.Width > 0)
                {
                    var list = renderer.ClickableList.ToList();
                    var selects = list.Where(t => t.Bounds.IntersectsWith(renderer.SelectRect));
                    if (selects.Count() > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var s in selects)
                        {
                            switch (s.Type)
                            {
                                case RectType.User:
                                    var user = (ChatUser)s.Content;
                                    sb.Append(user.Name + ": ");
                                    break;

                                case RectType.Emote:
                                    var emote = (Emote)s.Content;
                                    sb.Append(emote.Name + " ");
                                    break;

                                case RectType.Badge:
                                    var badge = (BadgeBase)s.Content;
                                    sb.Append(badge.Name + " ");
                                    break;

                                default:
                                    sb.Append(s.Content + " ");
                                    break;
                            }
                        }
                        selectedText = sb.ToString().TrimEnd(' ');

                        if (!string.IsNullOrEmpty(selectedText))
                        {
                            tSMi_Text.Text = selectedText;
                            cMS_TextOptions.Show(this.PointToScreen(e.Location));
                        }
                    }
                }
                CheckClick(e.Location);

                renderer.SelectRect = Rectangle.Empty;
            }
            catch { }
            onMouseDown = false;
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //if (!renderer.gifVisible)
            //    renderer.Render(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            if (renderer != null) renderer.Invalidate();
        }

        private void ChatClient_MessageReceived(ChatMessage message)
        {
            if (!this.Visible) return;

            //if (message.IsType(renderer.MessageType))
            //{
            //    renderer.SetAllMessages(renderer.MessageType, "");
            //}

            if (OnMessageReceived != null)
                OnMessageReceived(this, message);
        }

        private void ChatClient_OnWhisperReceived(ChatMessage message)
        {
            if (!this.Visible) return;

            //if (renderer.MessageType == MsgType.Whisper)
            //{
            //    renderer.SetAllMessages(renderer.MessageType, renderer.WhisperName);
            //    //renderer.MessageCount = ChatClient.Messages.Count(string.Empty, renderer.MessageType, renderer.UserMessageName);
            //}
        }

        private void ChatClient_UserHasTimeouted(TimeOutResult result)
        {
            if (!this.Visible) return;
            //if (renderer.MessageType == MsgType.All_Messages)
            //{
            //    if (result.Channel.Equals(channel.Name, StringComparison.OrdinalIgnoreCase))
            //    {
            //        renderer.SetAllMessages(MsgType.All_Messages);
            //        //curSelected.Clear();// SLRect.Empty;
            //        if (!Pause) this.Invalidate();
            //    }
            //}
        }

        private void ChatView_MouseLeave(object sender, EventArgs e)
        {
            renderer.SelectRect = Rectangle.Empty;
            onMouseDown = false;
        }

        private void ChatView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (modifier_Key == Keys.Control)
            {
                renderer.SetFontSize(e.Delta > 0);
            }
            else
            {
                renderer.ScrollEmotes(e.Delta);

                if (renderer.ViewStart > 0)
                {
                    lbl_ScrollDown.Visible = true;
                    lbl_ScrollDown.BringToFront();
                }
                else
                {
                    lbl_ScrollDown.Visible = false;
                }
            }
            // this.Invalidate();
        }

        private void CheckClick(Point Location)
        {
            SLRect curSelected = renderer.ClickableList.FirstOrDefault(t => t.Bounds.Contains(Location));
            if (!curSelected.IsEmpty)
            {
                switch (curSelected.Type)
                {
                    case RectType.ModActionBan:
                        {
                            string name = curSelected.Content.ToString();
                            ChatClient.SendMessage("/ban " + name, Channel.Name);
                        }
                        break;

                    case RectType.ModActionTimeout:
                        {
                            string name = curSelected.Content.ToString();
                            ChatClient.SendMessage("/timeout " + name, Channel.Name);
                        }
                        break;

                    case RectType.Link:
                        if (OnLinkClicked != null)
                            OnLinkClicked(this, curSelected.Content.ToString());
                        break;

                    case RectType.Emote:
                        Emote em = (Emote)curSelected.Content;
                        if (OnEmoteClicked != null)
                            OnEmoteClicked(this, em);
                        break;

                    case RectType.User:
                        ChatUser cu = (ChatUser)curSelected.Content;
                        if (OnUserNameClicked != null)
                            OnUserNameClicked(this, cu, Location);
                        break;

                    case RectType.Badge:
                        Badge bd = (Badge)curSelected.Content;
                        if (OnBadgeClicked != null)
                            OnBadgeClicked(this, bd);
                        break;

                    case RectType.Delegate:
                        curSelected.Invoke();
                        break;
                }
            }
            else
            {
            }
        }

        private void lbl_ScrollDown_Click(object sender, EventArgs e)
        {
            if (lbl_ScrollDown.Visible)
            {
                renderer.ViewStart = 0;
                lbl_ScrollDown.Visible = false;
            }
        }

        //private Renderer renderer;
        private async void RefreshLoop()
        {
            long dt = DateTime.Now.Ticks;
            while (true)
            {
                if (this.IsDisposed)
                    break;

                try
                {
                    double wait = 30;
                    if (!Pause)
                    {
                        if (!renderer.gifVisible)
                        {
                            wait = 200;
                        }
                        if (renderer.Render())
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(ShowScrollDownLabel));
                            }
                            else
                            {
                                ShowScrollDownLabel();
                            }
                        }
                        //}
                        //else
                        //{
                        //    if (this.InvokeRequired)
                        //    {
                        //        this.Invoke(new Action(this.Invalidate));
                        //    }
                        //    else
                        //    {
                        //        this.Invalidate();
                        //    }

                        //    wait = 200;
                        //}
                    }
                    else wait = 1000;

                    double tt = ((DateTime.Now.Ticks - dt) / (double)TimeSpan.TicksPerMillisecond);
                    if (tt < wait)
                    {
                        await Task.Delay((int)(wait - tt));
                    }
                    dt = DateTime.Now.Ticks;
                }
                catch
                {
                }
            }
        }

        private void ShowScrollDownLabel()
        {
            lbl_ScrollDown.Visible = true;
        }

        private void tSMi_Copy_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedText))
            {
                Clipboard.SetText(selectedText, TextDataFormat.UnicodeText);
            }
        }

        private void tSMi_Search_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.google.at/search?q=" + selectedText);
        }
    }
}

namespace LX29_ChatClient.Forms
{
    //public partial class ChatView
    //{
    //public class DX_Renderer
    //{
    //    private SlimDX.Direct2D.WindowRenderTarget d2dWindowRenderTarget;

    //    public void BeginDrawing()
    //    {
    //        d2dWindowRenderTarget.BeginDraw();
    //    }

    //    public void Clear()
    //    {
    //        d2dWindowRenderTarget.Clear(new SlimDX.Color4(Color.LightSteelBlue));
    //    }

    //    public void Create(Control targetControl)
    //    {
    //        SlimDX.Direct2D.Factory d2dFactory = new SlimDX.Direct2D.Factory(SlimDX.Direct2D.FactoryType.Multithreaded, SlimDX.Direct2D.DebugLevel.None);
    //        SlimDX.Direct2D.WindowRenderTarget renderer =
    //            new SlimDX.Direct2D.WindowRenderTarget(d2dFactory,
    //                new SlimDX.Direct2D.WindowRenderTargetProperties()
    //                {
    //                    Handle = targetControl.Handle,
    //                    PixelSize = targetControl.Size,
    //                    PresentOptions = SlimDX.Direct2D.PresentOptions.Immediately
    //                });

    //        d2dWindowRenderTarget = renderer;
    //    }

    //    public void DrawImage(SlimDX.Direct2D.Bitmap d2dBitmap, RectangleF rect)
    //    {
    //        d2dWindowRenderTarget.DrawBitmap(d2dBitmap, rect);// new Rectangle(0, 0, (int)d2dBitmap.Size.Width, (int)d2dBitmap.Size.Height));/**/
    //    }

    //    public void EndDrawing()
    //    {
    //        d2dWindowRenderTarget.EndDraw();
    //    }

    //    public SlimDX.Direct2D.Bitmap MakeBitmap(Bitmap bitmap)
    //    {
    //        // SlimDX.Direct2D.Bitmap d2dBitmap = null;
    //        System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);//TODO: PixelFormat is very important!!! Check!
    //        SlimDX.DataStream dataStream = new SlimDX.DataStream(bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, true, false);
    //        SlimDX.Direct2D.PixelFormat d2dPixelFormat = new SlimDX.Direct2D.PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, SlimDX.Direct2D.AlphaMode.Premultiplied);
    //        SlimDX.Direct2D.BitmapProperties d2dBitmapProperties = new SlimDX.Direct2D.BitmapProperties();
    //        d2dBitmapProperties.PixelFormat = d2dPixelFormat;
    //        var d2dBitmap = new SlimDX.Direct2D.Bitmap(d2dWindowRenderTarget, new Size(bitmap.Width, bitmap.Height), dataStream, bitmapData.Stride, d2dBitmapProperties);
    //        bitmap.UnlockBits(bitmapData);
    //        return d2dBitmap;
    //    }
    //}

    public class RenderDevice
    {
        public bool ShowName = true;

        private bool _showEmotes = false;

        //private DX_Renderer dx = new DX_Renderer();
        private int emoteDlCnt = 0;

        private int emoteDlCntMax = 0;

        private DateTime emoteDLTime = DateTime.UtcNow;

        private float emoteDrawMax = -600;

        private float emoteDrawStart = 0;

        private Color Link_Color = Color.DodgerBlue;

        private int msgCount = 0;

        private bool swapColor = true;

        public RenderDevice(Control form)
        {
            control = form;

            internalImages.Add(internImages.Ban,
                new EmoteImage("Ban", LX29_TwitchChat.Properties.Resources.ban));
            internalImages.Add(internImages.Timeout,
               new EmoteImage("Timeout", LX29_TwitchChat.Properties.Resources.timeout));
            //dx.Create(control);

            infoStrFormat.LineAlignment = StringAlignment.Near;
            infoStrFormat.Alignment = StringAlignment.Far;
            infoStrFormat.Trimming = StringTrimming.None;

            centerStrFormat = new StringFormat(infoStrFormat);
            centerStrFormat.Alignment = StringAlignment.Center;
            centerStrFormat.LineAlignment = StringAlignment.Center;

            VcenterStrFormat = new StringFormat(centerStrFormat);
            VcenterStrFormat.LineAlignment = StringAlignment.Center;
            VcenterStrFormat.Alignment = StringAlignment.Near;

            Font = new System.Drawing.Font("Calibri", 12f);
            //FontBaseSize = Font.Size;

            MessageType = MsgType.All_Messages;
            AutoScroll = true;

            Font = new Font(Settings.ChatFontName, (float)Settings.ChatFontSize);
        }

        private enum internImages
        {
            Ban,
            Timeout,
        }

        #region Fields

        public bool AutoScroll = true;
        public ChannelInfo Channel;
        public List<SLRect> ClickableList = new List<SLRect>();
        public bool gifVisible = false;
        public RectangleF SelectRect = RectangleF.Empty;
        private const int timeSizeFac = 2;
        private readonly object drawLock = new object();
        private float _CharHeight = 0f;

        //private Color BG_Color = UserColors.ChatBackground;
        //private Color BG_Color1 = UserColors.ChatBackground1;
        private BufferedGraphics bufferedGraphics = null;

        private BufferedGraphicsContext bufferedGraphicsContex = null;
        private StringFormat centerStrFormat = new StringFormat();
        private Control control;
        private long dtFps = DateTime.Now.Ticks;
        private long dtFPS_Lock = DateTime.Now.Ticks;
        private Font font = new Font("Arial", 12f);
        private int FPS = 0;
        private int fpscnt = 0;

        // private Color HL_Color = Color.White;
        private StringFormat infoStrFormat = new StringFormat();

        private Dictionary<internImages, EmoteImage> internalImages = new Dictionary<internImages, EmoteImage>();
        private bool isChangingGraphics = false;

        private Point mouseLocation = Point.Empty;

        private Color MSG_Color = Color.Gainsboro;

        private SolidBrush Notice_Color = new SolidBrush(UserColors.ToColor("#d8c6ff"));

        private Regex reg = new Regex(@"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$",
                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Font timeFont;

        private Color TO_Color = Color.DarkGray;

        private Font userFont;

        private StringFormat VcenterStrFormat = new StringFormat();

        private int viewStart = 0;

        private float _BadgePadding
        {
            get { return (float)Settings.BadgePadding; }
        }

        private float _BadgeSizeFac
        {
            get { return (float)Settings.BadgeSizeFac; }
        }

        private float _EmotePadding
        {
            get { return (float)Settings.EmotePadding; }
        }

        private float _EmoteSizeFac
        {
            get { return (float)Settings.EmoteSizeFac; }
        }

        private float _LinePadding
        {
            get { return (float)Settings.LinePadding; }
        }

        private float _LineSpacing
        {
            get { return (float)Settings.LineSpacing; }
        }

        private float _TimePadding
        {
            get { return (float)Settings.TimePadding; }
        }

        private float _WordPadding
        {
            get { return (float)Settings.WordPadding; }
        }

        private bool UserIsMod
        {
            get;
            set;
        }

        #endregion Fields

        #region Properties

        public FontStyle TimeOutStyle = FontStyle.Strikeout;

        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                userFont = new Font(value, FontStyle.Bold);
                timeFont = new Font(font.FontFamily, font.Size - timeSizeFac, font.Style);
            }
        }

        //public int MessageCount
        //{
        //    get;
        //    private set;
        //}

        public MsgType MessageType
        {
            get;
            set;
        }

        public bool Pause
        {
            get;
            set;
        }

        public bool SmallEmotes
        {
            get;
            set;
        }

        public int ViewStart
        {
            get { return viewStart; }
            set
            {
                if (Channel != null)
                {
                    int msgcnt = ChatClient.Messages.Count(Channel.Name, MessageType);
                    viewStart = Math.Max(0, Math.Min(msgcnt, value));
                    AutoScroll = (viewStart > 0) ? false : true;
                    //if (AutoScroll) MessageCount = msgcnt;
                }
            }
        }

        public string WhisperName
        {
            get;
            set;
        }

        #endregion Properties

        public bool ShowEmotes
        {
            get { return _showEmotes; }
            set
            {
                _showEmotes = value;
                ClickableList.Clear();

                //gifVisible = true;
            }
        }

        [DllImport("coredll.dll")]
        public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        public void Invalidate()
        {
            try
            {
                if (isChangingGraphics) return;
                isChangingGraphics = true;
                lock (drawLock)
                {
                    if (bufferedGraphics != null) bufferedGraphics.Dispose();
                    bufferedGraphicsContex = BufferedGraphicsManager.Current;
                    bufferedGraphics = bufferedGraphicsContex.Allocate(control.CreateGraphics(), control.ClientRectangle);
                    bufferedGraphics.Graphics.SetGraphicQuality(true, true);
                }
                isChangingGraphics = false;
            }
            catch
            {
            }
        }

        public bool Render()
        {
            return Render(bufferedGraphics.Graphics, true);
        }

        //public void Reload()
        //{
        //    //MessageCount = ChatClient.Messages.Count(Channel.Name, MessageType);
        //}
        public bool Render(Graphics g, bool b = false)
        {
            if (b)
            {
                g = bufferedGraphics.Graphics;
            }
            lock (drawLock)
            {
                if (b) g.Clear(UserColors.ChatBackground);
                if (ShowEmotes)
                {
                    DrawEmotes(g);
                }
                else
                {
                    DrawMessagesNew(g);
                }
                DrawInfoOverlay(g);
                if (b) bufferedGraphics.Render();
            }

            int ttms = (int)((DateTime.Now.Ticks - dtFps) / TimeSpan.TicksPerMillisecond);
            if (ttms > 1000)
            {
                FPS = fpscnt;
                fpscnt = 0;

                dtFps = DateTime.Now.Ticks;
            }
            fpscnt++;

            if (viewStart > 0)
            {
                AutoScroll = false;
                return true;
            }
            return false;
        }

        public void ScrollEmotes(int delta)
        {
            if (!_showEmotes)
            {
                int d = (int)((Math.Abs(delta) / 100f) + 0.5);

                if (delta > 0) ViewStart += d;
                else if (delta < 0) ViewStart -= d;
            }
            else
            {
                emoteDrawStart = Math.Max(-600, Math.Min(0, emoteDrawStart + (delta / 10)));
            }
        }

        public void SetAllMessages(MsgType Type, string name = null)
        {
            _showEmotes = false;
            WhisperName = name;
            MessageType = Type;
            var user = ChatClient.Users.Get(ChatClient.SelfUserName, Channel.Name);
            if (user != null)
            {
                UserIsMod = user.Types.Any(t => ((int)t >= (int)UserType.moderator));
            }
            //this.Invalidate();
            //MessageCount = ChatClient.Messages.Count(Channel.Name, MessageType, UserMessageName);
        }

        public void SetFontSize(decimal value)
        {
            value = Math.Max(timeSizeFac + 1, value);
            this.Font = new Font(font.FontFamily, (float)value, font.Style);
            //return GetFontPercent();
        }

        public void SetFontSize(bool increment)
        {
            float fac = -0.1f;
            if (increment) fac = 0.1f;
            fac = Math.Max(timeSizeFac + 1, font.Size + fac);
            this.Font = new Font(font.FontFamily, fac, font.Style);
            //Make zoom independent from normal chat Font
            // fggdf;
        }

        private void DrawEmotes(Graphics g)
        {
            try
            {
                RectangleF bounds = g.VisibleClipBounds;

                if (!ChatClient.Emotes.Finished)
                {
                    g.DrawText("Loading Emotes", this.font, Color.White, bounds, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    return;
                }
                float x = _EmotePadding;
                float y = emoteDrawStart + _EmotePadding;

                var emotes = ChatClient.Emotes.GetEmotes(Channel.Name)
                    .OrderByDescending(t => t.Origin == EmoteOrigin.Twitch_Global)
                    .ThenByDescending(t => t.Origin == EmoteOrigin.BTTV_Global)
                    .ThenByDescending(t => t.Origin == EmoteOrigin.FFZ_Global)
                    .ThenBy(t => t.Channel);
                string lastChannel = emotes.ElementAt(0).Channel;

                g.DrawString(lastChannel, this.font, Brushes.Gainsboro, x, y);
                y += g.MeasureString(lastChannel, this.font).Height + _EmotePadding;

                gifVisible = false;

                SizeF size = SizeF.Empty;
                ClickableList.Clear();
                foreach (var em in emotes)
                {
                    size = em.Image.CalcSize(32);
                    if (!lastChannel.Contains(em.Channel))
                    {
                        y += size.Height + _EmotePadding;
                        x = _EmotePadding;
                        g.DrawString(em.Channel, this.font, Brushes.Gainsboro, x, y);
                        y += g.MeasureString(em.Channel, this.font).Height + _EmotePadding;
                        lastChannel = em.Channel;
                    }
                    if (x + size.Width >= bounds.Right - _EmotePadding)
                    {
                        y += size.Height + _EmotePadding;
                        x = _EmotePadding;
                    }
                    if (!_showEmotes) return;
                    var result = em.Image.Draw(g, x, y, size.Width, size.Height, false, EmoteImageSize.Small);
                    if (result == EmoteImageDrawResult.IsGif)
                    {
                        gifVisible = true;
                    }
                    if (result == EmoteImageDrawResult.Downloading)
                    {
                        if (DateTime.UtcNow.Subtract(emoteDLTime).TotalMilliseconds > 500) emoteDlCntMax = Math.Max(1, emoteDlCntMax - 1);
                        else emoteDlCntMax = Math.Min(20, emoteDlCntMax + 1);

                        emoteDlCnt++;
                        if (emoteDlCnt > emoteDlCntMax)
                        {
                            emoteDLTime = DateTime.UtcNow;
                            emoteDlCnt = 0;
                            break;
                        }
                    }
                    else
                    {
                        emoteDlCnt--;
                    }

                    ClickableList.Add(new SLRect(x, y, size.Width, size.Height, em, RectType.Emote));
                    x += size.Width + _EmotePadding;
                }
                emoteDrawMax = y + size.Height + _EmotePadding;
            }
            catch
            {
            }
        }

        private void DrawInfoOverlay(Graphics g)
        {
            Emote emote = null;
            float width = g.VisibleClipBounds.Width;
            float Height = g.VisibleClipBounds.Height;
            var selects = ClickableList.Where(t => t.Bounds.IntersectsWith(SelectRect)).ToList();
            if (selects.Count > 0)
            {
                foreach (var curSelected in selects)
                {
                    string Text = string.Empty;
                    switch (curSelected.Type)
                    {
                        case RectType.ModActionTimeout:
                            Text = "Timeout User for 600s";
                            break;

                        case RectType.ModActionBan:
                            Text = "Ban User";
                            break;

                        case RectType.User:
                            var user = (ChatUser)curSelected.Content;
                            Text = user.Name;
                            int msgcnt = ChatClient.Messages.MessageCount(Channel.Name, user.Name);
                            Text += "\r\nMessages:" + msgcnt;
                            if (user.HasTimeOut)
                            {
                                if (user.To_Timer != null)
                                {
                                    Text += "\r\nTimeout: " + TimeSpan.FromSeconds(user.To_Timer.Result.TimeOutDuration).ToString();
                                }
                            }
                            else if (user.IsBanned)
                            {
                                Text += "\r\nBanned";
                            }
                            break;

                        case RectType.Emote:
                            emote = (Emote)curSelected.Content;
                            Text = "Emote: " + emote.Name + "\r\nChannel: " + emote.Channel;
                            break;

                        case RectType.Badge:
                            var badge = (BadgeBase)curSelected.Content;
                            Text = badge.Type.UppercaseFirst();
                            string type = "";
                            if (badge.Type.EqualsAny(out type, StringComparison.OrdinalIgnoreCase
                                , "subscriber", "bits"))
                            {
                                switch (type)
                                {
                                    case "subscriber":
                                        Text += "\r\nMonths: " + badge.Version;
                                        break;

                                    case "bits":
                                        Text += "\r\nAmount: " + badge.Version;
                                        break;
                                }
                            }
                            break;
                    }

                    if (!string.IsNullOrEmpty(Text))
                    {
                        SolidBrush BackColor = new SolidBrush(Color.FromArgb(240, 0, 0, 0));

                        RectangleF bounds = new RectangleF();
                        SizeF txtSize = g.MeasureText(Text, font);
                        bounds.Size = txtSize;
                        bounds.Size = new SizeF((int)(bounds.Width * 1.1f), (int)(bounds.Height * 1.25f));

                        float emSiz = bounds.Width / 1.5f;
                        if (emote != null)
                        {
                            bounds.Height += emSiz;
                        }

                        bounds.Location = new PointF(curSelected.Bounds.X - ((bounds.Width - curSelected.Bounds.Width) / 2), (curSelected.Bounds.Y - bounds.Height) - 5);
                        bounds.X = Math.Max(0, Math.Min(width - bounds.Width, bounds.X));
                        bounds.Y = Math.Max(0, Math.Min(Height - bounds.Height, bounds.Y));

                        g.FillRectangle(BackColor, bounds);
                        g.DrawText(Text, font, Color.WhiteSmoke, bounds, TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);
                        g.DrawRectangle(Pens.LightGray, Rectangle.Truncate(bounds));

                        if (emote != null)
                        {
                            emote.Image.Draw(g, bounds.X + emSiz / 4f, bounds.Y + txtSize.Height, emSiz, emSiz, false, EmoteImageSize.Large);
                        }
                    }
                    if (SelectRect.Width > 0)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.LightSteelBlue)), curSelected.Bounds);
                    }
                    if (curSelected.Type != RectType.Text) g.FillRectangle(Brushes.WhiteSmoke, curSelected.Bounds.X, curSelected.Bounds.Bottom, curSelected.Bounds.Width, 1);
                }
            }
            //g.DrawString(selectedText, Font, Brushes.Red, 0, 0);
        }

        private void DrawMessage(Graphics graphics, ChatMessage message, RectangleF bounds, float yInput, float height)
        {
            MeasureMessage(graphics, message, bounds, yInput, height, false);
        }

        private void DrawMessagesNew(Graphics g)
        {
            try
            {
                int i = 0;
                RectangleF bounds = g.VisibleClipBounds;
                float bottom = ((viewStart == 0) ? 5 : (15 - _LineSpacing));
                //if (count > 0)
                //{c
                float y = bounds.Bottom - bottom;

                var messages = ChatClient.Messages[Channel.Name, WhisperName, MessageType];
                if (messages != null)
                {
                    if (viewStart == 0 && AutoScroll)
                    {
                        msgCount = messages.Count;
                    }
                    gifVisible = false;
                    ClickableList.Clear();
                    int start = Math.Min(messages.Count - 1, Math.Max(0, (msgCount - viewStart) - 1));
                    for (i = start; i >= 0; i--)
                    {
                        if (isChangingGraphics || ShowEmotes)
                        {
                            return;
                        }
                        float height = MeasureMessage(g, messages[i], bounds);

                        if (y < 0)
                            break;

                        y -= height;
                        if (y > bounds.Bottom)
                            break;

                        swapColor = (i % 2 > 0);
                        DrawMessage(g, messages[i], bounds, y, height);
                    }
                }
                else
                {
                    g.DrawString("<Waiting for Messages>", new Font("Arial", 12), Brushes.LightGray, bounds, centerStrFormat);
                }

                if (FPS <= 100)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Size: " + this.Font.Size.ToString("F1") + "pt");
                    sb.AppendLine("Refresh/s: " + FPS);
                    g.DrawString(sb.ToString(), this.timeFont, Brushes.Red, bounds, infoStrFormat);
                }
            }
            catch
            {
            }
        }

        private float MeasureMessage(Graphics graphics, ChatMessage message, RectangleF bounds, float yInput = 0, float height = 0, bool measure = true)
        {
            float emote_Y_Offset = 6;

            var user = message.User;

            Color userColor = user.GetColor();
            Color msgColor = MSG_Color;
            Color bgColor = swapColor ? UserColors.ChatBackground : UserColors.ChatBackground1;

            FontStyle style = this.font.Style;

            float y = yInput;
            float x = _LinePadding;
            bool isTimeout = false;

            if (!measure)
            {
                _CharHeight = graphics.MeasureText("A", userFont).Height;
            }

            float emoteHeight = _CharHeight * _EmoteSizeFac;
            float badgeHeight = _CharHeight * _BadgeSizeFac;

            #region Style&Font

            if (!measure)
            {
                bool difBG = swapColor;
                if (message.IsType(MsgType.Action))
                {
                    if (user != null && !user.IsEmpty)
                    {
                        msgColor = userColor;
                        difBG = true;
                    }
                }
                else if (message.IsType(MsgType.HL_Messages))
                {
                    bgColor = Color.DarkRed;
                    difBG = true;
                }
                else if (message.IsType(MsgType.UserNotice))
                {
                    bgColor = Color.FromArgb(60, 40, 60);
                    difBG = true;
                }
                if (!message.Timeout.IsEmpty)
                {
                    isTimeout = true;
                    msgColor = TO_Color;
                    style = TimeOutStyle;// FontStyle.Strikeout;
                }

                using (SolidBrush sb = new SolidBrush(bgColor))
                {
                    graphics.FillRectangle(sb, 0, y - _LineSpacing, bounds.Width, height - (_LineSpacing - 2));
                }
                using (SolidBrush sb = new SolidBrush(UserColors.ChatLine))
                {
                    graphics.FillRectangle(sb, 0, y - (_LineSpacing + 2), bounds.Width, 1);
                }
            }

            #endregion Style&Font

            string time = (ShowName) ? message.SendTime.ToLongTimeString() : message.SendTime.ToShortTimeString();
            SizeF sf = graphics.MeasureText(time, timeFont);
            //list.Add(new DrawItem(time, timeFont, Color.Gray, x, y, sf));
            if (!measure)
            {
                graphics.DrawText(time, timeFont, Color.Gray, x, y + timeSizeFac);
            }

            x += sf.Width + _WordPadding + _TimePadding;

            if (!measure)
            {
                if (UserIsMod && user != null && !user.IsEmpty && user.Types.All(t => ((int)t < (int)UserType.moderator)))
                {
                    internalImages[internImages.Ban].Draw(graphics, x, y, badgeHeight, badgeHeight);
                    ClickableList.Add(new SLRect(x, y, badgeHeight, badgeHeight, user.Name, RectType.ModActionBan));
                    x += badgeHeight + _BadgePadding;

                    internalImages[internImages.Timeout].Draw(graphics, x, y, badgeHeight, badgeHeight);
                    ClickableList.Add(new SLRect(x, y, badgeHeight, badgeHeight, user.Name, RectType.ModActionTimeout));
                    x += badgeHeight + _BadgePadding;
                }
            }

            if (ShowName)
            {
                #region User

                if (user != null && !user.IsEmpty)
                {
                    #region Whisper

                    if (message.IsType(MsgType.Whisper))
                    {
                        ChatUser first = user;
                        ChatUser second = ChatClient.Users.Self;
                        if (message.IsType(MsgType.Outgoing))
                        {
                            first = second;
                            second = ChatClient.Users.Get(message.Channel, "");
                        }

                        Color firstColor = first.GetColor();
                        sf = graphics.MeasureText(first.DisplayName + " > ", userFont);

                        if (!measure)
                        {
                            ClickableList.Add(new SLRect(new RectangleF(x, y, sf.Width, sf.Height), first, RectType.User));

                            graphics.DrawText(first.DisplayName + " > ", userFont, firstColor, x, y);
                        }

                        x += sf.Width;

                        Color secondColor = second.GetColor();
                        sf = graphics.MeasureText(second.DisplayName + ": ", userFont);
                        //list.Add(new DrawItem(second.DisplayName + ":", userFont, secondColor, x, y, sf));
                        if (!measure)
                        {
                            ClickableList.Add(new SLRect(new RectangleF(x, y, sf.Width, sf.Height), second, RectType.User));

                            graphics.DrawText(second.DisplayName + ": ", userFont, secondColor, x, y);
                        }

                        x += sf.Width;
                    }

                    #endregion Whisper

                    else
                    {
                        foreach (var bt in user.Badges)
                        {
                            if (bt.Origin == BadgeOrigin.FFZ_AP)
                            {
                            }
                            var badge = ChatClient.Emotes.Badges[bt];
                            if (badge != null)
                            {
                                if (!measure)
                                {
                                    ClickableList.Add(new SLRect(x, y, badgeHeight, badgeHeight, bt, RectType.Badge));

                                    badge.Draw(bt, graphics, x, y, badgeHeight, badgeHeight);
                                }
                                x += badgeHeight + _BadgePadding;
                            }
                        }
                        x += _BadgePadding;
                        sf = graphics.MeasureText(user.DisplayName + ": ", userFont);

                        if (!measure)
                        {
                            ClickableList.Add(new SLRect(x, y, sf.Width, sf.Height, user, RectType.User));
                            graphics.DrawText(user.DisplayName + ": ", userFont, userColor, x, y);
                        }
                        x += sf.Width;
                    }
                }

                #endregion User
            }
            Font font = new Font(this.font, style);
            float lineSpace = _LineSpacing;
            Color msg_Color = msgColor;
            foreach (var w in message.ChatWords)
            {
                bool isLink = false;
                msgColor = msg_Color;

                if (w.IsEmote)
                {
                    Emote em = w.Emote;

                    sf = em.Image.CalcSize(emoteHeight);
                    lineSpace = _LineSpacing * 2f;
                    if (x + sf.Width > bounds.Right)
                    {
                        x = _LinePadding;
                        y += (sf.Height + lineSpace) - emote_Y_Offset;
                    }

                    if (!measure)
                    {
                        ClickableList.Add(new SLRect(x, y, sf.Width, sf.Height, em, RectType.Emote));

                        var gif = em.Image.Draw(graphics, x, y - emote_Y_Offset, sf.Width, sf.Height, isTimeout) == EmoteImageDrawResult.IsGif;
                        if (gif && !measure) gifVisible = true;
                    }
                    x += sf.Width + _EmotePadding + _WordPadding;
                }
                else
                {
                    if (message.Timeout.IsEmpty && (w.Text.Contains('.') && !w.Text.Contains("..")
                        && reg.IsMatch(w.Text)))
                    {
                        msgColor = Link_Color;
                        isLink = true;
                    }

                    sf = graphics.MeasureString(w.Text, userFont);
                    if (x + sf.Width > bounds.Right)
                    {
                        x = _LinePadding;
                        y += _CharHeight + lineSpace;
                    }

                    if (!measure)
                    {
                        ClickableList.Add(new SLRect(x, y, sf.Width, sf.Height, w, (isLink ? RectType.Link : RectType.Text)));

                        graphics.DrawText(w.Text, font, msgColor, x, y);
                    }
                    x += sf.Width - _WordPadding;
                }
            }
            font.Dispose();
            //graphics.DrawLine(Pens.DarkGray, 0, y, bounds.Width, y);
            return (y == 0) ? _CharHeight + lineSpace + _LineSpacing : (y + lineSpace + _CharHeight) + _LineSpacing;
        }
    }
}

//}

namespace LX29_ChatClient.Forms
{
    public enum RectType
    {
        Link,
        Emote,
        Badge,
        User,
        Delegate,
        Text,
        ModActionBan,
        ModActionTimeout,
        //Word = Name | Link,
        //Image = Badge | Emote
    }

    public struct SLRect
    {
        public readonly static SLRect Empty = new SLRect(0, 0, 0, 0, null, RectType.User);
        public readonly RectangleF Bounds;
        public readonly object Content;
        public readonly RectType Type;

        public SLRect(float x, float y, float w, float h, object content, RectType type)
        {
            Bounds = new RectangleF(x, y, w, h);
            Content = content;
            Type = type;
        }

        public SLRect(RectangleF rect, object content, RectType type)
            : this(rect.X, rect.Y, rect.Width, rect.Height, content, type)
        {
        }

        public bool IsEmpty
        {
            get { return Bounds.IsEmpty; }
        }

        public static implicit operator RectangleF(SLRect r)
        {
            return r.Bounds;
        }

        public bool Contains(Point p)
        {
            return Bounds.Contains(p);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public void Invoke()
        {
            if (Type == RectType.Delegate && Content != null)
            {
                ((Action)Content).Invoke();
            }
        }
    }

    //public struct DrawRect
    //{
    //    public readonly RectangleF Bounds;
    //    public readonly Image Image;
    //    public readonly string Text;
    //    public readonly Font Font;
    //    public readonly Brush Brush;
    //    public readonly RectType Type;
    //    public readonly SizeF Size;

    //    public DrawRect(float x, float y, float width, float height, Image image, SizeF size)
    //    {
    //        Bounds = new RectangleF(x, y, width, height);
    //        Image = image;
    //        Text = null;
    //        Font = null;
    //        Brush = null;
    //        Size = size;
    //        Type = RectType.Image;
    //    }

    //    public DrawRect(RectangleF bounds, Image image, SizeF size) :
    //        this(bounds.X, bounds.Y, bounds.Width, bounds.Height, image, size) { }

    //    public DrawRect(string text, float textHeight, Font font, Brush brush, RectangleF bounds) :
    //        this(text, textHeight, font, brush, bounds.X, bounds.Y, bounds.Width, bounds.Height) { }

    //    public DrawRect(string text, float textHeight, Font font, Brush brush, float x, float y, float width, float height)
    //    {
    //        Bounds = new RectangleF(x, y, width, height);
    //        Image = null;
    //        Size = new SizeF(0, textHeight);
    //        Text = text;
    //        Font = font;
    //        Brush = brush;
    //        Type = RectType.Word;
    //    }

    //    //public void Draw(Graphics g, float xOffset, float yOffset)
    //    //{
    //    //    if (Type == RectType.Word)
    //    //    {
    //    //        g.DrawCenteredSrting(Text, Font, Brush, xOffset + Bounds.X, yOffset + Bounds.Y, Bounds.Height, Size.Height);
    //    //    }
    //    //    else if (Type == RectType.Image)
    //    //    {
    //    //        //gBuffer.DrawCenteredImage(em.Image, x, y, lineHeight, emS.Width, emS.Height);
    //    //        g.DrawCenteredImage(Image, xOffset + Bounds.X, yOffset + Bounds.Y, Bounds.Height, Size.Width, Size.Height);
    //    //    }
    //    //}
    //}
    //public static class FontSize
    //{
    //    public static readonly decimal FontBaseSize = 12.0m;

    //    public static int GetFontPercent(decimal Size)
    //    {
    //        return (int)((Size / FontBaseSize) * 100m);
    //    }

    //    public static int GetFontPercent(Font Font)
    //    {
    //        return (int)(((decimal)Font.Size / FontBaseSize) * 100m);
    //    }
    //}
}