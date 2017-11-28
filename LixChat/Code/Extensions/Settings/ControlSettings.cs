using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class ControlSettings : UserControl
    {
        public ControlSettings()
        {
            InitializeComponent();

            foreach (Control c in this.Controls)
            {
                UpdateColorControls(c);
            }
        }

        public Control GetSettingCheckBox(Size clientSize, bool value, SettingClasses classe, Action<bool, string> onselect)
        {
            CheckBox cb_value = new CheckBox();
            cb_value.AutoSize = true;
            cb_value.Name = "_" + classe.Name;
            cb_value.BackColor = System.Drawing.Color.Black;
            cb_value.Font = this.Font;
            cb_value.AutoSize = true;
            cb_value.ForeColor = System.Drawing.Color.Gainsboro;
            cb_value.Location = new System.Drawing.Point(clientSize.Width - cb_value.Width, 3);
            cb_value.TabIndex = 2;
            cb_value.Checked = value;
            cb_value.CheckedChanged += (sender, e) =>
            {
                var nud = (CheckBox)sender;
                onselect(nud.Checked, classe.Name);
            };
            cb_value.MouseDoubleClick += (sender, e) =>
            {
                var nud = (CheckBox)sender;
                var val = Settings.Standard.FirstOrDefault(t => t.Key.Equals(nud.Name));
                nud.Checked = (bool)val.Value;
            };
            cb_value.Dock = DockStyle.Left;
            cb_value.Text = classe.Text;
            return cb_value;
            //Panel panel1 = new Panel();
            //panel1.Controls.Add(cb_value);
            //panel1.Name = "panel_" + classe.Name;
            //panel1.Visible = true;
            //panel1.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            //panel1.Location = new Point(0, 0);
            //panel1.Size = new Size(clientSize.Width, cb_value.Height);
            //return panel1;
        }

        //public new void BringToFront()
        //{
        //    this.Show(null);
        //}
        public Control GetSettingPanel(Size clientSize, double value, SettingClasses classe, Action<decimal, string> onselect)
        {
            //
            // nUD_LineHeight
            //
            NumericUpDown nUD_LineHeight = new NumericUpDown();
            nUD_LineHeight.AutoSize = true;
            nUD_LineHeight.Name = classe.Name;
            nUD_LineHeight.BackColor = System.Drawing.Color.Black;
            nUD_LineHeight.BorderStyle = System.Windows.Forms.BorderStyle.None;
            nUD_LineHeight.DecimalPlaces = classe.DecimalPlaces;
            nUD_LineHeight.Font = this.Font;
            nUD_LineHeight.ForeColor = this.ForeColor;
            nUD_LineHeight.Increment = (decimal)classe.Inc;
            nUD_LineHeight.Dock = DockStyle.Right;
            nUD_LineHeight.Location = new System.Drawing.Point(clientSize.Width - nUD_LineHeight.Width, 3);
            nUD_LineHeight.Minimum = (decimal)classe.Min;
            nUD_LineHeight.Maximum = (decimal)classe.Max;
            nUD_LineHeight.AutoSize = true;
            //nUD_LineHeight.Size = new System.Drawing.Size(56, 18);
            nUD_LineHeight.TabIndex = 2;
            nUD_LineHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            nUD_LineHeight.Value = (decimal)value;
            nUD_LineHeight.ValueChanged += (sender, e) =>
                {
                    var nud = (NumericUpDown)sender;
                    onselect(nud.Value, classe.Name);
                };

            nUD_LineHeight.MouseClick += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Middle)
                    {
                        var nud = (NumericUpDown)sender;
                        var val = Settings.Standard.FirstOrDefault(t => t.Key.Equals(nud.Name));
                        nud.Value = (decimal)(double)val.Value;
                    }
                };
            //
            // label1
            // #
            Label label1 = new Label();
            label1.AutoSize = true;
            label1.Font = nUD_LineHeight.Font;
            label1.BackColor = nUD_LineHeight.BackColor;
            label1.Location = new System.Drawing.Point(3, 10);
            label1.Name = "lbl_" + classe.Name;
            label1.Dock = DockStyle.Left;
            label1.Text = classe.Text;
            label1.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Middle)
                {
                    var val = Settings.Standard.FirstOrDefault(t => t.Key.Equals(nUD_LineHeight.Name));
                    nUD_LineHeight.Value = (decimal)(double)val.Value;
                }
            };

            Panel panel1 = new Panel();
            panel1.Controls.Add(nUD_LineHeight);
            panel1.Controls.Add(label1);
            panel1.Name = "panel_" + classe.Name;
            // panel1.AutoSize = true;
            panel1.Visible = true;
            //panel1.AutoSize = true;
            panel1.BackColor = nUD_LineHeight.BackColor;
            panel1.Location = new Point(0, 0);
            panel1.Size = new Size(clientSize.Width, nUD_LineHeight.Height);
            return panel1;
        }

        public void SetChatViewFont(Font font)
        {
            foreach (var chanel in ChatClient.Channels.Values)
            {
                try
                {
                    chanel.ChatForm.chatView.Font = font;
                }
                catch { }
            }
        }

        public void SetChatViewFont(decimal a)
        {
            foreach (var channel in ChatClient.Channels)
            {
                if (channel.Value.ChatForm != null)
                {
                    channel.Value.ChatForm.chatView.SetFontSize(a);
                }
            }
        }

        //public new void Show()
        //{
        //    try
        //    {
        //        this.Visible = !this.Visible;
        //        if (this.Visible)
        //        {
        //            base.BringToFront();
        //        }
        //        else
        //        {
        //            Settings.Save();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        public void UpdateColorControls(Control myControl)
        {
            try
            {
                foreach (Control subC in myControl.Controls)
                {
                    UpdateColorControls(subC);
                }
            }
            catch
            { }
            try
            {
                myControl.Font = new Font("Consolas", 10f);
                myControl.ForeColor = this.ForeColor;
            }
            catch { }
            //try
            //{
            //    var props = myControl.GetType().GetProperties(System.Reflection.BindingFlags.Public).FirstOrDefault(t => t.Name.Equals("AutoSize"));
            //    if (props != null)
            //    {
            //    }
            //}
            //catch { }
        }

        private void btn_OpenScriptFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Settings._caonfigBaseDir);
        }

        private void btn_ReloadChat_Click(object sender, EventArgs e)
        {
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            foreach (var set in Settings.Standard)
            {
                Settings.SetValue(set.Key, set.Value);
            }
            SetControlSettings();
            //var controls = flowLayoutPanel5.Controls.Cast<Control>().SelectMany<Control, Control>(t => t.Controls.Cast<Control>()).ToArray();
        }

        private void btn_SaveHL_Click(object sender, EventArgs e)
        {
            ChatClient.ClearChatHighlightWord();
            var arr = rTB_HighlightWords.Text.Split('\n', '\r');
            foreach (var s in arr)
            {
                ChatClient.AddChatHighlightWord(s);
            }
            rTB_HighlightWords.Clear();
            foreach (var hl in ChatClient.ChatHighlights)
            {
                rTB_HighlightWords.AppendText(hl + "\n");
            }
        }

        private void btn_SelectBrowser_Click(object sender, EventArgs e)
        {
            Settings.GetBrowserPath();
        }

        private void btn_SelectChatBG_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.SolidColorOnly = true;
            cd.FullOpen = true;
            cd.CustomColors = new int[] { Settings.ChatBackGround };
            switch (cd.ShowDialog())
            {
                case DialogResult.OK:
                    Settings.ChatBackGround = cd.Color.ToArgb();
                    break;
            }
        }

        private void btn_SelectFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            try
            {
                fd.Font = new Font(Settings.ChatFontName, (float)Settings.ChatFontSize);
                fd.AllowVerticalFonts = false;
                fd.FontMustExist = true;
                fd.ShowEffects = false;
                fd.ShowColor = false;
                fd.AllowVectorFonts = true;
                fd.AllowSimulations = true;
                fd.ShowApply = true;

                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetChatViewFont(fd.Font);
                    Settings.ChatFontSize = fd.Font.Size;
                    Settings.ChatFontName = fd.Font.Name;
                }
            }
            catch (Exception x)
            {
                try
                {
                    fd.Dispose();
                }
                catch
                {
                }
                LX29_MessageBox.Show(x.Message);
            }
            finally
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ((Form)this.Parent).Close();
        }

        private void cB_DrawThreaded_CheckedChanged(object sender, EventArgs e)
        {
            //if (ChatView != null)
            //    ChatView.DrawThreaded = cB_DrawThreaded.Checked;
            //LX29_Helpers.Settings.ThreadedRendering = cB_DrawThreaded.Checked;
        }

        private void cb_ShowErrors_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowErrors = cb_ShowErrors.Checked;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int i = e.Index;
            var item = checkedListBox1.Items[i].ToString();
            var enabled = e.NewValue == CheckState.Checked;
            ChatClient.Emotes.Badges.SetEnabled(item, enabled);
        }

        private void ControlSettings_Load(object sender, EventArgs e)
        {
            rTB_HighlightWords.AddContextMenu();
            SetControlSettings();
        }

        private void ControlSettings_SizeChanged(object sender, EventArgs e)
        {
        }

        private void flowLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {
        }

        private void flowLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {
        }

        private void rTB_HighlightWords_TextChanged(object sender, EventArgs e)
        {
        }

        private void SetControls(Control c, SettingClasses[] keys, params Control[] extra)
        {
            try
            {
                c.Controls.Clear();
                foreach (var ci in extra)
                {
                    c.Controls.Add(ci);
                }
                var textsetts = Settings.GetFields(keys);
                foreach (var set in textsetts)
                {
                    if (set.Value is double)
                    {
                        double value = Math.Max((double)set.Min, Math.Min((double)set.Max, (double)set.Value));
                        c.Controls.Add(GetSettingPanel(c.ClientSize,
                            value, set, (a, s) =>
                            {
                                Settings.SetValue(set, (double)a);
                                if (s.Equals("_ChatFontSize"))
                                {
                                    SetChatViewFont(a);
                                }
                            }));
                    }
                    else if (set.Value is bool)
                    {
                        bool value = (bool)set.Value;
                        c.Controls.Add(GetSettingCheckBox(c.ClientSize,
                            value, set, (a, s) =>
                            {
                                Settings.SetValue(set, a);
                            }));
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        SetControls(c, keys, extra);
                        break;
                }
            }
        }

        private void SetControlSettings()
        {
            try
            {
                SetControls(flowLayoutPanel_TextOptions, SettingClasses.TextBasic);

                SetControls(flowLayoutPanel_RenderOptions, SettingClasses.EmoteBasic);

                SetControls(flowLayoutPanel_UserOptions, SettingClasses.UserBasic);

                SetControls(flowLayoutPanel_ChatOptions, SettingClasses.ChatBasic, btn_SelectChatBG, btn_SelectFont);

                SetControls(flowLayoutPanel_PlayerOptions, SettingClasses.PlayerBasic);

                cb_ShowErrors.Checked = Settings.ShowErrors;

                rTB_HighlightWords.Clear();
                foreach (var hl in ChatClient.ChatHighlights)
                {
                    rTB_HighlightWords.AppendText(hl + "\r\n");
                }
                try
                {
                    checkedListBox1.Items.Clear();
                    foreach (var badge in ChatClient.Emotes.Badges)
                    {
                        checkedListBox1.Items.Add(badge, true);
                    }
                }
                catch
                {
                }
            }
            catch (Exception x)
            {
                x.Handle("", true);
                this.btn_Close.PerformClick();
            }
        }

        //private void trackB_UserBrightness_Load(object sender, EventArgs e)
        //{
        //    this.trackB_UserBrightness.TextSelector =
        //        new System.Func<decimal, string>(t => ((t * 100).ToString("F0") + "%"));
        //}

        //private void trackB_UserBrightness_ValueChanged(object sender, EventArgs e)
        //{
        //    lbl_brightnessInfo.Text = "User Color Brightness " + trackB_UserBrightness.Value + "%";
        //    Settings.UserColorBrigthness = trackB_UserBrightness.Value;
        //}
    }
}