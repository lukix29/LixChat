namespace LX29_ChatClient.Addons
{
    partial class FormChatActionsQuick
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.autoChatActionsControl1 = new LX29_ChatClient.Addons.AutoChatActionsControl();
            this.quickTextControl1 = new LX29_ChatClient.Addons.QuickTextControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.ItemSize = new System.Drawing.Size(58, 18);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(750, 637);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabPage1.Controls.Add(this.autoChatActionsControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(742, 611);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Auto Actions";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabPage2.Controls.Add(this.quickTextControl1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(742, 611);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Quick Text";
            // 
            // autoChatActionsControl1
            // 
            this.autoChatActionsControl1.BackColor = System.Drawing.Color.Black;
            this.autoChatActionsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoChatActionsControl1.ForeColor = System.Drawing.Color.White;
            this.autoChatActionsControl1.Location = new System.Drawing.Point(3, 3);
            this.autoChatActionsControl1.Name = "autoChatActionsControl1";
            this.autoChatActionsControl1.Size = new System.Drawing.Size(736, 605);
            this.autoChatActionsControl1.TabIndex = 0;
            // 
            // quickTextControl1
            // 
            this.quickTextControl1.BackColor = System.Drawing.Color.Black;
            this.quickTextControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quickTextControl1.Location = new System.Drawing.Point(3, 3);
            this.quickTextControl1.Margin = new System.Windows.Forms.Padding(0);
            this.quickTextControl1.Name = "quickTextControl1";
            this.quickTextControl1.Size = new System.Drawing.Size(736, 605);
            this.quickTextControl1.TabIndex = 0;
            // 
            // FormChatActionsQuick
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(750, 637);
            this.Controls.Add(this.tabControl1);
            this.ForeColor = System.Drawing.Color.Gainsboro;
            this.Name = "FormChatActionsQuick";
            this.Text = "FormChatActionsQuick";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormChatActionsQuick_FormClosing);
            this.Load += new System.EventHandler(this.FormChatActionsQuick_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private LX29_ChatClient.Addons.AutoChatActionsControl autoChatActionsControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private QuickTextControl quickTextControl1;
    }
}