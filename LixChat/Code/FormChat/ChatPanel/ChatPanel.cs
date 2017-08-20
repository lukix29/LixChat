using LX29_ChatClient.Channels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class ChatPanel : UserControl
    {
        private int lastMessageScrollIndex = -1;

        private string lastSearch = "";

        private bool lockListSearch = false;

        private Dictionary<string, EmoteSearchResult> searchResult = new Dictionary<string, EmoteSearchResult>();

        //private int lastMessageScrollMax = 0;
        //private List<ChatMessage> sentMessages = new List<ChatMessage>();
        public ChatPanel()
        {
            InitializeComponent();
            lstB_Search.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public delegate void WhisperSent(object sender, ChatMessage message);

        public event WhisperSent OnWhisperSent;

        public ChannelInfo Channel
        {
            get { return this.chatView1.Channel; }
        }

        public ChatView ChatView
        {
            get { return this.chatView1; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public bool Pause
        {
            get { return chatView1.Pause; }
            set { chatView1.Pause = value; }
        }

        //public bool ShowInput
        //{
        //    get { return rTB_Send.Visible; }
        //    set
        //    {
        //        rTB_Send.Visible = value;
        //        if (!value)
        //        {
        //            chatView1.Anchor = AnchorStyles.None;
        //            chatView1.Dock = DockStyle.Fill;
        //        }
        //        else
        //        {
        //            chatView1.Dock = DockStyle.None;
        //            chatView1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        //        }
        //    }
        //}

        //public bool SmallInput
        //{
        //    get { return smallInput; }
        //    set
        //    {
        //        smallInput = value;
        //        if (smallInput)
        //        {
        //            rTB_Send.MinimumSize = new Size(50, 25);
        //            rTB_Send.MaximumSize = new Size(Int16.MaxValue, 25);
        //            rTB_Send.Height = 25;
        //        }
        //        else
        //        {
        //            rTB_Send.MinimumSize = new Size(50, 50);
        //            rTB_Send.MaximumSize = new Size(Int16.MaxValue, 50);
        //            rTB_Send.Height = 50;
        //        }
        //    }
        //}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            if (rTB_Send.TextLength > 0)
            {
                var text = rTB_Send.Text;
                rTB_Send.Clear();

                System.Threading.Tasks.Task.Run(() =>
                {
                    var msg = ChatMessage.Empty;
                    if (chatView1.UserMessageName.IsEmpty())
                    {
                        msg = ChatClient.SendMessage(text, Channel.Name);
                    }
                    else
                    {
                        msg = ChatClient.SendWhisper(text, chatView1.UserMessageName);
                    }
                    this.Invoke(new Action(() =>
                    {
                        if (!msg.IsEmpty)
                        {
                            if (msg.IsType(MsgType.Whisper))
                            {
                                if (OnWhisperSent != null)
                                    OnWhisperSent(this, msg);
                            }
                        }
                    }));
                });
            }
        }

        private void ChatHistory(KeyEventArgs e)
        {
            try
            {
                if (!lastSearch.IsEmpty()) return;

                e.SuppressKeyPress = true;

                var sa = ChatClient.Messages.Get(MsgType.Outgoing, Channel.Name);
                var list = sa.OrderBy(a => a.SendTime.Ticks).Reverse().ToList();
                int cnt = list.Count();
                if (cnt > 0)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Down:
                            {
                                lastMessageScrollIndex--;
                                if (lastMessageScrollIndex < 0)
                                {
                                    rTB_Send.Clear();
                                    lastMessageScrollIndex = -1;
                                }
                                else
                                {
                                    lastMessageScrollIndex = Math.Max(0, lastMessageScrollIndex);

                                    rTB_Send.Text = list.ElementAtOrDefault(lastMessageScrollIndex).Message;
                                }
                            }
                            break;

                        case Keys.Up:
                            {
                                lastMessageScrollIndex++;
                                lastMessageScrollIndex = Math.Min(cnt - 1, lastMessageScrollIndex);
                                rTB_Send.Text = list[lastMessageScrollIndex].Message;
                            }
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        private void ChatPanel_Load(object sender, EventArgs e)
        {
            try
            {
                chatView1.OnLinkClicked += chatView1_OnLinkClicked;
                chatView1.OnMessageReceived += chatView1_OnMessageReceived;
                chatView1.OnUserNameClicked += chatView1_OnUserNameClicked;
                chatView1.OnEmoteClicked += chatView1_OnEmoteClicked;

                userInfoPanel1.Font = new Font(chatView1.Font.FontFamily, 10);

                //this.ParentForm.AcceptButton = this.btn_Send;
                rTB_Send.AddContextMenu();
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        private void chatView1_MouseDown(object sender, MouseEventArgs e)
        {
            userInfoPanel1.Hide();
        }

        private void chatView1_MouseUp(object sender, MouseEventArgs e)
        {
            lstB_Search.Visible = false;
            lastSearch = "";
        }

        private void chatView1_OnEmoteClicked(ChatView sender, Emotes.Emote emote)
        {
            rTB_Send.AppendText(" " + emote.Name);
        }

        private void chatView1_OnLinkClicked(ChatView sender, string url)
        {
            //if (!url.StartsWith("http"))
            //{
            //    url = "https://" + url;
            //}
            Settings.StartBrowser(url);
        }

        private void chatView1_OnMessageReceived(ChatView sender, ChatMessage message)
        {
            if (message.IsType(MsgType.Outgoing))
            {
                //sentMessages.Add(message);
                //lastMessageScrollMax = Math.Max(0, sentMessages.Count - 1);
            }
        }

        private void chatView1_OnUserNameClicked(ChatView sender, ChatUser emote, Point mouse)
        {
            Point location = this.PointToClient(mouse);
            userInfoPanel1.Show(emote, location);
        }

        private void lstB_Search_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            e.DrawFocusRectangle();

            int y = e.Bounds.Y;

            Rectangle rec = new Rectangle(0, y, e.Bounds.Height, e.Bounds.Height);
            if (lstB_Search.ClientRectangle.IntersectsWith(rec))
            {
                int x = 0;
                var res = searchResult[lstB_Search.Items[e.Index].ToString()];
                if (res.IsEmote)
                {
                    var em = (Emotes.EmoteBase)res.Result;
                    x += 5;
                    em.Draw(e.Graphics, x, y, e.Bounds.Height, e.Bounds.Height, Settings.EmoteQuality);
                    x += 5 + e.Bounds.Height;
                }
                //else
                //{
                //    //c = ((ChatUser)res.Result).GetColor((float)LX29_Helpers.Settings.UserColorBrigthness);
                //    e.Graphics.FillRectangle(Brushes.DarkGray, elrec);
                //}
                TextRenderer.DrawText(e.Graphics, res.Name, e.Font, new Point(x, y), e.ForeColor);
            }
        }

        private void lstB_Search_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lockListSearch)
            {
                selectName();
                lstB_Search.Visible = false;
                rTB_Send.Focus();
                rTB_Send.Select(rTB_Send.TextLength, 0);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            //rTB_Send.SuspendLayout();
            //string text = rTB_Send.Text;
            //int sel = rTB_Send.SelectionStart;
            //rTB_Send.SelectAll();
            //rTB_Send.SelectedText = text.RemoveControlChars();
            //rTB_Send.SelectionStart = sel;
            //rTB_Send.ResumeLayout();
        }

        private void rTB_Send_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                if (lstB_Search.Visible)
                {
                    if (lstB_Search.Items.Count > 0)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Down:
                                if (lstB_Search.SelectedIndex + 1 >= lstB_Search.Items.Count)
                                    lstB_Search.SelectedIndex = 0;
                                else lstB_Search.SelectedIndex++;
                                break;

                            case Keys.Up:
                                if (lstB_Search.SelectedIndex - 1 < 0)
                                    lstB_Search.SelectedIndex = lstB_Search.Items.Count - 1;
                                else lstB_Search.SelectedIndex--;
                                break;
                        }
                    }
                    else
                    {
                        lastSearch = "";
                        searchResult.Clear();
                    }
                }
                else
                {
                    lastSearch = "";
                    searchResult.Clear();
                    ChatHistory(e);
                }
            }
        }

        private void rTB_Send_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    if (!lastSearch.IsEmpty())
                    {
                        selectName();
                    }
                    else
                    {
                        btn_Send.PerformClick();
                    }
                    lastSearch = "";
                    searchResult.Clear();
                }
                else if (e.KeyData == Keys.Tab)
                {
                    lockListSearch = true;
                    if (lastSearch.IsEmpty())
                    {
                        e.SuppressKeyPress = true;

                        var arr = rTB_Send.Text.Trim().Split(" ");
                        lastSearch = arr.Select(t => t.Trim()).Last();

                        if (lastSearch.Length > 1)
                        {
                            // System.Threading.Tasks.Task.Run(() =>
                            {
                                searchResult = search(lastSearch.ToLower());
                                if (searchResult != null && searchResult.Count() > 0)
                                {
                                    //this.Invoke(new Action(() =>
                                    {
                                        lstB_Search.Items.Clear();
                                        foreach (var result in searchResult)
                                        {
                                            if (result.Value.Result is Emotes.Emote)
                                            {
                                                var em = (Emotes.Emote)result.Value.Result;
                                                if (em.IsGif)
                                                {
                                                    if (Settings.AnimateGifInSearch) timer1.Interval = 200;
                                                }
                                            }
                                            lstB_Search.Items.Add(result.Key);
                                        }
                                        lstB_Search.SelectedIndex = 0;
                                        lstB_Search.Visible = true;
                                        lstB_Search.Location = new Point(0, chatView1.Bottom - lstB_Search.Height);

                                        lockListSearch = false;
                                    }//));
                                }
                            }//);
                        }
                    }
                    else
                    {
                        if (lstB_Search.SelectedIndex + 1 >= lstB_Search.Items.Count)
                            lstB_Search.SelectedIndex = 0;
                        else lstB_Search.SelectedIndex++;
                        lockListSearch = false;
                    }
                }
                else if (e.KeyData == Keys.Space)
                {
                    selectName();
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                }
                else
                {
                    timer1.Interval = 1000;
                    lastMessageScrollIndex = -1;
                    lastSearch = "";
                    searchResult.Clear();
                    lstB_Search.Visible = false;
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        rTB_Send_KeyUp(sender, e);
                        break;
                }
            }
        }

        private Dictionary<string, EmoteSearchResult> search(string s)
        {
            try
            {
                //  DateTime now = DateTime.Now;
                string search = s.ToLower().Trim();

                var users = ChatClient.Users.Find(search, Channel.Name)
                    .Select(u => new EmoteSearchResult(u, u.DisplayName))
                    .OrderBy(t => t.Name.Length).ThenBy(t => t.Name);

                var emotes = ChatClient.Emotes.Values.Find(s, Channel.Name)
                .Select(t => new EmoteSearchResult(t, t.Name));
                //.OrderBy(t => t.Name.Length).ThenBy(t => t.Name);

                var dict = users.Concat(emotes).ToDictionary(t => t.Name);
                // MessageBox.Show(DateTime.Now.Subtract(now).TotalMilliseconds.ToString());
                return dict;
            }
            catch { }
            return null;
        }

        private void selectName()
        {
            try
            {
                if ((lstB_Search.SelectedIndex >= 0 && !lastSearch.IsEmpty()))
                {
                    var text = rTB_Send.Text.Trim();
                    var repl = lstB_Search.SelectedItem;
                    if (repl != null)
                    {
                        text = text.Replace(lastSearch, repl.ToString()) + " ";

                        timer1.Interval = 1000;
                        rTB_Send.Clear();
                        rTB_Send.AppendText(text);
                        lstB_Search.Visible = false;
                        rTB_Send.Focus();
                        rTB_Send.Select(rTB_Send.TextLength - 1, 0);
                    }
                }
            }
            catch
            {
            }
            lastSearch = "";
            searchResult.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (lstB_Search.Visible)
                    lstB_Search.Invalidate();
                else
                {
                    // timer1.Interval = 1000;
                }

                if (Channel != null)
                {
                    if (Channel.HasSlowMode)
                    {
                        TimeSpan ts = DateTime.Now.Subtract(Channel.LastSendMessageTime);
                        int tts = Channel.SlowMode - (int)ts.TotalSeconds;

                        if (tts <= Channel.SlowMode && tts > 0)
                        {
                            btn_Send.Text = (Channel.SlowMode - (int)ts.TotalSeconds) + "s";// > 0 ? @"{mm\:ss}" : @"{ss}", ts);
                            return;
                        }
                    }
                    btn_Send.Text = "Chat";
                }
            }
            catch
            {
            }
        }

        public struct EmoteSearchResult
        {
            public readonly bool IsEmote;
            public readonly string Name;
            public readonly object Result;

            public EmoteSearchResult(object result, string name)
            {
                IsEmote = (result is LX29_ChatClient.Emotes.EmoteBase);
                Result = result;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}