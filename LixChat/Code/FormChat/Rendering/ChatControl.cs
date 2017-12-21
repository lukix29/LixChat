using LX29_ChatClient.Emotes;
using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class ChatView : UserControl
    {
        public readonly RenderDevice Renderer;

        //public readonly Scroller Scrollbar;
        private bool _isscrolldownvisible = false;

        private int _lastY = 0;
        private ApiResult channel;

        //private bool loopRunning = false;
        private Keys modifier_Key = Keys.None;

        //private Point mouseLocation = Point.Empty;
        private bool onMouseDown = false;

        private string selectedText = "";

        //private int yAcc = 0;

        //private double wait = 30;

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

                //Microsoft.Xna.Framework.Control.GraphicsDeviceControl gdc = new Microsoft.Xna.Framework.Control.GraphicsDeviceControl();
                //gdc.Paint += Gdc_Paint;
                //gdc.AutoDraw = true;
                //gdc.Dock = DockStyle.Fill;
                //this.Controls.Add(gdc);

                this.tSMi_Copy.Click += tSMi_Copy_Click;
                this.tSMi_Search.Click += tSMi_Search_Click;
                this.MouseWheel += ChatView_MouseWheel;
                //Scrollbar = new Scroller(this);

                Renderer = new RenderDevice(this);

                Renderer.OnShowScrollDownLabel += Renderer_OnShowScrollDownLabel;
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

        public event UserNameClicked OnUserNameClicked;

        //public event MessageReceived OnMessageReceived;
        [ReadOnly(true)]
        [Browsable(false)]
        public ApiResult Channel
        {
            get { return channel; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public new Font Font
        {
            get { return Renderer.Font; }
            set { Renderer.Font = value; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public bool Pause
        {
            get { return Renderer.Pause; }
            set { Renderer.Pause = value; }
        }

        //[ReadOnly(true)]
        //[Browsable(false)]
        //public int MessageCount
        //{
        //    get { return ChatClient.Messages.Count(Channel.Name) - Renderer.ViewStart; }
        //}
        [ReadOnly(true)]
        [Browsable(false)]
        public bool ShowName
        {
            get { return Renderer.ShowName; }
            set { Renderer.ShowName = value; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public string UserMessageName
        {
            get { return Renderer.WhisperName; }
            //set { renderer.UserMessageName = value; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public bool ViewAllEmotes
        {
            get { return Renderer.ShowAllEmotes; }
            set { Renderer.ShowAllEmotes = value; }
        }

        public void RefreshMessages()
        {
            Renderer.MessageReceived();
        }

        public void Renderer_OnShowScrollDownLabel(bool visible)
        {
            if (visible)
            {
                if (!_isscrolldownvisible)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(ShowScrollDownLabel));
                    }
                    else
                    {
                        ShowScrollDownLabel();
                    }
                    _isscrolldownvisible = true;
                }
            }
            else
            {
                if (_isscrolldownvisible)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(HideScrollDownLabel));
                    }
                    else
                    {
                        HideScrollDownLabel();
                    }
                    _isscrolldownvisible = false;
                }
            }
        }

        public void SetAllMessages(MsgType type, ApiResult ci = null, string name = "")
        {
            if (ci != null) Renderer.Channel = ci;
            Renderer.SetAllMessages(type, name);
        }

        public void SetChannel(ApiResult ci, MsgType type, string name = "")
        {
            channel = ci;
            SetAllMessages(type, ci, name);
            Stop();
            if (ci != null)
            {
                ChatClient.TryConnect(ci.ID);
            }
            ChatClient.OnMessageReceived += ChatClient_MessageReceived;
            ChatClient.OnTimeout += ChatClient_UserHasTimeouted;

            ChatClient.Messages.OnWhisperReceived += ChatClient_OnWhisperReceived;

            Renderer.Start();
        }

        public void SetFontSize(bool incement)
        {
            Renderer.SetFontSize(incement);
        }

        public void SetFontSize(decimal value)
        {
            Renderer.SetFontSize(value);
        }

        public void Stop()
        {
            Renderer.IsRunning = false;
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
                Renderer.SetFontSize(e.KeyValue == 187);
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

            var w = Renderer.ScrollRectangle.Width;
            var rect = new Rectangle(this.ClientSize.Width - w,
                w, w, this.ClientSize.Height - Renderer.ScrollRectangle.Height);

            Renderer.MouseDown = true;

            if (!rect.Contains(e.Location))
            {
                Renderer.SelectRect.Location = e.Location;
                onMouseDown = true;

                //renderer.AutoScroll = false;
                Renderer.AutoScroll = false;
                _lastY = e.Y;
            }
            else
            {
                timer1.Enabled = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                //mouseLocation = e.Location;
                if (onMouseDown)
                {
                    int yorig = _lastY - e.Y;
                    if (Math.Abs(yorig) > 25)
                    {
                        Renderer.SoftScroll -= Math.Min(25, yorig);
                        _lastY = e.Y;
                    }
                    else
                    {
                        int x0 = (int)Renderer.SelectRect.X;
                        int x1 = e.X;
                        if (x0 > x1)
                        {
                            //fgh
                            x0 = e.X;
                            x1 = (int)Renderer.SelectRect.X;
                        }
                        Renderer.SelectRect = RectangleF.FromLTRB(x0, Renderer.SelectRect.Y, x1, Renderer.SelectRect.Y + 1);
                    }
                }
                else
                {
                    var rect = new RectangleF(e.X, e.Y, 0, 2);
                    Renderer.SelectRect = rect;

                    var cnt = Renderer.ClickableList.Where(t => t.Bounds.IntersectsWith(rect) && t.Type != RectType.Text).ToList();
                    if (cnt.Count == 0)
                    {
                        Cursor = Cursors.Arrow;
                    }
                    else if (cnt[0].Type != RectType.Text)
                    {
                        Cursor = Cursors.Hand;
                    }
                }
                //try
                //{
                //    SLRect curSelected = Renderer.ClickableList.FirstOrDefault(t => t.Contains(e.Location));

                //    if (curSelected.IsEmpty)
                //    {
                //        Cursor = Cursors.Arrow;
                //        //renderer.AutoScroll = true;
                //    }
                //    else if (curSelected.Type != RectType.Text)
                //    {
                //        Cursor = Cursors.Hand;
                //    }
                //    // this.Invalidate();
                //}
                //catch
                //{
                //    Cursor = Cursors.Arrow;
                //    // renderer.AutoScroll = true;
                //}
                //Scrollbar.OnMouseMove(e);}
            }
            catch { }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {
                timer1.Enabled = false;

                List<SLRect> selects = Renderer.ClickableList.Where(t => t.Bounds.IntersectsWith(Renderer.SelectRect)).ToList();

                if (selects.Count > 1)
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
                else if (selects.Count > 0)
                {
                    CheckClick(e.Location, selects[0]);
                }
                Renderer.SelectRect = Rectangle.Empty;
            }
            catch { }
            //Scrollbar.OnMouseUp(e);
            onMouseDown = false;
            Renderer.MouseDown = false;
            Renderer.AutoScroll = true;
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //if (!renderer.gifVisible)
            //    renderer.Render(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            if (Renderer != null) Renderer.Invalidate();
            // if (Scrollbar != null) Scrollbar.OnResize(e);
        }

        private void ChatClient_MessageReceived(ChatMessage message)
        {
            if (!this.Visible) return;

            if (message.Channel_Name.Equals(channel.Name))
            {
                Renderer.MessageReceived();
            }
            //{
            //    renderer.SetAllMessages(renderer.MessageType, "");
            //}

            //if (OnMessageReceived != null)
            //    OnMessageReceived(this, message);
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
            Renderer.SelectRect = Rectangle.Empty;
            onMouseDown = false;
        }

        private void ChatView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (modifier_Key == Keys.Control)
            {
                Renderer.SetFontSize(e.Delta > 0);
            }
            else
            {
                Renderer.Scroll(e.Delta);
                //Scrollbar.OnMouseWheel(e);
                if (Renderer.SoftScroll > 0)
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

        private void CheckClick(Point Location, SLRect curSelected)
        {
            // SLRect curSelected = Renderer.ClickableList.FirstOrDefault(t => t.Bounds.Contains(Location));
            if (!curSelected.IsEmpty)
            {
                switch (curSelected.Type)
                {
                    case RectType.ModActionBan:
                        {
                            string name = curSelected.Content.ToString();
                            ChatClient.SendMessage("/ban " + name, Channel.ID);
                        }
                        break;

                    case RectType.ModActionTimeout:
                        {
                            string name = curSelected.Content.ToString();
                            ChatClient.SendMessage("/timeout " + name, Channel.ID);
                        }
                        break;

                    case RectType.Link:
                        OnLinkClicked?.Invoke(this, curSelected.Content.ToString());
                        break;

                    case RectType.Emote:
                        Emote em = (Emote)curSelected.Content;
                        OnEmoteClicked?.Invoke(this, em);
                        break;

                    case RectType.User:
                        ChatUser cu = (ChatUser)curSelected.Content;
                        OnUserNameClicked?.Invoke(this, cu, Location);
                        break;

                    case RectType.Badge:
                        Badge bd = (Badge)curSelected.Content;
                        OnBadgeClicked?.Invoke(this, bd);
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

        private void Gdc_Paint(object sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HideScrollDownLabel()
        {
            lbl_ScrollDown.Visible = false;
        }

        private void lbl_ScrollDown_Click(object sender, EventArgs e)

        {
            if (lbl_ScrollDown.Visible)
            {
                Renderer.SoftScroll = 0;
                HideScrollDownLabel();
            }
        }

        //private Renderer renderer;
        //private async void RefreshLoop()
        //{
        //    var watch = new System.Diagnostics.Stopwatch();

        //    while (!this.IsDisposed)
        //    {
        //        if (this.IsDisposed)
        //            break;

        //        watch.Restart();
        //        try
        //        {
        //            if (!Pause)
        //            {
        //                if (!Renderer.gifVisible)
        //                {
        //                    wait = 1000;
        //                }
        //                else
        //                {
        //                    wait = 30;
        //                }
        //                if (Renderer.Render())
        //                {
        //                    if (!_isscrolldownvisible)
        //                    {
        //                        if (this.InvokeRequired)
        //                        {
        //                            this.Invoke(new Action(ShowScrollDownLabel));
        //                        }
        //                        else
        //                        {
        //                            ShowScrollDownLabel();
        //                        }
        //                        _isscrolldownvisible = true;
        //                    }
        //                }
        //                else
        //                {
        //                    if (_isscrolldownvisible)
        //                    {
        //                        if (this.InvokeRequired)
        //                        {
        //                            this.Invoke(new Action(HideScrollDownLabel));
        //                        }
        //                        else
        //                        {
        //                            HideScrollDownLabel();
        //                        }
        //                        _isscrolldownvisible = false;
        //                    }
        //                }
        //            }
        //            else wait = 1000;
        //            if (watch.ElapsedMilliseconds < wait)
        //            {
        //                await Task.Delay((int)Math.Max(0, Math.Min(10000, (wait - watch.ElapsedMilliseconds))));
        //            }
        //            //dt = DateTime.Now.Ticks;
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        //private void Render()
        //{
        //    if (!Pause)
        //    {
        //        if (!Renderer.gifVisible)
        //        {
        //            wait = 1000;
        //        }
        //        else
        //        {
        //            wait = 30;
        //        }
        //        if (Renderer.Render())
        //        {
        //            if (!_isscrolldownvisible)
        //            {
        //                if (this.InvokeRequired)
        //                {
        //                    this.Invoke(new Action(ShowScrollDownLabel));
        //                }
        //                else
        //                {
        //                    ShowScrollDownLabel();
        //                }
        //                _isscrolldownvisible = true;
        //            }
        //        }
        //        else
        //        {
        //            if (_isscrolldownvisible)
        //            {
        //                if (this.InvokeRequired)
        //                {
        //                    this.Invoke(new Action(HideScrollDownLabel));
        //                }
        //                else
        //                {
        //                    HideScrollDownLabel();
        //                }
        //                _isscrolldownvisible = false;
        //            }
        //        }
        //    }
        //    else wait = 1000;
        //}

        private void ShowScrollDownLabel()
        {
            lbl_ScrollDown.Visible = true;
        }

        private void timer1_Tick(object sender, EventArgs e)

        {
            var w = Renderer.ScrollRectangle.Width;
            var rect = new Rectangle(this.ClientSize.Width - w,
                w, w, this.ClientSize.Height - Renderer.ScrollRectangle.Height);

            var po = this.PointToClient(Cursor.Position);

            if (rect.Contains(po))
            {
                Renderer.SoftScroll = LXMath.Map(po.Y, rect.Bottom, rect.Y, 0, Renderer.SoftScrollMax);
            }
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