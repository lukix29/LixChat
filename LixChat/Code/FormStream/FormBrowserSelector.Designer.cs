namespace LX29_Twitch.Forms
{
    partial class FormBrowserSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBrowserSelector));
            this.listView1 = new System.Windows.Forms.ListView();
            this.btn_Select = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.listView1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.ForeColor = System.Drawing.Color.Gainsboro;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 35);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(307, 254);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Tile;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // btn_Select
            // 
            this.btn_Select.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Select.AutoSize = true;
            this.btn_Select.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Select.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Select.Location = new System.Drawing.Point(12, 295);
            this.btn_Select.Name = "btn_Select";
            this.btn_Select.Size = new System.Drawing.Size(102, 28);
            this.btn_Select.TabIndex = 1;
            this.btn_Select.Text = "Open Browser";
            this.btn_Select.UseVisualStyleBackColor = true;
            this.btn_Select.Click += new System.EventHandler(this.btn_Select_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(310, 33);
            this.label1.TabIndex = 4;
            this.label1.Text = "Choose Browser for Login";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormBrowserSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(331, 330);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Select);
            this.Controls.Add(this.listView1);
            this.ForeColor = System.Drawing.Color.Gainsboro;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormBrowserSelector";
            this.ShowIcon = false;
            this.Text = "Browser Selector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBrowserSelector_FormClosing);
            this.Load += new System.EventHandler(this.FormBrowserSelector_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btn_Select;
        private System.Windows.Forms.Label label1;

    }
}