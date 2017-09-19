using LX29_ChatClient.Channels;
using LX29_ChatClient.Emotes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

    public class RenderDevice
    {
        //public int msgCount = 0;
        public bool ShowName = true;

        private bool _showAllEmotes = false;

        private int emoteDlCnt = 0;

        private int emoteDlCntMax = 0;

        private DateTime emoteDLTime = DateTime.UtcNow;

        private float emoteDrawMax = -600;

        private float emoteDrawStart = 0;

        private Color Link_Color = Color.DodgerBlue;
        private List<ChatMessage> messages = new List<ChatMessage>();
        private bool swapColor = true;

        private IOrderedEnumerable<EmoteBase> tempEmotes = null;

        private int visibleMessages = 0;
        private readonly object drawLock = new object();

        public RenderDevice(ChatView form)
        {
            control = form;

            internalImages.Add(internImages.Ban,
                new EmoteImage("Ban", LX29_LixChat.Properties.Resources.ban));
            internalImages.Add(internImages.Timeout,
               new EmoteImage("Timeout", LX29_LixChat.Properties.Resources.timeout));
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

        public bool ShowAllEmotes
        {
            get { return _showAllEmotes; }
            set
            {
                _showAllEmotes = value;
                tempEmotes = null;
                ClickableList.Clear();
            }
        }

        public int VisibleMessages
        {
            get { return visibleMessages; }
        }

        private bool alternateBG
        {
            get { return Settings.AlternateBG; }
        }

        private bool ShowTimeoutMessages
        {
            get
            {
                return Settings.ShowTimeoutMessages;
            }
        }

        private bool ShowTimeStamp
        {
            get
            {
                return Settings.ShowTimeStamp;
            }
        }

        public void Invalidate()
        {
            try
            {
                if (isChangingGraphics > 0)
                    return;
                isChangingGraphics++;
                lock (drawLock)
                {
                    if (bufferedGraphics != null) bufferedGraphics.Dispose();
                    bufferedGraphicsContex = BufferedGraphicsManager.Current;
                    bufferedGraphics = bufferedGraphicsContex.Allocate(control.CreateGraphics(), control.ClientRectangle);
                    bufferedGraphics.Graphics.SetGraphicQuality(true, true);
                }
            }
            catch { }
            isChangingGraphics = Math.Max(0, isChangingGraphics - 1);
        }

        public void MessageReceived(bool force = false)
        {
            if (AutoScroll || force)
            {
                int msgcnt = ChatClient.Messages.Count(Channel.Name, MessageType, WhisperName);
                messages = ChatClient.Messages.GetMessages(Channel.Name, WhisperName, MessageType, msgcnt - 256, msgcnt - ViewStart);
            }
        }

        public bool Render()
        {
            Graphics g = bufferedGraphics.Graphics;

            lock (drawLock)
            {
                g.Clear(UserColors.ChatBackground);
                if (ShowAllEmotes)
                {
                    DrawEmotes(g);
                }
                else
                {
                    DrawMessagesNew(g);
                }
                DrawInfoOverlay(g);
                //  control.Scrollbar.OnPaint(g);
                bufferedGraphics.Render();
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
            if (!_showAllEmotes)
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
            _showAllEmotes = false;
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

                if (tempEmotes == null)
                {
                    tempEmotes = ChatClient.Emotes.GetEmotes(Channel.Name)
                        .OrderByDescending(t => t.Origin == EmoteOrigin.Twitch_Global)
                        .ThenByDescending(t => t.Origin == EmoteOrigin.BTTV_Global)
                        .ThenByDescending(t => t.Origin == EmoteOrigin.FFZ_Global)
                        .ThenBy(t => t.Channel);
                }
                string lastChannel = tempEmotes.ElementAt(0).Channel;

                g.DrawString(lastChannel, this.font, Brushes.Gainsboro, x, y);
                y += g.MeasureString(lastChannel, this.font).Height + _EmotePadding;

                gifVisible = false;

                SizeF size = SizeF.Empty;
                ClickableList.Clear();
                foreach (var em in tempEmotes)
                {
                    size = em.CalcSize(32, Settings.EmoteQuality);
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
                    if (!_showAllEmotes) return;
                    var result = em.Draw(g, x, y, size.Width, size.Height, EmoteImageSize.Small, false);
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
            EmoteBase emote = null;
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
                            int msgcnt = ChatClient.Messages.Count(Channel.Name, MsgType.All_Messages, user.Name);
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
                            emote = (EmoteBase)curSelected.Content;
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
                            emote.Draw(g, bounds.X + emSiz / 4f, bounds.Y + txtSize.Height, emSiz, emSiz, EmoteImageSize.Large, false);
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

        private void DrawMessage(Graphics graphics, ChatMessage message, int idx, RectangleF bounds, float yInput, float height)
        {
            MeasureMessage(graphics, message, idx, bounds, yInput, height, false);
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
                //var messages = ChatClient.Messages.GetMessages(Channel.Name, WhisperName, MessageType);
                if (messages != null && messages.Count > 0)
                {
                    //if (viewStart == 0 && AutoScroll)
                    //{
                    //    //msgCount = messages.Count;
                    //}
                    gifVisible = false;
                    ClickableList.Clear();
                    visibleMessages = 0;
                    int start = Math.Min(messages.Count - 1, Math.Max(0, (messages.Count - viewStart) - 1));
                    bool iwasZero = false;
                    for (i = start; i >= 0; i--)
                    {
                        if (isChangingGraphics > 0 || ShowAllEmotes)
                        {
                            return;
                        }
                        float height = MeasureMessage(g, messages[i], i, bounds);
                        if (i == 0)
                        {
                            iwasZero = true;
                        }
                        if (y < 0)
                        {
                            break;
                        }
                        y -= height;
                        if (y > bounds.Bottom)
                            break;

                        swapColor = (i % 2 > 0);
                        DrawMessage(g, messages[i], i, bounds, y, height);
                        visibleMessages++;
                    }
                    //if (iwasZero)
                    //{
                    //    messages = ChatClient.Messages.GetMessages(Channel.Name, WhisperName, MessageType, 0);
                    //}
                }
                else
                {
                    MessageReceived();
                    if (messages == null)
                    {
                        g.DrawString("<Waiting for Messages>", new Font("Arial", 12), Brushes.LightGray, bounds, centerStrFormat);
                    }
                }

                if (FPS <= 100)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Size: " + this.Font.Size.ToString("F1") + "pt");
                    sb.AppendLine("Refresh/s: " + FPS);
                    sb.AppendLine("V-Msg: " + visibleMessages);
                    g.DrawString(sb.ToString(), this.timeFont, Brushes.Red, bounds, infoStrFormat);
                }
            }
            catch
            {
            }
        }

        private float MeasureMessage(Graphics graphics, ChatMessage message, int idx, RectangleF bounds, float yInput = 0, float height = 0, bool measure = true)
        {
            bool drawImages = true;
            float emote_Y_Offset = 4;

            var user = message.User;

            if (!message.Timeout.IsEmpty && !ShowTimeoutMessages && !user.IsEmpty)
            {
                return 0;
            }

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
                else if (message.IsType(MsgType.UserNotice))
                {
                    bgColor = Color.FromArgb(60, 40, 60);
                    difBG = true;
                }
                if (message.IsType(MsgType.HL_Messages))
                {
                    bgColor = Color.DarkRed;
                    difBG = true;
                }
                if (!message.Timeout.IsEmpty)
                {
                    isTimeout = true;
                    msgColor = TO_Color;
                    style = TimeOutStyle;// FontStyle.Strikeout;
                }
                if (alternateBG)
                {
                    using (SolidBrush sb = new SolidBrush(bgColor))
                    {
                        graphics.FillRectangle(sb, 0, y - _LineSpacing, bounds.Width, height - (_LineSpacing - 2));
                    }
                    graphics.FillRectangle(Brushes.Black, 0, y - (_LineSpacing + 3), bounds.Width, 2);
                }
            }

            #endregion Style&Font

            SizeF sf = SizeF.Empty;
            if (Settings.isDebug)
            {
                sf = graphics.MeasureText(idx.ToString(), timeFont);

                if (!measure)
                {
                    graphics.DrawText(idx.ToString(), Font, Color.Red, x, y + timeSizeFac);
                }

                x += sf.Width + _WordPadding + _TimePadding;
            }
            if (ShowTimeStamp)
            {
                string time = (ShowName) ? message.SendTime.ToLongTimeString() : message.SendTime.ToShortTimeString();
                sf = graphics.MeasureText(time, timeFont);

                if (!measure)
                {
                    graphics.DrawText(time, timeFont, Color.Gray, x, y + timeSizeFac);
                }

                x += sf.Width + _WordPadding + _TimePadding;
            }
            if (!measure)
            {
                if (UserIsMod && user != null && !user.IsEmpty && user.Types.All(t => ((int)t < (int)UserType.moderator)))
                {
                    internalImages[internImages.Ban].Draw(graphics, x, y, badgeHeight, badgeHeight, EmoteImageSize.Large);
                    ClickableList.Add(new SLRect(x, y, badgeHeight, badgeHeight, user.Name, RectType.ModActionBan));
                    x += badgeHeight + _BadgePadding;

                    internalImages[internImages.Timeout].Draw(graphics, x, y, badgeHeight, badgeHeight, EmoteImageSize.Large);
                    ClickableList.Add(new SLRect(x, y, badgeHeight, badgeHeight, user.Name, RectType.ModActionTimeout));
                    x += badgeHeight + _BadgePadding;
                }
            }
            float time_Right = x;
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
                        if (drawImages)
                        {
                            foreach (var bt in user.Badges)
                            {
                                var badge = ChatClient.Emotes.Badges[bt];
                                if (badge != null && badge.IsEnabled)
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
                        }
                        time_Right = x;

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

                if (drawImages && w.IsEmote)
                {
                    foreach (EmoteBase em in w.Emote)
                    {
                        sf = em.CalcSize(emoteHeight, Settings.EmoteQuality);
                        lineSpace = sf.Height / 2 - _LineSpacing;
                        if (x + sf.Width > bounds.Right)
                        {
                            x = time_Right;//Linepadding
                            y += (sf.Height + lineSpace) - emote_Y_Offset;
                        }

                        if (!measure)
                        {
                            ClickableList.Add(new SLRect(x, y - emote_Y_Offset, sf.Width, sf.Height, em, RectType.Emote));

                            var gif = em.Draw(graphics, x, y - emote_Y_Offset, sf.Width, sf.Height, Settings.EmoteQuality, isTimeout) == EmoteImageDrawResult.IsGif;
                            if (gif && !measure) gifVisible = true;
                        }
                        x += sf.Width + _EmotePadding + _WordPadding;
                    }
                }
                else
                {
                    if (message.Timeout.IsEmpty && (w.Text.Contains('.') && !w.Text.Contains("..")
                        && reg.IsMatch(w.Text)))
                    {
                        msgColor = Link_Color;
                        isLink = true;
                    }

                    sf = graphics.MeasureText(w.Text, userFont);
                    if (x + sf.Width > bounds.Right)
                    {
                        x = time_Right;
                        y += _CharHeight + lineSpace;
                    }

                    if (!measure)
                    {
                        ClickableList.Add(new SLRect(x, y, sf.Width, sf.Height, w, (isLink ? RectType.Link : RectType.Text)));

                        graphics.DrawText(w.Text, font, msgColor, x, y);
                    }
                    x += sf.Width + _WordPadding;
                }
            }
            font.Dispose();
            //graphics.DrawLine(Pens.DarkGray, 0, y, bounds.Width, y);
            return (y == 0) ? _CharHeight + lineSpace + _LineSpacing : (y + lineSpace + _CharHeight) + _LineSpacing;
        }

        #region Fields

        public bool AutoScroll = true;
        public ChannelInfo Channel;
        public List<SLRect> ClickableList = new List<SLRect>();
        public bool gifVisible = false;
        public RectangleF SelectRect = RectangleF.Empty;
        private const int timeSizeFac = 2;

        //private readonly object drawLock = new object();
        private float _CharHeight = 0f;

        //private Color BG_Color = UserColors.ChatBackground;
        //private Color BG_Color1 = UserColors.ChatBackground1;
        private BufferedGraphics bufferedGraphics = null;

        private BufferedGraphicsContext bufferedGraphicsContex = null;
        private StringFormat centerStrFormat = new StringFormat();
        private ChatView control;
        private long dtFps = DateTime.Now.Ticks;
        private long dtFPS_Lock = DateTime.Now.Ticks;
        private Font font = new Font("Arial", 12f);
        private int FPS = 0;
        private int fpscnt = 0;

        // private Color HL_Color = Color.White;
        private StringFormat infoStrFormat = new StringFormat();

        private Dictionary<internImages, EmoteImage> internalImages = new Dictionary<internImages, EmoteImage>();
        private int isChangingGraphics = 0;

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
                    int msgcnt = ChatClient.Messages.Count(Channel.Name, MessageType, WhisperName);
                    viewStart = Math.Max(0, Math.Min(msgcnt, value));
                    AutoScroll = (viewStart > 0) ? false : true;

                    MessageReceived(true);
                }
            }
        }

        public string WhisperName
        {
            get;
            set;
        }

        #endregion Properties
    }

    //public class Scroller
    //{
    //    private ChatView device;
    //    private bool isMouseDown = false;
    //    private bool isScrolling = false;
    //    private object locko = new object();

    //    private Rectangle scrollRect = new Rectangle();

    //    private bool scrollVisible = false;

    //    public Scroller(ChatView view)
    //    {
    //        device = view;
    //    }

    //    private Rectangle ClientRectangle
    //    {
    //        get { return device.ClientRectangle; }
    //    }

    //    private Size ClientSize
    //    {
    //        get { return ClientRectangle.Size; }
    //    }

    //    private int itemCount
    //    {
    //        get { return device.Renderer.msgCount; }
    //    }

    //    private int maxVisibleItems
    //    {
    //        get { return device.Renderer.VisibleMessages; }
    //    }

    //    private int scrollBarHeight
    //    {
    //        get
    //        {
    //            return 20;
    //        }
    //    }

    //    private int scrollOffset
    //    {
    //        get { return device.Renderer.ViewStart; }
    //        set { device.Renderer.ViewStart = value; }
    //    }

    //    public void OnMouseDown(MouseEventArgs e)
    //    {
    //        if (new Rectangle(scrollRect.X - 5, 0, scrollRect.Width + 10, this.ClientSize.Height).
    //                Contains(e.Location))
    //        {
    //            if (new Rectangle(scrollRect.X - 5, scrollRect.Y, scrollRect.Width + 10, scrollRect.Height).
    //                 Contains(e.Location))
    //            {
    //                isScrolling = true;
    //            }
    //        }
    //        isMouseDown = true;
    //    }

    //    public void OnMouseMove(MouseEventArgs e)
    //    {
    //        if (isMouseDown)
    //        {
    //            if (isScrolling)
    //            {
    //                CalcScrolling(e.Y);
    //            }
    //            else if (!isScrolling)
    //            {
    //                if (e.Y >= this.ClientRectangle.Bottom)
    //                {
    //                    scrollOffset = Math.Min(itemCount - maxVisibleItems, scrollOffset + 1);
    //                }
    //                else if (e.Y <= 0)
    //                {
    //                    scrollOffset = Math.Max(0, scrollOffset - 1);
    //                }
    //            }
    //        }
    //    }

    //    public void OnMouseUp(MouseEventArgs e)
    //    {
    //        if (new Rectangle(scrollRect.X - 5, 0, scrollRect.Width + 10, this.ClientSize.Height).
    //             Contains(e.Location))
    //        {
    //            CalcScrolling(e.Y);
    //        }

    //        isScrolling = false;
    //        isMouseDown = false;
    //    }

    //    public void OnMouseWheel(MouseEventArgs e)
    //    {
    //        //scrollOffset = Math.Max(0, Math.Min(itemCount - maxVisibleItems, scrollOffset - Math.Sign(e.Delta)));

    //        int ys = LXMath.Map(scrollOffset, 0, itemCount - maxVisibleItems, 0, this.ClientSize.Height);
    //        ys = Math.Min(this.ClientRectangle.Bottom - scrollBarHeight, Math.Max(0, ys - scrollBarHeight / 2));
    //        scrollRect = new Rectangle(this.ClientRectangle.Right - 12, ys, 10, scrollBarHeight);
    //    }

    //    public void OnPaint(Graphics g)
    //    {
    //        try
    //        {
    //            // e.Graphics.SetGraphicQuality(true, true);
    //            if (maxVisibleItems == 0)
    //            {
    //                scrollOffset = 0;
    //                scrollVisible = false;
    //            }
    //            else
    //            {
    //                scrollVisible = true;
    //            }
    //            if (scrollVisible)
    //            {
    //                g.DrawRectangle(Pens.DarkGray, new Rectangle(scrollRect.X - 1, 0, scrollRect.Width + 1, this.ClientSize.Height - 1));
    //                g.FillRectangle(Brushes.DarkGray, scrollRect);
    //            }
    //            else
    //            {
    //                isScrolling = false;
    //            }
    //        }
    //        catch (Exception x)
    //        {
    //            //e.Graphics.DrawString(x.Message, this.Font, Brushes.Gainsboro, e.ClipRectangle, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
    //        }
    //    }

    //    public void OnResize(EventArgs e)
    //    {
    //        scrollRect = new Rectangle(this.ClientRectangle.Right - 12, scrollRect.Y, 10, scrollBarHeight);
    //    }

    //    private void CalcScrolling(int Y)
    //    {
    //        int ys = Math.Min(this.ClientRectangle.Bottom - scrollBarHeight, Math.Max(0, Y - scrollBarHeight / 2));
    //        scrollRect = new Rectangle(this.ClientRectangle.Right - 12, ys, 10, scrollBarHeight);

    //        var offset = LXMath.Map(Y, scrollBarHeight / 2,
    //            this.ClientSize.Height - scrollBarHeight / 2, 0, itemCount - maxVisibleItems);
    //        scrollOffset = Math.Max(0, Math.Min(itemCount - maxVisibleItems, offset));
    //    }
    //}

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