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

using Microsoft.VisualBasic;

using System;

using System.Collections;

using System.Collections.Generic;

using System.Data;
using System.Diagnostics;

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
            get { return renderer.ShowAllEmotes; }
            set { renderer.ShowAllEmotes = value; }
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

                        if (!selectedText.IsEmpty())
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
            if (!selectedText.IsEmpty())
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