namespace LX29_ChatClient.Forms
{
    partial class FormChatSettings
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
            this.txtB_Username = new System.Windows.Forms.TextBox();
            this.txtB_Message = new System.Windows.Forms.TextBox();
            this.txtB_Action = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lV_Main = new System.Windows.Forms.ListView();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_remove = new System.Windows.Forms.Button();
            this.cB_Enabled = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtB_Username
            // 
            this.txtB_Username.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtB_Username.BackColor = System.Drawing.Color.Black;
            this.txtB_Username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtB_Username.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtB_Username.ForeColor = System.Drawing.Color.White;
            this.txtB_Username.Location = new System.Drawing.Point(12, 28);
            this.txtB_Username.Name = "txtB_Username";
            this.txtB_Username.Size = new System.Drawing.Size(529, 22);
            this.txtB_Username.TabIndex = 0;
            // 
            // txtB_Message
            // 
            this.txtB_Message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtB_Message.BackColor = System.Drawing.Color.Black;
            this.txtB_Message.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtB_Message.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtB_Message.ForeColor = System.Drawing.Color.White;
            this.txtB_Message.Location = new System.Drawing.Point(12, 72);
            this.txtB_Message.Name = "txtB_Message";
            this.txtB_Message.Size = new System.Drawing.Size(529, 22);
            this.txtB_Message.TabIndex = 1;
            // 
            // txtB_Action
            // 
            this.txtB_Action.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtB_Action.BackColor = System.Drawing.Color.Black;
            this.txtB_Action.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtB_Action.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtB_Action.ForeColor = System.Drawing.Color.White;
            this.txtB_Action.Location = new System.Drawing.Point(12, 116);
            this.txtB_Action.Name = "txtB_Action";
            this.txtB_Action.Size = new System.Drawing.Size(529, 22);
            this.txtB_Action.TabIndex = 2;
            this.txtB_Action.TextChanged += new System.EventHandler(this.txtB_Action_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Message";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Action";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(12, 144);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lV_Main
            // 
            this.lV_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lV_Main.BackColor = System.Drawing.Color.Black;
            this.lV_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lV_Main.ForeColor = System.Drawing.Color.White;
            this.lV_Main.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lV_Main.Location = new System.Drawing.Point(12, 173);
            this.lV_Main.MultiSelect = false;
            this.lV_Main.Name = "lV_Main";
            this.lV_Main.Size = new System.Drawing.Size(529, 255);
            this.lV_Main.TabIndex = 8;
            this.lV_Main.UseCompatibleStateImageBehavior = false;
            this.lV_Main.View = System.Windows.Forms.View.Details;
            this.lV_Main.SelectedIndexChanged += new System.EventHandler(this.lV_Main_SelectedIndexChanged);
            this.lV_Main.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lV_Main_MouseUp);
            // 
            // btn_save
            // 
            this.btn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save.Location = new System.Drawing.Point(466, 144);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 23);
            this.btn_save.TabIndex = 9;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_remove
            // 
            this.btn_remove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_remove.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_remove.Location = new System.Drawing.Point(93, 144);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(75, 23);
            this.btn_remove.TabIndex = 10;
            this.btn_remove.Text = "Remove";
            this.btn_remove.UseVisualStyleBackColor = true;
            this.btn_remove.Visible = false;
            this.btn_remove.Click += new System.EventHandler(this.btn_remove_Click);
            // 
            // cB_Enabled
            // 
            this.cB_Enabled.AutoSize = true;
            this.cB_Enabled.Location = new System.Drawing.Point(174, 148);
            this.cB_Enabled.Name = "cB_Enabled";
            this.cB_Enabled.Size = new System.Drawing.Size(65, 17);
            this.cB_Enabled.TabIndex = 11;
            this.cB_Enabled.Text = "Enabled";
            this.cB_Enabled.UseVisualStyleBackColor = true;
            this.cB_Enabled.CheckedChanged += new System.EventHandler(this.cB_Enabled_CheckedChanged);
            // 
            // FormChatSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(553, 440);
            this.Controls.Add(this.cB_Enabled);
            this.Controls.Add(this.btn_remove);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.lV_Main);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtB_Action);
            this.Controls.Add(this.txtB_Message);
            this.Controls.Add(this.txtB_Username);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormChatSettings";
            this.Text = "Chat Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormChatSettings_FormClosing);
            this.Load += new System.EventHandler(this.FormChatSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtB_Username;
        private System.Windows.Forms.TextBox txtB_Message;
        private System.Windows.Forms.TextBox txtB_Action;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView lV_Main;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.CheckBox cB_Enabled;
    }
}