using LX29_ChatClient.Channels;
using System;
using System.Linq;
using System.Collections.Generic;
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
            string help = LX29_LixChat.Properties.Resources.ActionHelp;
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

        private void cB_FirstOrAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cB_FirstOrAll.Checked)
            {
                cB_FirstOrAll.Text = "Match START";
            }
            else
            {
                cB_FirstOrAll.Text = "Match ANY ocurrence";
            }
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
            if (Settings.isDebug)
            {
                var header = typeof(ChatAction).GetProperties().Where(t => !t.Name.Equals("IsEmpty"))
                        .OrderByDescending(t => t.PropertyType.Equals(typeof(string)))
                        .ThenByDescending(t => t.PropertyType.Equals(typeof(bool)))
                        .ThenByDescending(t => t.PropertyType.Equals(typeof(int)))
                        .Select(t => (new Label() { Text = t.Name }) as Control).ToList();

                var pn = Creator.CreatePanel(header, "Header", panel2.Width);
                pn.Location = new System.Drawing.Point(0, 0);

                panel2.Controls.Add(pn);

                List<Panel> panels = new List<Panel>();
                int y = pn.Height + 5;
                foreach (ChatAction ca in actions)
                {
                    List<Control> list = new List<Control>();
                    var props = ca.GetType().GetProperties().Where(t => !t.Name.Equals("IsEmpty"))
                        .OrderByDescending(t => t.PropertyType.Equals(typeof(string)))
                        .ThenByDescending(t => t.PropertyType.Equals(typeof(bool)))
                        .ThenByDescending(t => t.PropertyType.Equals(typeof(int)));
                    foreach (var prop in props)
                    {
                        if (prop.PropertyType.Equals(typeof(int)))
                        {
                            var i = (int)prop.GetValue(ca);
                            var nud = Creator.CreateNuD(i, prop.Name);
                            nud.ValueChanged += (o, e) =>
                                {
                                    prop.SetValue(ca, (int)nud.Value);
                                };
                            list.Add(nud);
                        }
                        else if (prop.PropertyType.Equals(typeof(bool)))
                        {
                            var b = (bool)prop.GetValue(ca);
                            var nud = Creator.CreateCheckBox(b, prop.Name);
                            nud.CheckedChanged += (o, e) =>
                            {
                                prop.SetValue(ca, nud.Checked);
                            };
                            list.Add(nud);
                        }
                        else if (prop.PropertyType.Equals(typeof(string)))
                        {
                            var s = (string)prop.GetValue(ca);
                            var nud = Creator.CreateRtB(prop.Name, s);
                            nud.TextChanged += (o, e) =>
                            {
                                prop.SetValue(ca, nud.Text);
                            };
                            list.Add(nud);
                        }
                    }
                    // list = list.OrderByDescending(t => t is RichTextBox).ToList();

                    pn = Creator.CreatePanel(list, ca.Message, panel2.Width);
                    pn.Location = new System.Drawing.Point(0, y);
                    y += pn.Height + 5;
                    panel2.Controls.Add(pn);
                }
            }
            else
            {
                panel2.Visible = false;
                lstB_Main.Items.Clear();
                lstB_Main.Items.AddRange(actions.Select(t => t.ToString()).ToArray());
            }
            if (withuser)
            {
                comboBox1.Items.Clear();
                System.Threading.Tasks.Task.Run(() =>
                {
                    var names = ChatClient.Users.GetAllNames();
                    if (names != null)
                    {
                        this.Invoke(new Action(() =>
                        {
                            comboBox1.Items.AddRange(names);
                            comboBox1.SelectedIndex = 0;
                        }));
                    }
                });
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {
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

        public static class Creator
        {
            public static CheckBox CreateCheckBox(bool check, string name)
            {
                var cB = new CheckBox();
                cB.Anchor = System.Windows.Forms.AnchorStyles.Left;
                cB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                cB.Name = name;
                cB.Checked = check;
                cB.Text = name;
                cB.UseVisualStyleBackColor = true;
                cB.AutoSize = true;
                return cB;
            }

            public static NumericUpDown CreateNuD(decimal value, string name)
            {
                var nUD = new NumericUpDown();
                nUD.Anchor = System.Windows.Forms.AnchorStyles.Left;
                nUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
                nUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                nUD.ForeColor = System.Drawing.Color.Gainsboro;
                nUD.Name = name;
                nUD.Size = new System.Drawing.Size(50, 22);
                nUD.Minimum = 0;
                //nUD.AutoSize = true;
                nUD.Maximum = Int16.MaxValue;
                nUD.Value = value;
                return nUD;
            }

            public static Panel CreatePanel(List<Control> controls, string name, int width)
            {
                Panel panel2 = new Panel();
                panel2.Size = new System.Drawing.Size(width, 50);
                panel2.AutoScroll = true;
                panel2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                int x = 0;
                foreach (var ctrl in controls)
                {
                    ctrl.Location = new System.Drawing.Point(x, panel2.Height / 2 - ctrl.Height / 2);
                    x += ctrl.Width + 5;
                }
                panel2.Controls.AddRange(controls.ToArray());
                panel2.Name = name;
                return panel2;
            }

            public static RichTextBox CreateRtB(string name, string text)
            {
                var rTB = new RichTextBox();
                rTB.Anchor = System.Windows.Forms.AnchorStyles.Left;
                rTB.Multiline = false;
                rTB.Name = name;
                rTB.Size = new System.Drawing.Size(132, 22);
                rTB.TabIndex = 21;
                rTB.Text = text;
                return rTB;
            }
        }
    }
}