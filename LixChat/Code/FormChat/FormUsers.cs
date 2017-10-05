using LX29_ChatClient.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class FormUsers : Form
    {
        private ChannelInfo channel = null;

        private bool lockListBox = false;
        private string search = "";

        //private int treeMax = 0;
        private Dictionary<string, ChatUser> users = new Dictionary<string, ChatUser>();

        public FormUsers()
        {
            InitializeComponent();
        }

        public void Show(ChannelInfo info)
        {
            try
            {
                channel = info;
                SetUsers(ChatClient.Users.Get(channel.Name));

                chatView1.SetChannel(info.ApiResult, MsgType.All_Messages);

                timer1.Enabled = true;

                base.Show();
            }
            catch
            {
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                chatView1.Dispose();
            }
            catch
            {
            }
            base.OnClosed(e);
        }

        private void chatView1_OnLinkClicked(ChatView sender, string url)
        {
            if (!url.StartsWith("http"))
            {
                url = "https://" + url;
            }
            System.Diagnostics.Process.Start(url);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void FormUsers_Load(object sender, EventArgs e)
        {
            SetTreeView();

            chatView1.OnLinkClicked += chatView1_OnLinkClicked;
        }

        private void GetUserInfos(string name)
        {
            if (lockListBox) return;
            lockListBox = true;
            var user = ChatClient.Users.Get(name, channel.Name);
            if (!user.IsEmpty)
            {
                //int idx = lstB_Users.Items.IndexOf(name);
                //lstB_Users.Items[idx] = user.DisplayName;

                apiInfoPanel1.SetChatInfos(user.ApiResult);
                SetChatInfoBox(name);

                SetMessages(MsgType.All_Messages, name);
            }
            lockListBox = false;
        }

        private void lstB_Users_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Change back to listbox!!!!!!!!!!!!!!!!!
            //var user = ChatClient.GetUser(selectedNode.Text, channel.Name);
            //string id = TwitchApi.GetUserID(user.Name).ID;
            //ApiResult result = TwitchApi.GetStreamOrChannel(id);
            //user.ApiResult = result;
            //SetTreeViewUsers(selectedNode);
        }

        private void lstB_Users_MouseEnter(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void lstB_Users_MouseLeave(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void lstB_Users_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstB_Users.SelectedItem != null)
            {
                string item = lstB_Users.SelectedItem.ToString().GetBetween("- ", "").Trim();
                this.BeginInvoke(new Action(() => GetUserInfos(item)));
            }
        }

        private void SetChatInfoBox(string name)
        {
            foreach (TreeNode s in treeView1.Nodes)
            {
                MsgType msgt = (MsgType)Enum.Parse(typeof(MsgType), s.Name);
                s.Text = s.Name + ": " + ChatClient.Messages.Count(channel.Name, msgt, name);
            }
        }

        private void SetMessages(MsgType type, string userName)
        {
            chatView1.SetAllMessages(type, null, userName);
        }

        private void SetTreeView()
        {
            List<TreeNode> list = new List<TreeNode>();
            var sa = Enum.GetNames(typeof(MsgType)).OrderByDescending(t => t.Length);
            foreach (var s in sa)
            {
                //MsgType msgt = (MsgType)Enum.Parse(typeof(MsgType), s);
                var n = new TreeNode(s + ": ");
                n.Name = s;
                list.Add(n);
            }
            //TreeNode node = new TreeNode("Messages", list.ToArray());
            treeView1.Nodes.AddRange(list.ToArray());
        }

        private void SetUsers(Dictionary<string, ChatUser> users)
        {
            Task.Run(() =>
            {
                this.users = users;

                var sa = users.Keys.Where(t => t.ToLower().StartsWith(search)).Select(t => new { name = t, count = ChatClient.Messages.Count(channel.Name, MsgType.All_Messages, t) })
                .OrderByDescending(t => t.count).Select(t => string.Format("{0,-4:####}", t.count) + " - " + t.name).ToArray();

                this.Invoke(new Action(() =>
                    {
                        var selidx = lstB_Users.SelectedItem;
                        lstB_Users.BeginUpdate();
                        lstB_Users.Items.Clear();
                        lstB_Users.Items.AddRange(sa);
                        lstB_Users.EndUpdate();
                        this.Text = "Users: " + sa.Length;
                        lstB_Users.SelectedItem = selidx;
                    }));
            });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            search = textBox1.Text.ToLower();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                var users = ChatClient.Users.Get(channel.Name);

                SetUsers(users);

                if (lstB_Users.SelectedItem != null)
                {
                    string item = lstB_Users.SelectedItem.ToString().Trim();
                    SetChatInfoBox(item);
                }
            }
            catch { }
        }
    }
}