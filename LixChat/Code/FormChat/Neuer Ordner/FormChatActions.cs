using LX29_ChatClient.Actions;
using LX29_ChatClient.Channels;
using System;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class FormChatSettings : Form
    {
        private ChannelInfo channelName;

        public FormChatSettings(ChannelInfo channelName)
        {
            InitializeComponent();
            this.channelName = channelName;
        }

        private int Index
        {
            get
            {
                if (lV_Main.SelectedIndices.Count > 0)
                {
                    int idx = lV_Main.SelectedIndices[0];
                    if (idx >= 0)
                    {
                        return idx;
                    }
                }
                return -1;
            }
        }

        private void AddItem(string channel, string Username, string Message, string Action)
        {
            string[] items = new string[] { Username, Message, Action };
            if (Index >= 0)
            {
                channelName.AutoActions.SetChatAction(Index, new ChatAction(items));
                lV_Main.Items[Index] = new ListViewItem(items);
            }
            else
            {
                if (channelName.AutoActions.AddChatAction(new ChatAction(items)))
                {
                    lV_Main.Items.Add(new ListViewItem(items));
                }
            }
            lV_Main.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            int idx = lV_Main.SelectedIndices[0];
            channelName.AutoActions.ChatActions.RemoveAt(idx);
            lV_Main.Items.RemoveAt(idx);
            lV_Main.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            //StreamManager.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddItem(channelName.Name, txtB_Username.Text, txtB_Message.Text, txtB_Action.Text);
        }

        private void cB_Enabled_CheckedChanged(object sender, EventArgs e)
        {
            channelName.AutoActions.ExecuteActions = cB_Enabled.Checked;
        }

        private void FormChatSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void FormChatSettings_Load(object sender, EventArgs e)
        {
            this.lV_Main.Columns.Add("Username", "Username", 100);
            this.lV_Main.Columns.Add("Message", "Message", 100);
            this.lV_Main.Columns.Add("Action", "Action", 100);

            this.lV_Main.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.lV_Main.View = View.Details;

            foreach (ChatAction ca in channelName.AutoActions.ChatActions)
            {
                string[] items = new string[] { ca.RawUsername, ca.RawMessage, ca.RawAction };
                lV_Main.Items.Add(new ListViewItem(items));
            }

            cB_Enabled.Checked = channelName.AutoActions.ExecuteActions;
            lV_Main.AllowColumnReorder = false;
            lV_Main.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void lV_Main_MouseUp(object sender, MouseEventArgs e)
        {
            if (lV_Main.SelectedIndices.Count == 0)
            {
                txtB_Username.Text = "";
                txtB_Message.Text = "";
                txtB_Action.Text = "";
                btn_remove.Visible = false;
            }
        }

        private void lV_Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Index >= 0)
            {
                ChatAction ca = channelName.AutoActions.GetChatAction(Index);
                txtB_Username.Text = ca.Username;
                txtB_Message.Text = ca.Message;
                txtB_Action.Text = ca.RawAction;
                btn_remove.Visible = true;
            }
        }

        private void txtB_Action_TextChanged(object sender, EventArgs e)
        {
            txtB_Action.Text = txtB_Action.Text.Replace("(", "[").Replace(")", "]");
        }
    }
}