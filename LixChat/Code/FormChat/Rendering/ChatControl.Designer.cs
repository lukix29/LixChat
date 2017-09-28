namespace LX29_ChatClient.Forms
{
    partial class ChatView
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
            this.cMS_TextOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tSMi_Text = new System.Windows.Forms.ToolStripTextBox();
            this.tSMi_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_Search = new System.Windows.Forms.ToolStripMenuItem();
            this.lbl_ScrollDown = new System.Windows.Forms.Label();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.cMS_TextOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // cMS_TextOptions
            // 
            this.cMS_TextOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMi_Text,
            this.tSMi_Copy,
            this.tSMi_Search});
            this.cMS_TextOptions.Name = "cMS_TextOptions";
            this.cMS_TextOptions.Size = new System.Drawing.Size(211, 73);
            // 
            // tSMi_Text
            // 
            this.tSMi_Text.Font = new System.Drawing.Font("Arial", 10F);
            this.tSMi_Text.Name = "tSMi_Text";
            this.tSMi_Text.ReadOnly = true;
            this.tSMi_Text.Size = new System.Drawing.Size(150, 23);
            // 
            // tSMi_Copy
            // 
            this.tSMi_Copy.Name = "tSMi_Copy";
            this.tSMi_Copy.Size = new System.Drawing.Size(210, 22);
            this.tSMi_Copy.Text = "Copy";
            // 
            // tSMi_Search
            // 
            this.tSMi_Search.Name = "tSMi_Search";
            this.tSMi_Search.Size = new System.Drawing.Size(210, 22);
            this.tSMi_Search.Text = "Search Google";
            // 
            // lbl_ScrollDown
            // 
            this.lbl_ScrollDown.BackColor = System.Drawing.Color.DimGray;
            this.lbl_ScrollDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_ScrollDown.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbl_ScrollDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ScrollDown.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lbl_ScrollDown.Location = new System.Drawing.Point(0, 549);
            this.lbl_ScrollDown.Name = "lbl_ScrollDown";
            this.lbl_ScrollDown.Size = new System.Drawing.Size(492, 21);
            this.lbl_ScrollDown.TabIndex = 1;
            this.lbl_ScrollDown.Text = "↓ More Messages below. ↓";
            this.lbl_ScrollDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_ScrollDown.Visible = false;
            this.lbl_ScrollDown.Click += new System.EventHandler(this.lbl_ScrollDown_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(181, 345);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(200, 100);
            this.elementHost1.TabIndex = 2;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Visible = false;
            this.elementHost1.Child = null;
            // 
            // ChatView
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.lbl_ScrollDown);
            this.Name = "ChatView";
            this.Size = new System.Drawing.Size(492, 570);
            this.MouseLeave += new System.EventHandler(this.ChatView_MouseLeave);
            this.cMS_TextOptions.ResumeLayout(false);
            this.cMS_TextOptions.PerformLayout();
            this.ResumeLayout(false);

        }




        #endregion

        private System.Windows.Forms.ContextMenuStrip cMS_TextOptions;
        private System.Windows.Forms.ToolStripMenuItem tSMi_Copy;
        private System.Windows.Forms.ToolStripMenuItem tSMi_Search;
        private System.Windows.Forms.ToolStripTextBox tSMi_Text;
        private System.Windows.Forms.Label lbl_ScrollDown;
        private System.Windows.Forms.Integration.ElementHost elementHost1;

    }
}
