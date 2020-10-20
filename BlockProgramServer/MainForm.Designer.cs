namespace BlockProgramServer
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnServOpen = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnServOpen
            // 
            this.btnServOpen.Location = new System.Drawing.Point(206, 92);
            this.btnServOpen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnServOpen.Name = "btnServOpen";
            this.btnServOpen.Size = new System.Drawing.Size(86, 29);
            this.btnServOpen.TabIndex = 0;
            this.btnServOpen.Text = "Open";
            this.btnServOpen.UseVisualStyleBackColor = true;
            this.btnServOpen.Click += new System.EventHandler(this.btnServOpen_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(539, 128);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 562);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnServOpen);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnServOpen;
        private System.Windows.Forms.Button button1;
    }
}

