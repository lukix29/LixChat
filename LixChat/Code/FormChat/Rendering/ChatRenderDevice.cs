﻿using LX29_ChatClient.Emotes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        public bool ShowName { get; set; } = true;

        private readonly object drawLock = new object();
        private bool _showAllEmotes = false;

        private int emoteDlCnt = 0;

        private int emoteDlCntMax = 0;

        private DateTime emoteDLTime = DateTime.UtcNow;

        //private float emoteDrawMax = -600;

        //private float emoteDrawStart = 0;

        //private List<ChatMessage> messages = new List<ChatMessage>();
        //private bool swapColor = true;

        private IOrderedEnumerable<IEmoteBase> tempEmotes = null;

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

            centerStrFormat = new StringFormat(infoStrFormat)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            VcenterStrFormat = new StringFormat(centerStrFormat)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };

            Font = new System.Drawing.Font("Calibri", 12f);
            //FontBaseSize = Font.Size;

            MessageType = MsgType.All_Messages;
            AutoScroll = true;
            SoftScrollMax = 10000;
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
                SoftScrollMin = 0;
                SoftScrollMax = 10000;
                SoftScroll = value ? (int)DrawBounds.Bottom : 0;
                wait = 1;
            }
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
                    wait = 1;
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
                Messages = ChatClient.Messages.GetMessages(Channel.Name, WhisperName, MessageType, msgcnt - 256, msgcnt);// - ViewStart);
                wait = 1;
                //DrawMessagesNew();
                //bufferedGraphics.Render();
            }
        }

        //  private bool isrenderingasync = false;

        //public async void _RenderAsync()
        //{
        //    if (!isrenderingasync)
        //    {
        //        isrenderingasync = true;
        //        await Task.Run(() => Render());
        //        isrenderingasync = false;
        //    }
        //}

        private int wait = 30;

        public delegate void ShowScrollDownLabel(bool visible);

        public event ShowScrollDownLabel OnShowScrollDownLabel;

        private bool isRunning = false;

        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }

        //private Renderer renderer;
        public void Start()
        {
            if (!isRunning)
            {
                isRunning = true;
                wait = 1;
                Task.Factory.StartNew(() => _refreshLoop(), TaskCreationOptions.LongRunning);
            }
        }

        public void _refreshLoop()
        {
            var watch = new System.Diagnostics.Stopwatch();
#if DEBUG
            var fpswatch = new System.Diagnostics.Stopwatch();
            fpswatch.Start();
            int fpscnt = 0;
#endif
            try
            {
                while (isRunning)
                {
                    watch.Restart();
                    try
                    {
                        if (!Pause)
                        {
                            wait = 32;

                            bool visible = Render();

                            OnShowScrollDownLabel?.Invoke(visible);
                        }
                        else wait = 1000;

                        if (!isRunning)
                            break;
#if DEBUG
                        fpscnt++;
                        if (fpswatch.ElapsedMilliseconds >= 1000)
                        {
                            FPS = fpscnt;
                            fpscnt = 0;

                            fpswatch.Restart();
                        }
#endif
                        var elap = watch.ElapsedMilliseconds;
                        if (elap < wait)
                        {
                            System.Threading.Thread.Sleep((int)Math.Max(1, Math.Min(1000, (wait - elap))));
                        }
                    }
                    catch
                    {
                    }
                }
            }
            finally
            {
                watch.Stop();
                fpswatch.Stop();
            }
        }

        public bool MouseDown
        {
            get;
            set;
        }

        public RectangleF DrawBounds
        {
            get;
            private set;
        }

        public bool Render()
        {
            try
            {
                lock (drawLock)
                {
                    //var w = ScrollRectangle.Width;
                    //var rect = new Rectangle((int)bufferedGraphics.Graphics.VisibleClipBounds.Width - w,
                    //    w, w, (int)bufferedGraphics.Graphics.VisibleClipBounds.Height - ScrollRectangle.Height);

                    //var po = new Control().PointToClient(Cursor.Position);
                    //if (rect.Contains(po))
                    //{
                    //    int ey = po.Y;
                    //    SoftScroll = LXMath.Map(ey, rect.Bottom, rect.Y, 0, SoftScrollMax);
                    //}
                    DrawBounds = bufferedGraphics.Graphics.VisibleClipBounds;
                    bufferedGraphics.Graphics.Clear(UserColors.ChatBackground);
                    if (ShowAllEmotes)
                    {
                        DrawEmotes();
                    }
                    else
                    {
                        DrawMessagesNew();
                    }
                    DrawInfoOverlay();

#if DEBUG
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Size: " + this.Font.Size.ToString("F1") + "pt");
                    sb.AppendLine("Refresh/s: " + FPS);
                    sb.AppendLine("V-Msg: " + VisibleMessages);
                    sb.AppendLine("Scroll: " + SoftScroll);
                    sb.AppendLine("ScrMax: " + SoftScrollMax);
                    var str = sb.ToString();
                    SizeF sizeF = bufferedGraphics.Graphics.MeasureString(str, this.timeFont);
                    bufferedGraphics.Graphics.FillRectangle(Brushes.Black, new RectangleF(DrawBounds.Width - sizeF.Width, 0, sizeF.Width, sizeF.Height));
                    bufferedGraphics.Graphics.DrawString(sb.ToString(), this.timeFont, Brushes.Red, DrawBounds, infoStrFormat);
#endif

                    //  control.Scrollbar.OnPaint(g);
                    bufferedGraphics.Render();

                    if (_softScroll > 0)
                    {
                        AutoScroll = false;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public void Scroll(int delta)
        {
            //if (!_showAllEmotes)
            //{
            int d = (int)(delta / (float)Settings.ScrollFactor);

            SoftScroll += d;
            wait = 1;
            // }
            //else
            //{
            //    emoteDrawStart = Math.Max(-600, Math.Min(0, emoteDrawStart + (delta / 10)));
            //}
        }

        public void SetAllMessages(MsgType Type, string name = null)
        {
            _showAllEmotes = false;
            WhisperName = name;
            MessageType = Type;
            var user = ChatClient.ChatUsers.Get(ChatClient.SelfUserName, Channel.Name);
            if (user != null && !user.IsEmpty)
            {
                UserIsMod = user.Types.Any(t => ((int)t >= (int)UserType.moderator));
            }
            wait = 1;
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

        private void DrawEmotes()
        {
            try
            {
                Graphics g = bufferedGraphics.Graphics;

                RectangleF bounds = g.VisibleClipBounds;

                SoftScrollMin = (int)bounds.Bottom - 10;
                if (!ChatClient.Emotes.Finished)
                {
                    g.DrawText("Loading Emotes", this.font, Color.White, bounds, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    return;
                }
                float x = _EmotePadding;
                float y = (bounds.Bottom - _softScroll) - _EmotePadding;
                if (y > 10) y = 10;

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

                tempList.Clear();

                foreach (var em in tempEmotes)
                {
                    size = em.CalcSize(32);
                    if (!lastChannel.Equals(em.Channel))
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
                    var result = em.Draw(g, x, y, size.Width, size.Height, false);
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

                    tempList.Add(new SLRect(x, y, size.Width, size.Height, em, RectType.Emote));

                    x += size.Width + _EmotePadding;
                }
                ClickableList.RePopulate(tempList);
                //SoftScrollMax = (int)((y + size.Height + _EmotePadding) * 0.5f);
            }
            catch
            {
            }
        }

        private void DrawInfoOverlay()
        {
            Graphics g = bufferedGraphics.Graphics;

            IEmoteBase emote = null;
            float width = g.VisibleClipBounds.Width;
            float Height = g.VisibleClipBounds.Height;
            List<SLRect> selects = ClickableList.Where(t => t.Bounds.IntersectsWith(SelectRect)).ToList();

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
                            emote = (IEmoteBase)curSelected.Content;
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
                            emote.Draw(g, bounds.X + emSiz / 4f, bounds.Y + txtSize.Height, emSiz, emSiz, false);
                        }
                    }
                    if (SelectRect.Width > 0)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.LightSteelBlue)), curSelected.Bounds);
                    }
                    if (curSelected.Type != RectType.Text) g.FillRectangle(Brushes.WhiteSmoke, curSelected.Bounds.X, curSelected.Bounds.Bottom, curSelected.Bounds.Width, 1);

                    g.DrawRectangle(Pens.Red, curSelected.Bounds.X, curSelected.Bounds.Y, curSelected.Bounds.Width, curSelected.Bounds.Height);
                }
            }
            //g.DrawString(selectedText, Font, Brushes.Red, 0, 0);
        }

        private void DrawMessage(Graphics graphics, ChatMessage message, RectangleF bounds, float yInput, float height, bool swapColor)
        {
            MeasureMessage(graphics, message, bounds, swapColor, yInput, height, false);
        }

        public int SoftScrollMax { get; private set; } = 10000;
        public int SoftScrollMin { get; private set; } = 0;

        public int SoftScroll
        {
            get { return _softScroll; }
            set
            {
                if (Channel != null)
                {
                    //int msgcnt = ChatClient.Messages.Count(Channel.Name, MessageType, WhisperName);
                    _softScroll = Math.Max(SoftScrollMin, Math.Min(SoftScrollMax * 10, value));
                    AutoScroll = (_softScroll > 0) ? false : true;
                    if (_softScroll == 0)
                    {
                        SoftScrollMax = 10000;
                    }
                    wait = 1;
                    // MessageReceived(true);
                }
            }
        }

        public Size ScrollRectangle
        {
            get
            {
                return new Size(10, ((_softScroll == 0) ? 5 : (40 - (int)_LineSpacing)));
            }
        }

        private struct msgDraw

        {
            public ChatMessage msg;
            public float height;
            public float y;
            public bool swap;
        }

        private void DrawMessagesNew()
        {
            try
            {
                //Gtk.DotNet.Graphics.
                Graphics g = bufferedGraphics.Graphics;

                RectangleF bounds = g.VisibleClipBounds;
                float bottom = ((_softScroll == 0) ? 5 : (40 - _LineSpacing));
                //if (count > 0)
                //{c
                float y = (bounds.Bottom - bottom) + SoftScroll;
                //var messages = ChatClient.Messages.GetMessages(Channel.Name, WhisperName, MessageType);
                if (Messages != null && Messages.Count > 0)
                {
                    //if (viewStart == 0 && AutoScroll)
                    //{
                    //    //msgCount = messages.Count;
                    //}
                    gifVisible = false;
                    tempList.Clear();
                    int start = Messages.Count - 1;
                    //float height = 0;
                    List<msgDraw> list = new List<msgDraw>();
                    for (int i = start; i >= 0; i--)
                    {
                        var msg = Messages[i];
                        float height = MeasureMessage(g, msg, bounds);

                        if (y < 0)
                            break;
                        y -= height;
                        if (y >= bounds.Bottom)
                            continue;

                        list.Add(new msgDraw { height = height, msg = msg, y = y, swap = (i % 2 > 0) });
                    }
                    VisibleMessages = list.Count;
                    for (int i = 0; i < VisibleMessages; i++)
                    {
                        if (isChangingGraphics > 0 || ShowAllEmotes)
                        {
                            return;
                        }
                        var item = list[i];

                        DrawMessage(g, item.msg, bounds, item.y, item.height, item.swap);
                    }
                    ClickableList.RePopulate(tempList);
                    if (!MouseDown)
                    {
                        if (y >= bounds.Height - bottom)
                        {
                            SoftScroll -= (int)(50);
                            SoftScrollMax = SoftScroll;
                        }
                    }

                    //e.Graphics.DrawRectangle(Pens.DarkGray, );
                    float ys = LXMath.Map(SoftScroll, 0, SoftScrollMax, ScrollRectangle.Width, bounds.Height - ScrollRectangle.Width);
                    g.FillRectangle(Brushes.DarkGray, bounds.Right - ScrollRectangle.Width, Math.Max(0, (bounds.Height - bottom) - ys),
                        ScrollRectangle.Width, ScrollRectangle.Width * 2);
                }
                else
                {
                    MessageReceived();
                    if (Messages == null)
                    {
                        g.DrawString("<Waiting for Messages>", new Font("Arial", 12), Brushes.LightGray, bounds, centerStrFormat);
                    }
                }
            }
            catch
            {
            }
        }

        private float MeasureMessage(Graphics graphics, ChatMessage message, RectangleF bounds, bool swapColor = false, float yInput = 0, float height = 0, bool measure = true)
        {
            bool drawImages = true;
            bool alignText = Settings.AlignText;
            float emote_Y_Offset = 4;
            //gifVisible = true;
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
                    internalImages[internImages.Ban].Draw(graphics, x, y, badgeHeight, badgeHeight);
                    tempList.Add(new SLRect(x, y, badgeHeight, badgeHeight, user.Name, RectType.ModActionBan));
                    x += badgeHeight + _BadgePadding;

                    internalImages[internImages.Timeout].Draw(graphics, x, y, badgeHeight, badgeHeight);
                    tempList.Add(new SLRect(x, y, badgeHeight, badgeHeight, user.Name, RectType.ModActionTimeout));
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
                        ChatUser second = ChatClient.ChatUsers.Self;
                        if (message.IsType(MsgType.Outgoing))
                        {
                            first = second;
                            second = ChatClient.ChatUsers.Get(message.Channel_Name, "");
                        }

                        Color firstColor = first.GetColor();
                        sf = graphics.MeasureText(first.DisplayName + " > ", userFont);

                        if (!measure)
                        {
                            tempList.Add(new SLRect(new RectangleF(x, y, sf.Width, sf.Height), first, RectType.User));

                            graphics.DrawText(first.DisplayName + " > ", userFont, firstColor, x, y);
                        }

                        x += sf.Width;

                        Color secondColor = second.GetColor();
                        sf = graphics.MeasureText(second.DisplayName + ": ", userFont);
                        //list.Add(new DrawItem(second.DisplayName + ":", userFont, secondColor, x, y, sf));
                        if (!measure)
                        {
                            tempList.Add(new SLRect(new RectangleF(x, y, sf.Width, sf.Height), second, RectType.User));

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
                                        tempList.Add(new SLRect(x, y, badgeHeight, badgeHeight, bt, RectType.Badge));

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
                            tempList.Add(new SLRect(x, y, sf.Width, sf.Height, user, RectType.User));
                            graphics.DrawText(user.DisplayName + ": ", userFont, userColor, x, y);
                        }
                        x += sf.Width + _WordPadding;
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
                    foreach (IEmoteBase em in w.Emote)
                    {
                        sf = em.CalcSize(emoteHeight);
                        lineSpace = sf.Height / 2 - _LineSpacing;
                        if (x + sf.Width > bounds.Right)
                        {
                            x = alignText ? time_Right : (Math.Max(0, _WordPadding) + _TimePadding);//Linepadding
                            y += (sf.Height + lineSpace) - emote_Y_Offset;
                        }

                        if (!measure)
                        {
                            tempList.Add(new SLRect(x, y - emote_Y_Offset, sf.Width, sf.Height, em, RectType.Emote));

                            var gif = em.Draw(graphics, x, y - emote_Y_Offset, sf.Width, sf.Height, isTimeout) == EmoteImageDrawResult.IsGif;
                            if (gif && !measure) gifVisible = true;
                        }
                        x += sf.Width + _EmotePadding + Math.Max(0, _WordPadding);
                    }
                }
                else
                {
                    if (message.Timeout.IsEmpty && (w.Text.Contains('.') && !w.Text.Contains("..")
                        && reg.IsMatch(w.Text)))
                    {
                        msgColor = Link_Color1;
                        isLink = true;
                    }
                    sf = graphics.MeasureText(w.Text, userFont);
                    if (x + sf.Width > bounds.Right)
                    {
                        x = alignText ? time_Right : (Math.Max(0, _WordPadding) + _TimePadding);//Linepadding
                        y += _CharHeight + lineSpace;
                    }

                    if (!measure)
                    {
                        tempList.Add(new SLRect(x, y, sf.Width, sf.Height, w, (isLink ? RectType.Link : RectType.Text)));

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
        public LX29_Twitch.Api.ApiResult Channel;

        //public readonly object ClickableListLock = new object();
        public SafeList<SLRect> ClickableList = new SafeList<SLRect>();

        private List<SLRect> tempList = new List<SLRect>();
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

        //private long dtFPS_Lock = DateTime.Now.Ticks;
        private Font font = new Font("Arial", 12f);

        private int FPS = 0;
        //private int fpscnt = 0;

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

        private int _softScroll = 0;

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

        public MsgType MessageType
        {
            get;
            private set;
        }

        public bool Pause
        {
            get;
            set;
        }

        public string WhisperName
        {
            get;
            set;
        }

        public List<ChatMessage> Messages { get; private set; }
        public Color Link_Color1 { get => Color.DodgerBlue; }
        public int VisibleMessages { get; private set; }

        #endregion Properties
    }
}