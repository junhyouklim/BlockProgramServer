namespace pc_controller
{
    partial class RegisterForm
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
            this.listbox_able = new System.Windows.Forms.ListBox();
            this.listbox_limit = new System.Windows.Forms.ListBox();
            this.btn_register = new System.Windows.Forms.Button();
            this.picturebox_right = new System.Windows.Forms.PictureBox();
            this.pictureBox_left = new System.Windows.Forms.PictureBox();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.textBox_direct = new System.Windows.Forms.TextBox();
            this.btn_direct = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picturebox_right)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_left)).BeginInit();
            this.SuspendLayout();
            // 
            // listbox_able
            // 
            this.listbox_able.FormattingEnabled = true;
            this.listbox_able.ItemHeight = 12;
            this.listbox_able.Location = new System.Drawing.Point(12, 12);
            this.listbox_able.Name = "listbox_able";
            this.listbox_able.Size = new System.Drawing.Size(193, 304);
            this.listbox_able.TabIndex = 0;
            // 
            // listbox_limit
            // 
            this.listbox_limit.FormattingEnabled = true;
            this.listbox_limit.ItemHeight = 12;
            this.listbox_limit.Location = new System.Drawing.Point(261, 12);
            this.listbox_limit.Name = "listbox_limit";
            this.listbox_limit.Size = new System.Drawing.Size(193, 304);
            this.listbox_limit.TabIndex = 1;
            // 
            // btn_register
            // 
            this.btn_register.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_register.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_register.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn_register.Location = new System.Drawing.Point(281, 331);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(75, 32);
            this.btn_register.TabIndex = 2;
            this.btn_register.Text = "등록완료";
            this.btn_register.UseVisualStyleBackColor = false;
            this.btn_register.Click += new System.EventHandler(this.btn_register_Click);
            // 
            // picturebox_right
            // 
            this.picturebox_right.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.picturebox_right.Image = global::pc_controller.Properties.Resources.오른쪽;
            this.picturebox_right.Location = new System.Drawing.Point(211, 94);
            this.picturebox_right.Name = "picturebox_right";
            this.picturebox_right.Size = new System.Drawing.Size(44, 31);
            this.picturebox_right.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picturebox_right.TabIndex = 4;
            this.picturebox_right.TabStop = false;
            this.picturebox_right.Click += new System.EventHandler(this.picturebox_right_Click);
            // 
            // pictureBox_left
            // 
            this.pictureBox_left.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox_left.Image = global::pc_controller.Properties.Resources.왼쪽;
            this.pictureBox_left.Location = new System.Drawing.Point(211, 207);
            this.pictureBox_left.Name = "pictureBox_left";
            this.pictureBox_left.Size = new System.Drawing.Size(44, 31);
            this.pictureBox_left.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_left.TabIndex = 5;
            this.pictureBox_left.TabStop = false;
            this.pictureBox_left.Click += new System.EventHandler(this.pictureBox_left_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_cancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_cancel.Location = new System.Drawing.Point(362, 331);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 32);
            this.btn_cancel.TabIndex = 3;
            this.btn_cancel.Text = "취소하기";
            this.btn_cancel.UseVisualStyleBackColor = false;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // textBox_direct
            // 
            this.textBox_direct.Location = new System.Drawing.Point(12, 338);
            this.textBox_direct.Name = "textBox_direct";
            this.textBox_direct.Size = new System.Drawing.Size(130, 21);
            this.textBox_direct.TabIndex = 6;
            // 
            // btn_direct
            // 
            this.btn_direct.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_direct.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_direct.Location = new System.Drawing.Point(142, 331);
            this.btn_direct.Name = "btn_direct";
            this.btn_direct.Size = new System.Drawing.Size(73, 32);
            this.btn_direct.TabIndex = 7;
            this.btn_direct.Text = "직접등록";
            this.btn_direct.UseVisualStyleBackColor = false;
            this.btn_direct.Click += new System.EventHandler(this.btn_direct_Click);
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 371);
            this.Controls.Add(this.btn_direct);
            this.Controls.Add(this.textBox_direct);
            this.Controls.Add(this.pictureBox_left);
            this.Controls.Add(this.picturebox_right);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_register);
            this.Controls.Add(this.listbox_limit);
            this.Controls.Add(this.listbox_able);
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register Limit";
            ((System.ComponentModel.ISupportInitialize)(this.picturebox_right)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_left)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listbox_able;
        private System.Windows.Forms.ListBox listbox_limit;
        private System.Windows.Forms.Button btn_register;
        private System.Windows.Forms.PictureBox picturebox_right;
        private System.Windows.Forms.PictureBox pictureBox_left;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.TextBox textBox_direct;
        private System.Windows.Forms.Button btn_direct;
    }
}