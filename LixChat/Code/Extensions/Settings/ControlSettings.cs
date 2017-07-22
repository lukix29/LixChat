using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace LX29_ChatClient.Forms
{
    public partial class ControlSettings : UserControl
    {
        //[ReadOnly(true)]
        //[Browsable(false)]
        //public new readonly Color BackColor = Color.FromArgb(40, 40, 50);

        public ControlSettings()
        {
            InitializeComponent();
        }

        public ChatView ChatView
        {
            get;
            private set;
        }

        // [System.ComponentModel.DefaultValue(true)]
        //public bool ShowAllSettings
        //{
        //    get { return !splitContainer1.Panel2Collapsed; }
        //    set
        //    {
        //        splitContainer1.Panel2Collapsed = !value;
        //    }
        //}

        public static Panel GetSettingPanel(Size clientSize, double value, SettingClasses classe, Action<decimal, string> onselect)
        {
            //
            // nUD_LineHeight
            //
            NumericUpDown nUD_LineHeight = new NumericUpDown();
            nUD_LineHeight.AutoSize = true;
            nUD_LineHeight.Name = "nUD_" + classe.Name;
            nUD_LineHeight.BackColor = System.Drawing.Color.Black;
            nUD_LineHeight.BorderStyle = System.Windows.Forms.BorderStyle.None;
            nUD_LineHeight.DecimalPlaces = classe.DecimalPlaces;
            nUD_LineHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            nUD_LineHeight.ForeColor = System.Drawing.Color.Gainsboro;
            nUD_LineHeight.Increment = (decimal)classe.Inc;
            nUD_LineHeight.Dock = DockStyle.Right;
            nUD_LineHeight.Location = new System.Drawing.Point(clientSize.Width - nUD_LineHeight.Width, 3);
            nUD_LineHeight.Minimum = (decimal)classe.Min;
            nUD_LineHeight.Maximum = (decimal)classe.Max;
            //nUD_LineHeight.Size = new System.Drawing.Size(56, 18);
            nUD_LineHeight.TabIndex = 2;
            nUD_LineHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            nUD_LineHeight.Value = (decimal)value;
            nUD_LineHeight.ValueChanged += (sender, e) =>
                {
                    var nud = (NumericUpDown)sender;
                    onselect(nud.Value, classe.Name);
                };
            //
            // label1
            // #
            Label label1 = new Label();
            label1.AutoSize = true;
            label1.Font = nUD_LineHeight.Font;
            label1.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            label1.Location = new System.Drawing.Point(3, 10);
            label1.Name = "lbl_" + classe.Name;
            label1.Dock = DockStyle.Left;
            label1.Text = classe.Text;

            Panel panel1 = new Panel();
            panel1.Controls.Add(nUD_LineHeight);
            panel1.Controls.Add(label1);
            panel1.Name = "panel_" + classe.Name;
            // panel1.AutoSize = true;
            panel1.Visible = true;
            panel1.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            panel1.Location = new Point(0, 0);
            panel1.Size = new Size(clientSize.Width, nUD_LineHeight.Height);
            return panel1;
        }

        public new void BringToFront()
        {
            this.Show(null);
        }

        public void Show(ChatView chatView)
        {
            try
            {
                this.ChatView = chatView;
                this.Visible = !this.Visible;
                if (this.Visible)
                {
                    splitContainer1.SplitterDistance = groupBox1.Height;

                    SetControls(flowLayoutPanel_TextOptions, SettingClasses.TextBasic);

                    SetControls(flowLayoutPanel_RenderOptions, SettingClasses.EmoteBasic, cB_AnimatedEmotes);

                    SetControls(flowLayoutPanel_UserOptions, SettingClasses.UserBasic);

                    cb_ShowErrors.Checked = Settings.ShowErrors;
                    cB_AnimatedEmotes.Checked = Settings.AnimatedEmotes;

                    rTB_HighlightWords.Clear();
                    foreach (var hl in ChatClient.ChatHighlights)
                    {
                        rTB_HighlightWords.AppendText(hl + "\r\n");
                    }

                    base.BringToFront();
                }
                else
                {
                    Settings.Save();
                }
            }
            catch
            {
            }
        }

        private void btn_OpenScriptFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Settings.caonfigBaseDir);
        }

        private void btn_ReloadChat_Click(object sender, EventArgs e)
        {
        }

        private void btn_SaveHL_Click(object sender, EventArgs e)
        {
            ChatClient.ClearChatHighlightWord();
            foreach (var s in rTB_HighlightWords.Lines)
            {
                ChatClient.AddChatHighlightWord(s);
            }
            rTB_HighlightWords.Clear();
            foreach (var hl in ChatClient.ChatHighlights)
            {
                rTB_HighlightWords.AppendText(hl + "\r\n");
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
                Font oldFont = ChatView.Font;
                fd.Font = ChatView.Font;
                fd.AllowVerticalFonts = false;
                fd.FontMustExist = true;
                fd.ShowEffects = false;
                fd.ShowColor = false;
                fd.AllowVectorFonts = true;
                fd.AllowSimulations = true;
                fd.ShowApply = true;
                fd.Apply += delegate(object sender0, EventArgs e0)
                {
                    if (ChatView != null)
                    {
                        //ChatView.FontBaseSize = fd.Font.Size;
                        ChatView.Font = fd.Font;
                    }
                };

                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (ChatView != null)
                    {
                        //ChatView.FontBaseSize = fd.Font.Size;
                        ChatView.Font = fd.Font;
                    }
                    Settings.ChatFontSize = ChatView.Font.Size;
                    Settings.ChatFontName = ChatView.Font.Name;
                }
                else
                {
                    if (ChatView != null)
                    {
                        //ChatView.FontBaseSize = oldFont.Size;
                        ChatView.Font = oldFont;
                    }
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
            this.Visible = false;
        }

        private void cB_AnimatedEmotes_CheckedChanged(object sender, EventArgs e)
        {
            Settings.AnimatedEmotes = cB_AnimatedEmotes.Checked;
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

        private void ControlSettings_SizeChanged(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = groupBox1.Bottom + 5;
        }

        private void flowLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {
        }

        private void SetControls(Control c, SettingClasses[] keys, params Control[] extra)
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
                    double value = Math.Max(set.Min, Math.Min(set.Max, (double)set.Value));
                    c.Controls.Add(GetSettingPanel(c.ClientSize,
                        value, set, (a, s) =>
                        {
                            Settings.SetValue(set, (double)a);
                            if (s.Equals("_ChatFontSize") && ChatView != null)
                            {
                                ChatView.SetFontSize(a);
                            }
                        }));
                }
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