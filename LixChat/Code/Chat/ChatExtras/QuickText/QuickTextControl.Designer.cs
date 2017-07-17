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
            this.SuspendLayout();
            // 
            // lstB_Main
            // 
            this.lstB_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstB_Main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lstB_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstB_Main.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstB_Main.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstB_Main.FormattingEnabled = true;
            this.lstB_Main.ItemHeight = 16;
            this.lstB_Main.Location = new System.Drawing.Point(0, 0);
            this.lstB_Main.Name = "lstB_Main";
            this.lstB_Main.Size = new System.Drawing.Size(479, 498);
            this.lstB_Main.TabIndex = 0;
            // 
            // rtb_add
            // 
            this.rtb_add.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtb_add.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.rtb_add.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtb_add.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_add.Location = new System.Drawing.Point(0, 504);
            this.rtb_add.Name = "rtb_add";
            this.rtb_add.Size = new System.Drawing.Size(479, 68);
            this.rtb_add.TabIndex = 1;
            this.rtb_add.Text = "";
            this.rtb_add.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // btn_remove
            // 
            this.btn_remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_remove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btn_remove.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_remove.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_remove.Location = new System.Drawing.Point(132, 574);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(128, 30);
            this.btn_remove.TabIndex = 2;
            this.btn_remove.Text = "Remove Quick Text";
            this.btn_remove.UseVisualStyleBackColor = false;
            // 
            // btn_Add
            // 
            this.btn_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Add.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btn_Add.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Add.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_Add.Location = new System.Drawing.Point(3, 574);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(123, 30);
            this.btn_Add.TabIndex = 3;
            this.btn_Add.Text = "Add Quick Text";
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // QuickTextControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.btn_remove);
            this.Controls.Add(this.rtb_add);
            this.Controls.Add(this.lstB_Main);
            this.Name = "QuickTextControl";
            this.Size = new System.Drawing.Size(479, 604);
            this.Load += new System.EventHandler(this.QuickTextControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstB_Main;
        private System.Windows.Forms.RichTextBox rtb_add;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.Button btn_Add;
    }
}
