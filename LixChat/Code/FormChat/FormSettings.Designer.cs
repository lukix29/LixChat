namespace LX29_ChatClient.Forms
{
    partial class FormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.controlSettings1 = new LX29_ChatClient.Forms.ControlSettings();
            this.SuspendLayout();
            // 
            // controlSettings1
            // 
            this.controlSettings1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controlSettings1.BackColor = System.Drawing.Color.Black;
            this.controlSettings1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlSettings1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlSettings1.ForeColor = System.Drawing.Color.LightGray;
            this.controlSettings1.Location = new System.Drawing.Point(0, 0);
            this.controlSettings1.MinimumSize = new System.Drawing.Size(747, 312);
            this.controlSettings1.Name = "controlSettings1";
            this.controlSettings1.Size = new System.Drawing.Size(765, 532);
            this.controlSettings1.TabIndex = 0;
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(765, 532);
            this.Controls.Add(this.controlSettings1);
            this.ForeColor = System.Drawing.Color.Gainsboro;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSettings";
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSettings_FormClosed);
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ControlSettings controlSettings1;


    }
}