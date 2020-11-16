using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pc_controller
{
    public partial class PwInput : Form
    {
        public string wroted_pwd { get; set; }
        public string registered_pwd { get; set; }
        public PwInput()
        {
            InitializeComponent();
            btn_pwd_find.Visible = false;
        }

        public PwInput(string pwd)
        {
            InitializeComponent();
            btn_pwd_find.Visible = true;
            this.registered_pwd = pwd;
        }

        public PwInput(ref string pwd)
        {
            
            InitializeComponent();
            wroted_pwd = pwd;
        }

        private void btn_pwd_Click(object sender, EventArgs e)
        {
            if(textbox_pwd.Text != null)
            {
                wroted_pwd = textbox_pwd.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("빈칸");
            }
        }

        private void btn_pwd_find_Click(object sender, EventArgs e)
        {
            this.textbox_pwd.Text = registered_pwd;
        }
    }
}
