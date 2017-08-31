using LX29_ChatClient.Channels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LX29_ChatClient.Addons
{
    public partial class FormAutoChatActions : Form
    {
        private List<ChatAction> actions = new List<ChatAction>();
        private float addFacSize = 100;
        private ChannelInfo channel = null;
        private string channelName = "";
        private bool isSaved = true;
        private string oldChannelname = string.Empty;

        private string[] UserNames;

        public FormAutoChatActions(ChannelInfo channel)
        {
            this.channel = channel;
            channelName = channel.Name;
            InitializeComponent();
        }

        //private void AddItem()
        //{
        //    var ca = GetAction();

        //    if (ChatClient.AutoActions.ChangeChatAction(ca))
        //    {
        //        lstB_Main.Items.Clear();
        //        var actions = ChatClient.AutoActions.GetChannelActions(channelName);
        //        foreach (ChatAction c in actions)
        //        {
        //            lstB_Main.Items.Add(c.ToString());
        //        }
        //    }

        //    //lV_Main.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        //}

        //private void btn_Change_Click(object sender, EventArgs e)
        //{
        //    if (lstB_Main.SelectedIndex >= 0)
        //    {
        //        var ca = GetAction();

        //        ChatClient.AutoActions[lstB_Main.SelectedIndex] = ca;

        //        LoadActions(false);
        //    }
        //}

        private void btn_Help_Click(object sender, EventArgs e)
        {
            string help = LX29_LixChat.Properties.Resources.ActionHelp;
            LX29_MessageBox.Show(help, "Action Help");
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            try
            {
                var cb = panel2.Controls.Cast<Control>().Where(t => (t is CheckBox && t.Name.StartsWith("sel_"))).Select(t => t as CheckBox);
                var temp = new List<ChatAction>();
                foreach (var c in cb)
                {
                    if (!c.Checked)
                    {
                        var idx = int.Parse(c.Name.LastLine("_"));
                        temp.Add(actions[idx]);
                    }
                }
                actions = new List<ChatAction>(temp);
                LoadActions(false);

                isSaved = false;
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            isSaved = true;
            ChatClient.AutoActions.Save(actions);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ChatAction action = new ChatAction(channel.Name);
                actions.Add(action);
                CreateChatActionControls();
                isSaved = false;
            }
            catch (Exception x)
            {
                x.Handle();
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

        private void CreateChatActionControls()
        {
            try
            {
                int y = 0;
                if (panel2.Controls.Count > 0)
                {
                    y = panel2.Controls.Cast<Control>().Max(t => t.Bottom) + 5;
                }

                panel2.Controls.Clear();
                for (int i = 0; i < actions.Count; i++)
                {
                    ChatAction ca = actions[i];
                    List<Control> list = new List<Control>();
                    var props = ca.GetType().GetProperties().Where(t => !t.Name.StartsWith("Is"))
                        .OrderByDescending(t => t.PropertyType.Equals(typeof(string)))
                        .ThenByDescending(t => t.PropertyType.Equals(typeof(bool)))
                        .ThenByDescending(t => t.PropertyType.Equals(typeof(int)));

                    var cb = Creator.CreateCheckBox(false, "sel_" + i, "");
                    //cb.CheckedChanged += (o, e) =>
                    //{
                    //};
                    list.Add(cb);
                    foreach (var prop in props)
                    {
                        if (prop.PropertyType.Equals(typeof(int)))
                        {
                            var iv = (int)prop.GetValue(ca);
                            var nud = Creator.CreateNuD(iv, prop.Name + "_" + i);
                            nud.ValueChanged += (o, e) =>
                                {
                                    prop.SetValue(ca, (int)nud.Value);
                                    isSaved = false;
                                };
                            list.Add(nud);
                        }
                        else if (prop.PropertyType.Equals(typeof(bool)))
                        {
                            var b = (bool)prop.GetValue(ca);
                            var nud = Creator.CreateCheckBox(b, prop.Name + "_" + i, prop.Name);
                            nud.CheckedChanged += (o, e) =>
                            {
                                prop.SetValue(ca, nud.Checked);
                                isSaved = false;
                            };
                            list.Add(nud);
                        }
                        else if (prop.PropertyType.Equals(typeof(string)))
                        {
                            var s = (string)prop.GetValue(ca);
                            if (prop.Name.Equals("username", StringComparison.OrdinalIgnoreCase))
                            {
                                var nud = Creator.CreateTextPreviewControl(prop.Name + "_" + i, s);
                                nud.TextChanged += (o, e) =>
                                {
                                    if (nud.Text.Equals("*"))
                                    {
                                        prop.SetValue(ca, nud.Text);
                                        isSaved = false;
                                    }
                                    else
                                    {
                                        nud.SearchArray(UserNames);
                                    }
                                };
                                nud.KeyUp += (o, e) =>
                                {
                                    if (e.KeyCode == Keys.Enter)
                                    {
                                        prop.SetValue(ca, nud.Text);
                                        isSaved = false;
                                    }
                                };
                                list.Add(nud);
                            }
                            else
                            {
                                var nud = Creator.CreateRtB(prop.Name + "_" + i, s);
                                nud.TextChanged += (o, e) =>
                                {
                                    prop.SetValue(ca, nud.Text);
                                    isSaved = false;
                                };
                                list.Add(nud);
                            }
                        }
                    }

                    int height = list.Max(t => t.Height);
                    int x = 5;
                    foreach (var ctrl in list)
                    {
                        ctrl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                        ctrl.Location = new System.Drawing.Point(x, y);
                        ctrl.Width = (int)addFacSize;
                        //if (fillLabels) labels.Add(new Label() { Text = ctrl.Name, Location = new System.Drawing.Point(x, 0) });
                        x += ctrl.Width + 5;
                        panel2.Controls.Add(ctrl);
                    }
                    //fillLabels = false;

                    y += height + 5;
                }
                //return labels;
            }
            catch
            {
            }
            //return null;
        }

        private void FormChatSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isSaved)
            {
                switch (LX29_MessageBox.Show("Do you want to save changes before Closing?", "Chat Actions", MessageBoxButtons.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        ChatClient.AutoActions.Save(actions);
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

        private void LoadActions(bool withuser)
        {
            try
            {
                actions = ChatClient.AutoActions.GetChannelActions(channelName).Select(t => (ChatAction)t.Clone()).ToList();

                var props = new ChatAction("").GetType().GetProperties().Where(t => !t.Name.StartsWith("Is"))
                    .OrderByDescending(t => t.PropertyType.Equals(typeof(string)))
                    .ThenByDescending(t => t.PropertyType.Equals(typeof(bool)))
                    .ThenByDescending(t => t.PropertyType.Equals(typeof(int)));
                int x = 5;
                addFacSize = panel2.Width / (float)props.Count();
                foreach (var prop in props)
                {
                    Label ctrl = new Label();
                    ctrl.Text = prop.Name;
                    ctrl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    ctrl.Location = new System.Drawing.Point(x, 5);
                    ctrl.Width = (int)addFacSize;
                    panel2.Controls.Add(ctrl);
                    x += ctrl.Width + 5;
                }
                CreateChatActionControls();

                if (withuser)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        UserNames = ChatClient.Users.GetAllNames();
                    });
                }
                btn_remove.Enabled = false;
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        public static class Creator
        {
            public static CheckBox CreateCheckBox(bool check, string name, string text)
            {
                var cB = new CheckBox();
                cB.Anchor = System.Windows.Forms.AnchorStyles.Left;
                cB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                cB.Name = name;
                cB.Checked = check;
                cB.Text = text;
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
                nUD.Size = new System.Drawing.Size(40, 22);
                nUD.Minimum = 0;
                //nUD.AutoSize = true;
                nUD.Maximum = Int16.MaxValue;
                nUD.Value = value;
                return nUD;
            }

            public static RichTextBox CreateRtB(string name, string text)
            {
                var rTB = new RichTextBox();
                rTB.Anchor = System.Windows.Forms.AnchorStyles.Left;
                rTB.Multiline = false;
                rTB.Name = name;
                rTB.Size = new System.Drawing.Size(132, 22);
                rTB.Text = text;
                return rTB;
            }

            public static TextPreviewControl CreateTextPreviewControl(string name, string text)//, string[] items)
            {
                var rTB = new TextPreviewControl();
                rTB.Anchor = System.Windows.Forms.AnchorStyles.Left;
                rTB.Name = name;
                rTB.Size = new System.Drawing.Size(132, 22);
                rTB.Text = text;
                //rTB.Items.AddRange(items);
                return rTB;
            }
        }
    }
}