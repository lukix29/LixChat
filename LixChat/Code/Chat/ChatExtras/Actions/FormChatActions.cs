using LX29_ChatClient.Channels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace LX29_ChatClient.Addons
{
    public partial class FormAutoChatActions : Form
    {
        private List<ChatAction> actions = new List<ChatAction>();
        private float addFacSize = 100;
        private bool isSaved = true;
        private List<Tuple<string, int>> labels = new List<Tuple<string, int>>();

        private string[] UserNames;

        public FormAutoChatActions()
        {
            InitializeComponent();
        }

        public CheckBox CreateCheckBox(bool check, string name, string text)
        {
            var cB = new CheckBox();
            cB.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //cB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cB.Name = name;
            cB.Checked = check;
            cB.Text = text;
            cB.Size = new System.Drawing.Size(50, 22);
            cB.UseVisualStyleBackColor = true;
            cB.AutoSize = true;
            return cB;
        }

        public NumericUpDown CreateNuD(decimal value, string name)
        {
            var nUD = new NumericUpDown();
            nUD.Anchor = System.Windows.Forms.AnchorStyles.Left;
            nUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            //nUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            nUD.ForeColor = System.Drawing.Color.Gainsboro;
            nUD.Name = name;
            nUD.Size = new System.Drawing.Size(60, 22);
            nUD.Minimum = 0;
            //nUD.AutoSize = true;
            nUD.Maximum = Int16.MaxValue;
            nUD.Value = value;
            return nUD;
        }

        public RichTextBox CreateRtB(string name, string text)
        {
            var rTB = new RichTextBox();
            rTB.Anchor = System.Windows.Forms.AnchorStyles.Left;
            rTB.Multiline = false;
            rTB.Name = name;
            rTB.Size = new System.Drawing.Size((int)addFacSize, 22);
            rTB.Text = text;
            return rTB;
        }

        public TextPreviewControl CreateTextPreviewControl(string name, string text)//, string[] items)
        {
            var rTB = new TextPreviewControl();
            rTB.Anchor = System.Windows.Forms.AnchorStyles.Left;
            rTB.Name = name;
            rTB.Size = new System.Drawing.Size((int)addFacSize, 22);
            rTB.Text = text;
            //rTB.Items.AddRange(items);
            return rTB;
        }

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
                actions.Clear();
                if (temp.Count > 0) actions.AddRange(temp);

                CreateChatActionControls();

                isSaved = false;
            }
            catch (Exception x)
            {
                x.Handle("", true);
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
                ChatAction action = new ChatAction();
                actions.Add(action);
                CreateChatActionControls();
                isSaved = false;
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        private void CreateChatActionControls()
        {
            try
            {
                panel2.Controls.Clear();

                int x = 5;
                Label lbl = new Label();
                lbl.Text = "A";// prop.Name;
                lbl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                lbl.Location = new System.Drawing.Point(x, 5);

                bool fillLabels = true;
                int y = lbl.Bottom + 5;
                labels.Clear();
                for (int i = 0; i < actions.Count; i++)
                {
                    ChatAction ca = actions[i];
                    List<Control> list = new List<Control>();
                    var props = ca.GetType().GetProperties().Where(t => !t.Name.StartsWith("Is"))
                         .OrderByDescending(t => t.PropertyType.Equals(typeof(string)))
                         .ThenByDescending(t => t.PropertyType.Equals(typeof(bool)))
                         .ThenByDescending(t => t.PropertyType.Equals(typeof(int)));

                    addFacSize = panel2.Width / (float)props.Count();

                    var cb = CreateCheckBox(false, "sel_" + i, "");
                    cb.Width = cb.Height;
                    // cb.AutoSize = true;
                    list.Add(cb);
                    foreach (var prop in props)
                    {
                        if (prop.PropertyType.Equals(typeof(int)))
                        {
                            var iv = (int)prop.GetValue(ca);
                            var nud = CreateNuD(iv, prop.Name + "_" + i);
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
                            var nud = CreateCheckBox(b, prop.Name + "_" + i, prop.Name);
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
                            if (prop.Name.Equals("username", StringComparison.OrdinalIgnoreCase) ||
                                prop.Name.Equals("channel", StringComparison.OrdinalIgnoreCase))
                            {
                                var nud = CreateTextPreviewControl(prop.Name + "_" + i, s);
                                nud.Leave += (o, e) =>
                                {
                                    prop.SetValue(ca, nud.Text);
                                    isSaved = false;
                                };
                                nud.TextChanged += (o, e) =>
                                {
                                    if (nud.Text.Equals("*"))
                                    {
                                        prop.SetValue(ca, nud.Text);
                                    }
                                    else
                                    {
                                        if (prop.Name.Equals("channel", StringComparison.OrdinalIgnoreCase))
                                        {
                                            nud.SearchArray(ChatClient.Channels.Select(t => t.Key));
                                        }
                                        else
                                        {
                                            nud.SearchArray(UserNames);
                                        }
                                    }
                                    isSaved = false;
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
                                var nud = CreateRtB(prop.Name + "_" + i, s);
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
                    x = 5;
                    var g = panel2.CreateGraphics();
                    for (int o = 0; o < list.Count; o++)
                    {
                        var ctrl = list[o];
                        ctrl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                        ctrl.Location = new System.Drawing.Point(x, y);
                        if (o > 0 && !(ctrl is NumericUpDown)) ctrl.Width = (int)addFacSize;
                        if (fillLabels) //labels.Add(new Label() { Text = ctrl.Name, Location = new System.Drawing.Point(x, 0) });
                            labels.Add(new Tuple<string, int>(ctrl.Name.Split('_').First(), x));
                        x += ctrl.Width + 5;
                        panel2.Controls.Add(ctrl);
                    }
                    g.Dispose();
                    fillLabels = false;

                    y += height + 5;
                }
                this.Refresh();
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
            panel2.Paint += panel2_Paint;
            LoadActions(true);

            this.Text = "Chat Actions";
        }

        private void LoadActions(bool withuser)
        {
            try
            {
                actions = ChatClient.AutoActions.GetChannelActions();

                CreateChatActionControls();

                if (withuser)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        UserNames = ChatClient.Users.GetAllNames();
                    });
                }
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            foreach (var lbl in labels)
            {
                e.Graphics.DrawString(lbl.Item1, this.Font, Brushes.White, lbl.Item2, 5);
            }
        }
    }
}