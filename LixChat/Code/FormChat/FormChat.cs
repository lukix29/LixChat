using LX29_ChatClient.Channels;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace LX29_ChatClient.Forms
{
    public partial class FormChat : Form
    {
        private static Color curMsgColor = Color.DarkRed;

        private static Color newMsgColor = Color.FromArgb(0, 100, 0);

        //private bool autoScroll = true;

        private ChannelInfo currentChannel;

        //private bool isSearch = false;

        private MsgType MessageType = MsgType.All_Messages;

        // private string prevMsg = "";

        public FormChat(ChannelInfo si)
        {
            IsClosed = false;
            InitializeComponent();
            currentChannel = si;
        }

        public ChatView chatView
        {
            get { return chatPanel1.ChatView; }
        }

        public bool IsClosed
        {
            get;
            private set;
        }

        private void AddTSMItem(string name, bool insert = false)
        {
            ToolStripButton item = new ToolStripButton(name + " ");
            item.Name = name;

            SetColor(item, false);
            item.MouseUp += item_Click;

            if (!insert) toolStrip1.Items.Add(item);
            else toolStrip1.Items.Insert(0, item);

            if (name.Equals("Whisper", StringComparison.OrdinalIgnoreCase))
            {
                toolStrip1.Items.Add(new ToolStripSeparator());
            }
        }

        private void AddTSMItem(MsgType si, bool insert = false)
        {
            string name = Enum.GetName(typeof(MsgType), si);
            AddTSMItem(name, insert);
        }

        private void ChatClient_OnWhisperReceived(ChatMessage message)
        {
            if (!toolStrip1.Items.ContainsKey(message.Channel))
            {
                AddTSMItem(message.Channel);
                this.BeginInvoke(new Action(() => SetNewMessageTSMi(message)));
            }
            else
            {
                this.BeginInvoke(new Action(() => SetNewMessageTSMi(message)));
            }
        }

        private void chatPanel1_Load(object sender, EventArgs e)
        {
        }

        private void chatPanel1_OnWhisperSent(object sender, ChatMessage message)
        {
            ChatClient_OnWhisperReceived(message);
        }

        private void chatView_OnMessageReceived(ChatView sender, ChatMessage message)
        {
            this.BeginInvoke(new Action(() => SetNewMessageTSMi(message)));
        }

        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            chatView.Stop();
            ChatClient.Messages.OnWhisperReceived -= ChatClient_OnWhisperReceived;

            IsClosed = true;
        }

        private void FormChat_Load(object sender, EventArgs e)
        {
            IsClosed = false;
            try
            {
                this.AllowTransparency = false;
                this.TransparencyKey = Color.DarkGoldenrod;
                chatView.BackColor = UserColors.ChatBackground;

                var ca = toolStrip1.Items.Cast<ToolStripItem>()
                    .Where(t => t.Text.Equals("Emotes") || t.Text.Equals("Settings")).ToArray();
                toolStrip1.Items.Clear();
                toolStrip1.Items.AddRange(ca);

                toolStrip1.Items.Insert(0, new ToolStripSeparator());
                AddTSMItem(MsgType.HL_Messages, true);
                AddTSMItem(MsgType.Outgoing, true);
                AddTSMItem(MsgType.All_Messages, true);
                foreach (var name in ChatClient.Messages.Whispers.Keys)
                {
                    AddTSMItem(name);
                }

                SetColor(tS_Btn_ChatSettings, false);

                ChatClient.Messages.OnWhisperReceived += ChatClient_OnWhisperReceived;

                chatPanel1.OnWhisperSent += chatPanel1_OnWhisperSent;

                chatView.OnMessageReceived += chatView_OnMessageReceived;

                chatView.SetChannel(currentChannel, MsgType.All_Messages);
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        FormChat_Load(sender, e);
                        break;

                    case MessageBoxResult.Abort:
                        this.Close();
                        return;
                }
            }
        }

        private void item_Click(object sender, MouseEventArgs e)
        {
            try
            {
                ToolStripButton tsb = (ToolStripButton)sender;
                string nme = tsb.Name;
                if (Enum.TryParse<MsgType>(nme, out MessageType))
                {
                    chatView.SetAllMessages(MessageType);
                }
                else
                {
                    chatView.SetAllMessages(MsgType.Whisper, null, nme);
                }
                chatView.ShowEmotes = false;
                SetColor(tsb, false);
            }
            catch
            {
            }
        }

        private void SetColor(ToolStripItem item, bool newMessage)
        {
            if (newMessage)
            {
                item.BackColor = UserColors.ChatBackground;
                item.ForeColor = Color.FromArgb(250, 50, 50);
            }
            else
            {
                item.BackColor = UserColors.ChatBackground;
                item.ForeColor = Color.White;
            }
        }

        private void SetNewMessageTSMi(ChatMessage message)
        {
            try
            {
                string name = "All_Messages";
                if (message.IsType(MsgType.Whisper))
                {
                    name = message.Channel;
                }
                if (message.Channel.Equals(currentChannel.Name) && !message.IsType(MessageType))
                {
                    var item = toolStrip1.Items.Find(name, true);
                    if (item.Length > 0)
                    {
                        SetColor(item[0], true);
                    }
                }
            }
            catch
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.Text = currentChannel.DisplayName + " @ Twitch Chat (" + Enum.GetName(typeof(MsgType), MessageType) + ") |" +
                    " Messages: " + chatView.MessageCount +
                    " AllCount: " + ChatClient.Messages.MessageCount(currentChannel.Name);
            }
            catch { }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void tS_Btn_ChatSettings_Click(object sender, EventArgs e)
        {
            try
            {
                controlSettings1.Show(chatView);
            }
            catch
            {
            }
        }

        private void tS_Btn_Emotes_Click(object sender, EventArgs e)
        {
            chatView.ShowEmotes = true;
        }
    }

    //public class LXQueue
    //{
    //    //  private DateTime lastSec = DateTime.MinValue;
    //    private DateTime lastAdd = DateTime.MinValue;

    //    private Queue<string> queue = new Queue<string>();

    //    //private int msgCnt = 0;
    //    private double timeout = 100.0;

    //    //public LXQueue()
    //    //{
    //    //    lastSec = DateTime.Now;
    //    //}

    //    public int CollectTime
    //    {
    //        get { return (int)timeout; }
    //        set
    //        {
    //            timeout = Math.Max(0, value);
    //        }
    //    }

    //    //public int MsgPerSec
    //    //{
    //    //    get;
    //    //    private set;
    //    //}

    //    public int Count
    //    {
    //        get { return queue.Count; }
    //    }

    //    public string Enqueue(string s)
    //    {
    //        if (DateTime.Now.Subtract(lastAdd).TotalMilliseconds > timeout)
    //        {
    //            lastAdd = DateTime.Now;
    //            if (queue.Count > 0)
    //            {
    //                StringBuilder sb = new StringBuilder();
    //                while (queue.Count > 0)
    //                {
    //                    sb.AppendLine(queue.Dequeue());
    //                }
    //                return sb.ToString();
    //            }
    //            else
    //            {
    //                return s;
    //            }
    //        }
    //        else
    //        {
    //            queue.Enqueue(s);
    //            return string.Empty;
    //        }
    //    }

    //    public void Clear()
    //    {
    //        queue.Clear();
    //        // msgCnt = 0;
    //        // lastSec = DateTime.Now;
    //        lastAdd = DateTime.MinValue;
    //    }
    //}

    //public static class HTMLFactory
    //{
    //    public const string Font = "Helvetica";
    //    public const float Size = 3f;
    //    public const string MSG_Color = "#D3D3D3";
    //    public const string TO_Color = "#7A7A7A";
    //    public const string HL_Color = "#FF0000";
    //    public const string Notice_Color = "#47476b";

    //    public static string EmoteWindow()
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        var ems = ChatClient.Emotes.Emotes.Values.OrderBy(t => (t.Name));
    //        foreach (var emote in ems)
    //        {
    //            sb.Append(GetImage(emote.URL, emote.Name));
    //        }
    //        return sb.ToString();
    //    }

    //    private static Regex reg = new Regex(@"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$",
    //        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    //    public static string GetMessage(ChatMessage msg, string color = MSG_Color,
    //        bool bold = false, float size = Size)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        if (!msg.HasTimeOut)
    //        {
    //            foreach (string s in msg.Words)
    //            {
    //                try
    //                {
    //                    if (s.Contains('.') && !s.Contains("..") && reg.IsMatch(s))
    //                    {
    //                        sb.Append("<a href=\"" + s + "\">" + s + " </a> ");
    //                    }
    //                    else if (ChatClient.Emotes.ContainsKey(s))
    //                    {
    //                        Emote em = ChatClient.Emotes[s];
    //                        sb.Append(GetImage(em.URL, em.Name));
    //                    }
    //                    else
    //                    {
    //                        sb.Append(s + " ");
    //                    }
    //                }
    //                catch { }
    //            }
    //        }
    //        else sb.Append(msg.Message);
    //        return GetString(sb.ToString(), color, bold, size);
    //    }

    //    public static string GetString(string text, string color = MSG_Color,
    //        bool bold = false, float size = Size)
    //    {
    //        return "<font face=\"" + Font + "\" size=\"" + size.ToString("F2") + "\" color=\"" + color +
    //            "\">" + (bold ? "<b>" : "") + text + (bold ? "</b>" : "") + "</font>";
    //    }

    //    public static string GetImage(string url, string text, int height = 25)
    //    {
    //        return "<img height=" + height + " src=\"" + url + "\" style=\"vertical-align:middle\" alt=\"" + text + "\" /> ";
    //    }

    //    private static string getBadges(ChatUser user1)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        foreach (BadgeType bt in user1.Badges)
    //        {
    //            //Badge url = ChatClient.Emotes.Badges.GetBadge(bt, user1.Channel);
    //            //if (url != null)
    //            //{
    //            //    //sb.Append(GetImage(url.GetURL(bt), url.UserType, 15));
    //            //}
    //        }
    //        return sb.ToString();
    //    }

    //    public static string GetUsers(ChatUser user1, ChatUser user2, ChatMessage m)
    //    {
    //        StringBuilder sb = new StringBuilder();

    //        sb.Append(HTMLFactory.GetString(m.SendTMi.ToLongTimeString(), TO_Color, false, 2));

    //        sb.Append(getBadges(user1));

    //        sb.Append(HTMLFactory.GetString(user1.DisplayName, user1.HexColor, true));
    //        sb.Append(HTMLFactory.GetString(" > ", MSG_Color, true));
    //        sb.Append(HTMLFactory.GetString(user2.DisplayName + ": ", user2.HexColor, true));
    //        return sb.ToString();
    //    }

    //    public static string GetUser(ChatUser user, ChatMessage m, string color = null)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append(HTMLFactory.GetString(m.SendTMi.ToLongTimeString(), TO_Color, false, 2));

    //        sb.Append(getBadges(user));
    //        // sb.Append(HTMLFactory.GetString(" " + user.BadgeString, MSG_Color));
    //        string s = user.DisplayName;
    //        if (string.IsNullOrEmpty(s))
    //        {
    //            s = "!ERROR!";
    //        }
    //        //if (string.IsNullOrEmpty(color))
    //        //{
    //        //    color = user.HexColor;
    //        //}
    //        //else
    //        //{
    //        //}
    //        sb.Append(HTMLFactory.GetString(user.DisplayName + ": ", user.HexColor, true));

    //        return sb.ToString();
    //    }

    //    public static string GetUser(string user, string tmi)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        if (string.IsNullOrEmpty(tmi))
    //        {
    //            tmi = DateTime.Now.ToLongTimeString();
    //        }
    //        sb.Append(HTMLFactory.GetString(tmi, TO_Color, false, 2));
    //        if (!string.IsNullOrEmpty(user))
    //        {
    //            user = ": ";
    //            sb.Append(HTMLFactory.GetString(user, MSG_Color, true));
    //        }
    //        return sb.ToString();
    //    }
    //}
}