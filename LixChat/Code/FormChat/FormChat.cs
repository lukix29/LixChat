using LX29_ChatClient.Channels;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
                this.Invoke(new Action(() => SetNewMessageTSMi(message)));
            }
            else
            {
                this.Invoke(new Action(() => SetNewMessageTSMi(message)));
            }
        }

        private void chatPanel1_Load(object sender, EventArgs e)
        {
        }

        private void chatPanel1_OnWhisperSent(object sender, ChatMessage message)
        {
            ChatClient_OnWhisperReceived(message);
        }

        private void chatView_OnMessageReceived(ChatMessage msg)
        {
            if (msg.Channel.Equals(currentChannel.Name))
            {
                SetNewMessageTSMi(msg);
            }
        }

        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            chatView.Stop();
            ChatClient.Messages.OnWhisperReceived -= ChatClient_OnWhisperReceived;
            ChatClient.OnMessageReceived -= chatView_OnMessageReceived;
            chatPanel1.OnWhisperSent -= chatPanel1_OnWhisperSent;
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

                ChatClient.Messages.OnWhisperReceived += ChatClient_OnWhisperReceived;

                chatPanel1.OnWhisperSent += chatPanel1_OnWhisperSent;

                ChatClient.OnMessageReceived += chatView_OnMessageReceived;

                ChatClient.TryConnect(currentChannel.Name);

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
                chatView.RefreshMessages();
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
                    //" Messages: " + chatView.MessageCount +
                    " Messages: " + ChatClient.Messages.Count(currentChannel.Name);
            }
            catch { }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void tS_Btn_Emotes_Click(object sender, EventArgs e)
        {
            chatView.ShowEmotes = true;
        }
    }
}