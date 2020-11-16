namespace pc_controller
{
    partial class PwInput
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
            this.textbox_pwd = new System.Windows.Forms.TextBox();
            this.btn_pwd = new System.Windows.Forms.Button();
            this.btn_pwd_find = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textbox_pwd
            // 
            this.textbox_pwd.Location = new System.Drawing.Point(12, 12);
            this.textbox_pwd.Name = "textbox_pwd";
            this.textbox_pwd.PasswordChar = '*';
            this.textbox_pwd.Size = new System.Drawing.Size(283, 21);
            this.textbox_pwd.TabIndex = 0;
            // 
            // btn_pwd
            // 
            this.btn_pwd.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_pwd.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_pwd.Location = new System.Drawing.Point(301, 6);
            this.btn_pwd.Name = "btn_pwd";
            this.btn_pwd.Size = new System.Drawing.Size(75, 30);
            this.btn_pwd.TabIndex = 1;
            this.btn_pwd.Text = "입력";
            this.btn_pwd.UseVisualStyleBackColor = false;
            this.btn_pwd.Click += new System.EventHandler(this.btn_pwd_Click);
            // 
            // btn_pwd_find
            // 
            this.btn_pwd_find.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_pwd_find.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_pwd_find.Location = new System.Drawing.Point(301, 35);
            this.btn_pwd_find.Name = "btn_pwd_find";
            this.btn_pwd_find.Size = new System.Drawing.Size(75, 30);
            this.btn_pwd_find.TabIndex = 2;
            this.btn_pwd_find.Text = "pwd찾기";
            this.btn_pwd_find.UseVisualStyleBackColor = false;
            this.btn_pwd_find.Click += new System.EventHandler(this.btn_pwd_find_Click);
            // 
            // PwInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 70);
            this.Controls.Add(this.btn_pwd_find);
            this.Controls.Add(this.btn_pwd);
            this.Controls.Add(this.textbox_pwd);
            this.Name = "PwInput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PwInput";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textbox_pwd;
        private System.Windows.Forms.Button btn_pwd;
        private System.Windows.Forms.Button btn_pwd_find;
    }
}