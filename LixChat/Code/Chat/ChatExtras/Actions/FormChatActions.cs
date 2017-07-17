using LX29_ChatClient.Channels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LX29_ChatClient.Addons
{
    public partial class FormAutoChatActions : Form
    {
        private ChannelInfo channel = null;
        private string channelName = "";
        private bool isSaved = true;
        private string oldChannelname = string.Empty;

        public FormAutoChatActions(ChannelInfo channel)
        {
            this.channel = channel;
            channelName = channel.Name;
            InitializeComponent();
        }

        private void AddItem()
        {
            var ca = GetAction();

            if (ChatClient.AutoActions.ChangeChatAction(ca))
            {
                lstB_Main.Items.Clear();
                var actions = ChatClient.AutoActions.GetChannelActions(channelName);
                foreach (ChatAction c in actions)
                {
                    lstB_Main.Items.Add(c.ToString());
                }
            }

            //lV_Main.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void btn_Change_Click(object sender, EventArgs e)
        {
            if (lstB_Main.SelectedIndex >= 0)
            {
                var ca = GetAction();

                ChatClient.AutoActions[lstB_Main.SelectedIndex] = ca;

                LoadActions(false);
            }
        }

        private void btn_Help_Click(object sender, EventArgs e)
        {
            string help = LX29_TwitchChat.Properties.Resources.ActionHelp;
            LX29_MessageBox.Show(help, "Action Help");
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            if (lstB_Main.SelectedIndex >= 0)
            {
                string value = lstB_Main.SelectedItem.ToString();
                ChatClient.AutoActions.RemoveAt(t => t.Equals(value));

                LoadActions(false);

                isSaved = false;
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            isSaved = true;
            ChatClient.AutoActions.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtB_Username.TextLength > 0)
            {
                AddItem();
                isSaved = false;
            }
        }

        private void cB_Enabled_CheckedChanged(object sender, EventArgs e)
        {
            //StreamManager.ChatClient.ExecuteActions = cB_Enabled.Checked;
        }

        private void cB_Global_CheckedChanged(object sender, EventArgs e)
        {
            if (cB_Global.Checked)
            {
                oldChannelname = channel.Name;
                channelName = "global";
                this.Text = "GLOBAL Chat Actions";
            }
            else
            {
                channelName = oldChannelname;
                this.Text = "Chat Actions for Channel: " + channel.DisplayName;
            }
            LoadActions(true);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtB_Username.Text = comboBox1.SelectedItem.ToString();
        }

        private void FormChatSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isSaved)
            {
                switch (LX29_MessageBox.Show("Do you want to save changes before Closing?", "Chat Actions", MessageBoxButtons.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        ChatClient.AutoActions.Save();
                        break;

                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
            ChatClient.AutoActions.ChatActionShowing = false;
        }

        private void FormChatSettings_Load(object sender, EventArgs e)
        {
            LoadActions(true);

            this.Text = "Chat Actions for Channel: " + channel.DisplayName;
        }

        private ChatAction GetAction()
        {
            return new ChatAction(
               txtB_Username.Text,
               channelName,
               txtB_Message.Text,
              txtB_Action.Text,
              (int)nUD_Cooldown.Value,
              cB_FirstOrAll.Checked,
              (int)nUD_Delay.Value,
              cB_Enabled.Checked
              );
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void LoadActions(bool withuser)
        {
            lstB_Main.Items.Clear();
            var actions = ChatClient.AutoActions.GetChannelActions(channelName);
            foreach (ChatAction ca in actions)
            {
                lstB_Main.Items.Add(ca.ToString());
            }
            //txtB_Username.Text =
            //txtB_Message.Text =
            //txtB_Action.Text = "";
            //numericUpDown1.Value = 0;
            //numericUpDown2.Value = 0;
            //cb
            if (withuser)
            {
                comboBox1.Items.Clear();
                var names = ChatClient.Users.GetAllNames();
                if (names != null)
                {
                    comboBox1.Items.AddRange(names);
                    comboBox1.SelectedIndex = 0;
                }
            }
            //cB_MsgTypes.Items.Clear();
            //Array values = Enum.GetValues(typeof(MsgType));
            //foreach (MsgType val in values)
            //{
            //    cB_MsgTypes.Items.Add(val);
            //}
            //cB_MsgTypes.SelectedItem = MsgType.All_Messages;

            cB_FirstOrAll.Checked = false;
            cB_Enabled.Checked = false;
            btn_remove.Enabled = false;
        }

        private void lstB_Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstB_Main.SelectedIndex >= 0)
            {
                var item = lstB_Main.SelectedItem.ToString();

                ChatAction ca = ChatClient.AutoActions.FirstOrDefault(t => t.ToString().Equals(item));
                if (ca != null)
                {
                    txtB_Username.Text = ca.Username;
                    txtB_Message.Text = ca.Message;
                    txtB_Action.Text = ca.Action;
                    nUD_Cooldown.Value = ca.Cooldown;
                    cB_Enabled.Checked = ca.Enabled;
                    btn_remove.Enabled = true;
                    cB_FirstOrAll.Checked = ca.CheckStart;
                }
            }
        }

        private void txtB_Action_TextChanged(object sender, EventArgs e)
        {
            txtB_Action.Text = txtB_Action.Text.Replace("(", "[").Replace(")", "]");
        }

        private void txtB_Search_MouseDown(object sender, MouseEventArgs e)
        {
            txtB_Search.Clear();
        }

        private void txtB_Search_TextChanged(object sender, EventArgs e)
        {
            string text = txtB_Search.Text.ToLower();
            var find = comboBox1.Items.Cast<string>().ToList().FindIndex(t => t.ToLower().Contains(text));
            comboBox1.SelectedIndex = find;
        }
    }
}