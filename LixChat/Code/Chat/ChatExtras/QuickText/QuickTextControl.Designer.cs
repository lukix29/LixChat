namespace LX29_ChatClient.Addons
{
    partial class QuickTextControl
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
            this.lstB_Main = new System.Windows.Forms.ListBox();
            this.rtb_add = new System.Windows.Forms.RichTextBox();
            this.btn_remove = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            this.txtB_Alias = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstB_Main
            // 
            this.lstB_Main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lstB_Main.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstB_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstB_Main.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstB_Main.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstB_Main.FormattingEnabled = true;
            this.lstB_Main.IntegralHeight = false;
            this.lstB_Main.ItemHeight = 19;
            this.lstB_Main.Location = new System.Drawing.Point(0, 0);
            this.lstB_Main.Margin = new System.Windows.Forms.Padding(0);
            this.lstB_Main.Name = "lstB_Main";
            this.lstB_Main.Size = new System.Drawing.Size(479, 524);
            this.lstB_Main.TabIndex = 0;
            this.lstB_Main.SelectedIndexChanged += new System.EventHandler(this.lstB_Main_SelectedIndexChanged);
            // 
            // rtb_add
            // 
            this.rtb_add.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtb_add.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.rtb_add.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtb_add.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_add.ForeColor = System.Drawing.Color.Gainsboro;
            this.rtb_add.Location = new System.Drawing.Point(118, 4);
            this.rtb_add.Name = "rtb_add";
            this.rtb_add.Size = new System.Drawing.Size(361, 51);
            this.rtb_add.TabIndex = 1;
            this.rtb_add.Text = "";
            this.rtb_add.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // btn_remove
            // 
            this.btn_remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_remove.AutoSize = true;
            this.btn_remove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btn_remove.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_remove.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_remove.Location = new System.Drawing.Point(90, 61);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(100, 28);
            this.btn_remove.TabIndex = 2;
            this.btn_remove.Text = "Remove Entry";
            this.btn_remove.UseVisualStyleBackColor = false;
            this.btn_remove.Click += new System.EventHandler(this.btn_remove_Click);
            // 
            // btn_Add
            // 
            this.btn_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Add.AutoSize = true;
            this.btn_Add.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btn_Add.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Add.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_Add.Location = new System.Drawing.Point(5, 61);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(82, 28);
            this.btn_Add.TabIndex = 3;
            this.btn_Add.Text = "Add Entry";
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // txtB_Alias
            // 
            this.txtB_Alias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtB_Alias.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtB_Alias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtB_Alias.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtB_Alias.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtB_Alias.Location = new System.Drawing.Point(6, 28);
            this.txtB_Alias.Name = "txtB_Alias";
            this.txtB_Alias.Size = new System.Drawing.Size(106, 26);
            this.txtB_Alias.TabIndex = 5;
            this.txtB_Alias.TextChanged += new System.EventHandler(this.txtB_Alias_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox2.BackColor = System.Drawing.Color.Black;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.Gainsboro;
            this.textBox2.Location = new System.Drawing.Point(6, 7);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(106, 19);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "Text Alias:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rtb_add);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.btn_remove);
            this.panel1.Controls.Add(this.txtB_Alias);
            this.panel1.Controls.Add(this.btn_Add);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(479, 90);
            this.panel1.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstB_Main);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2MinSize = 90;
            this.splitContainer1.Size = new System.Drawing.Size(479, 615);
            this.splitContainer1.SplitterDistance = 524;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 8;
            // 
            // QuickTextControl
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.splitContainer1);
            this.Name = "QuickTextControl";
            this.Size = new System.Drawing.Size(479, 615);
            this.Load += new System.EventHandler(this.QuickTextControl_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstB_Main;
        private System.Windows.Forms.RichTextBox rtb_add;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.TextBox txtB_Alias;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
