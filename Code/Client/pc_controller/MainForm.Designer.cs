namespace pc_controller
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.list_available = new System.Windows.Forms.ListBox();
            this.list_waiting = new System.Windows.Forms.ListBox();
            this.label_status = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_request = new System.Windows.Forms.Button();
            this.list_controled = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.icon_hidden = new System.Windows.Forms.NotifyIcon(this.components);
            this.menu_strip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.list_pg_limited = new System.Windows.Forms.ListBox();
            this.menu_strip.SuspendLayout();
            this.SuspendLayout();
            // 
            // list_available
            // 
            this.list_available.FormattingEnabled = true;
            this.list_available.ItemHeight = 12;
            this.list_available.Location = new System.Drawing.Point(12, 49);
            this.list_available.Name = "list_available";
            this.list_available.Size = new System.Drawing.Size(236, 208);
            this.list_available.TabIndex = 0;
            this.list_available.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.list_available_MouseDoubleClick);
            // 
            // list_waiting
            // 
            this.list_waiting.FormattingEnabled = true;
            this.list_waiting.ItemHeight = 12;
            this.list_waiting.Location = new System.Drawing.Point(257, 49);
            this.list_waiting.Name = "list_waiting";
            this.list_waiting.Size = new System.Drawing.Size(236, 136);
            this.list_waiting.TabIndex = 1;
            this.list_waiting.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.list_waiting_MouseDoubleClick);
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_status.Location = new System.Drawing.Point(37, 21);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(190, 16);
            this.label_status.TabIndex = 2;
            this.label_status.Text = "요청 가능한 클라이언트";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(273, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "요청 대기중인 클라이언트";
            // 
            // btn_request
            // 
            this.btn_request.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_request.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_request.Location = new System.Drawing.Point(12, 257);
            this.btn_request.Name = "btn_request";
            this.btn_request.Size = new System.Drawing.Size(239, 106);
            this.btn_request.TabIndex = 4;
            this.btn_request.Text = "요청철회";
            this.btn_request.UseVisualStyleBackColor = false;
            this.btn_request.Click += new System.EventHandler(this.btn_request_Click);
            // 
            // list_controled
            // 
            this.list_controled.FormattingEnabled = true;
            this.list_controled.ItemHeight = 12;
            this.list_controled.Location = new System.Drawing.Point(257, 215);
            this.list_controled.Name = "list_controled";
            this.list_controled.Size = new System.Drawing.Size(236, 148);
            this.list_controled.TabIndex = 5;
            this.list_controled.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.list_controled_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(296, 193);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "제어중인 클라이언트";
            // 
            // icon_hidden
            // 
            this.icon_hidden.ContextMenuStrip = this.menu_strip;
            this.icon_hidden.Icon = ((System.Drawing.Icon)(resources.GetObject("icon_hidden.Icon")));
            this.icon_hidden.Text = "Blocker";
            this.icon_hidden.Visible = true;
            this.icon_hidden.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.icon_hidden_MouseDoubleClick);
            // 
            // menu_strip
            // 
            this.menu_strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.menu_strip.Name = "menu_strip";
            this.menu_strip.Size = new System.Drawing.Size(102, 48);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.openToolStripMenuItem.Text = "open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.quitToolStripMenuItem.Text = "quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // list_pg_limited
            // 
            this.list_pg_limited.FormattingEnabled = true;
            this.list_pg_limited.ItemHeight = 12;
            this.list_pg_limited.Location = new System.Drawing.Point(12, 49);
            this.list_pg_limited.Name = "list_pg_limited";
            this.list_pg_limited.Size = new System.Drawing.Size(239, 208);
            this.list_pg_limited.TabIndex = 8;
            this.list_pg_limited.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 380);
            this.Controls.Add(this.list_pg_limited);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.list_controled);
            this.Controls.Add(this.btn_request);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.list_waiting);
            this.Controls.Add(this.list_available);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Program Blocker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menu_strip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox list_available;
        private System.Windows.Forms.ListBox list_waiting;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_request;
        private System.Windows.Forms.ListBox list_controled;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NotifyIcon icon_hidden;
        private System.Windows.Forms.ContextMenuStrip menu_strip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ListBox list_pg_limited;
    }
}

