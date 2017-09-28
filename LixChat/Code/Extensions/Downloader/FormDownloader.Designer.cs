namespace LX29_ChatClient
{
    partial class FormDownloader
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.lbl_Info = new System.Windows.Forms.Label();
            this.btn_Abort = new System.Windows.Forms.Button();
            this.btn_Start = new System.Windows.Forms.Button();
            this.lbl_Pre_Info = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(1, 62);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(659, 30);
            this.progressBar1.TabIndex = 0;
            this.progressBar1.Visible = false;
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Name.Location = new System.Drawing.Point(-1, 39);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(51, 20);
            this.lbl_Name.TabIndex = 1;
            this.lbl_Name.Text = "Name";
            this.lbl_Name.Visible = false;
            // 
            // lbl_Info
            // 
            this.lbl_Info.AutoSize = true;
            this.lbl_Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Info.Location = new System.Drawing.Point(-1, 95);
            this.lbl_Info.Name = "lbl_Info";
            this.lbl_Info.Size = new System.Drawing.Size(37, 20);
            this.lbl_Info.TabIndex = 2;
            this.lbl_Info.Text = "Info";
            this.lbl_Info.Visible = false;
            // 
            // btn_Abort
            // 
            this.btn_Abort.AutoSize = true;
            this.btn_Abort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Abort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Abort.Location = new System.Drawing.Point(589, 98);
            this.btn_Abort.Name = "btn_Abort";
            this.btn_Abort.Size = new System.Drawing.Size(60, 32);
            this.btn_Abort.TabIndex = 3;
            this.btn_Abort.Text = "Abort";
            this.btn_Abort.UseVisualStyleBackColor = true;
            this.btn_Abort.Visible = false;
            this.btn_Abort.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_Start
            // 
            this.btn_Start.AutoSize = true;
            this.btn_Start.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Start.Location = new System.Drawing.Point(0, 128);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(661, 32);
            this.btn_Start.TabIndex = 4;
            this.btn_Start.Text = "Start Download";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // lbl_Pre_Info
            // 
            this.lbl_Pre_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_Pre_Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Pre_Info.Location = new System.Drawing.Point(0, 0);
            this.lbl_Pre_Info.Name = "lbl_Pre_Info";
            this.lbl_Pre_Info.Size = new System.Drawing.Size(661, 160);
            this.lbl_Pre_Info.TabIndex = 5;
            this.lbl_Pre_Info.Text = "Pre Info";
            this.lbl_Pre_Info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(661, 160);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.btn_Abort);
            this.Controls.Add(this.lbl_Info);
            this.Controls.Add(this.lbl_Name);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lbl_Pre_Info);
            this.ForeColor = System.Drawing.Color.Gainsboro;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormDownloader";
            this.Text = "Downloader";
            this.Load += new System.EventHandler(this.FormDownloader_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.Label lbl_Info;
        private System.Windows.Forms.Button btn_Abort;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Label lbl_Pre_Info;
    }
}