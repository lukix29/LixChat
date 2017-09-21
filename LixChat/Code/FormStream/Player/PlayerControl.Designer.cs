﻿namespace LX29_LixChat.Code.FormStream.Player
{
    partial class PlayerControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.panelVideoControls = new System.Windows.Forms.Panel();
            this.cB_Mute = new System.Windows.Forms.CheckBox();
            this.comBox_previewQuali = new System.Windows.Forms.ComboBox();
            this.cB_OnTop = new System.Windows.Forms.CheckBox();
            this.cB_Borderless = new System.Windows.Forms.CheckBox();
            this.volumeControl1 = new LX29_Twitch.Forms.VolumeControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cb_Pause = new System.Windows.Forms.CheckBox();
            this.panelVideoControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelVideo
            // 
            this.panelVideo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelVideo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.panelVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVideo.Location = new System.Drawing.Point(0, 0);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(874, 609);
            this.panelVideo.TabIndex = 19;
            this.panelVideo.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelVideo_MouseDoubleClick);
            this.panelVideo.MouseHover += new System.EventHandler(this.panelVideo_MouseHover);
            this.panelVideo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelVideo_MouseMove_1);
            // 
            // panelVideoControls
            // 
            this.panelVideoControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideoControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelVideoControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.panelVideoControls.Controls.Add(this.cb_Pause);
            this.panelVideoControls.Controls.Add(this.cB_Mute);
            this.panelVideoControls.Controls.Add(this.comBox_previewQuali);
            this.panelVideoControls.Controls.Add(this.cB_OnTop);
            this.panelVideoControls.Controls.Add(this.cB_Borderless);
            this.panelVideoControls.Controls.Add(this.volumeControl1);
            this.panelVideoControls.Location = new System.Drawing.Point(0, 573);
            this.panelVideoControls.Name = "panelVideoControls";
            this.panelVideoControls.Size = new System.Drawing.Size(874, 35);
            this.panelVideoControls.TabIndex = 18;
            // 
            // cB_Mute
            // 
            this.cB_Mute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cB_Mute.Appearance = System.Windows.Forms.Appearance.Button;
            this.cB_Mute.BackColor = System.Drawing.Color.Black;
            this.cB_Mute.BackgroundImage = global::LX29_LixChat.Properties.Resources.noMute;
            this.cB_Mute.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.cB_Mute.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cB_Mute.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cB_Mute.ForeColor = System.Drawing.Color.Gainsboro;
            this.cB_Mute.Location = new System.Drawing.Point(126, 7);
            this.cB_Mute.Name = "cB_Mute";
            this.cB_Mute.Size = new System.Drawing.Size(21, 20);
            this.cB_Mute.TabIndex = 16;
            this.cB_Mute.UseVisualStyleBackColor = false;
            this.cB_Mute.CheckedChanged += new System.EventHandler(this.cB_Mute_CheckedChanged);
            // 
            // comBox_previewQuali
            // 
            this.comBox_previewQuali.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comBox_previewQuali.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.comBox_previewQuali.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comBox_previewQuali.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comBox_previewQuali.ForeColor = System.Drawing.Color.Gainsboro;
            this.comBox_previewQuali.FormattingEnabled = true;
            this.comBox_previewQuali.Location = new System.Drawing.Point(33, 5);
            this.comBox_previewQuali.Name = "comBox_previewQuali";
            this.comBox_previewQuali.Size = new System.Drawing.Size(86, 24);
            this.comBox_previewQuali.TabIndex = 7;
            this.comBox_previewQuali.SelectedIndexChanged += new System.EventHandler(this.comBox_previewQuali_SelectedIndexChanged);
            // 
            // cB_OnTop
            // 
            this.cB_OnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cB_OnTop.AutoSize = true;
            this.cB_OnTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cB_OnTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cB_OnTop.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cB_OnTop.ForeColor = System.Drawing.Color.White;
            this.cB_OnTop.Location = new System.Drawing.Point(370, 7);
            this.cB_OnTop.Name = "cB_OnTop";
            this.cB_OnTop.Size = new System.Drawing.Size(65, 20);
            this.cB_OnTop.TabIndex = 9;
            this.cB_OnTop.Text = "On Top";
            this.cB_OnTop.UseVisualStyleBackColor = false;
            this.cB_OnTop.CheckedChanged += new System.EventHandler(this.cB_OnTop_CheckedChanged);
            // 
            // cB_Borderless
            // 
            this.cB_Borderless.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cB_Borderless.AutoSize = true;
            this.cB_Borderless.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cB_Borderless.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cB_Borderless.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cB_Borderless.ForeColor = System.Drawing.Color.White;
            this.cB_Borderless.Location = new System.Drawing.Point(441, 7);
            this.cB_Borderless.Name = "cB_Borderless";
            this.cB_Borderless.Size = new System.Drawing.Size(86, 20);
            this.cB_Borderless.TabIndex = 10;
            this.cB_Borderless.Text = "Borderless";
            this.cB_Borderless.UseVisualStyleBackColor = false;
            this.cB_Borderless.CheckedChanged += new System.EventHandler(this.cB_Borderless_CheckedChanged);
            // 
            // volumeControl1
            // 
            this.volumeControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volumeControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.volumeControl1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volumeControl1.ForeColor = System.Drawing.Color.Gray;
            this.volumeControl1.Location = new System.Drawing.Point(150, 7);
            this.volumeControl1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.volumeControl1.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.volumeControl1.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.volumeControl1.Name = "volumeControl1";
            this.volumeControl1.Size = new System.Drawing.Size(214, 20);
            this.volumeControl1.TabIndex = 12;
            this.volumeControl1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.volumeControl1.ValueChanged += new System.EventHandler(this.volumeControl1_ValueChanged_1);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // btn_Pause
            // 
            this.cb_Pause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_Pause.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb_Pause.BackColor = System.Drawing.Color.Black;
            this.cb_Pause.BackgroundImage = global::LX29_LixChat.Properties.Resources.pause;
            this.cb_Pause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cb_Pause.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_Pause.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cb_Pause.Location = new System.Drawing.Point(5, 5);
            this.cb_Pause.Name = "btn_Pause";
            this.cb_Pause.Size = new System.Drawing.Size(24, 24);
            this.cb_Pause.TabIndex = 0;
            this.cb_Pause.UseVisualStyleBackColor = false;
            this.cb_Pause.CheckedChanged += new System.EventHandler(this.btn_Pause_CheckedChanged);
            // 
            // PlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.panelVideoControls);
            this.Controls.Add(this.panelVideo);
            this.Name = "PlayerControl";
            this.Size = new System.Drawing.Size(874, 609);
            this.panelVideoControls.ResumeLayout(false);
            this.panelVideoControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelVideo;
        private System.Windows.Forms.Panel panelVideoControls;
        private System.Windows.Forms.ComboBox comBox_previewQuali;
        private System.Windows.Forms.CheckBox cB_OnTop;
        private System.Windows.Forms.CheckBox cB_Borderless;
        private LX29_Twitch.Forms.VolumeControl volumeControl1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cB_Mute;
        private System.Windows.Forms.CheckBox cb_Pause;
    }
}
