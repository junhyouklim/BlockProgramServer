using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockProgramServer
{
    public partial class MainForm : Form
    {
        private TcpIp server;
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnServOpen_Click(object sender, EventArgs e)
        {
            //서버 오픈 버튼
            server = new TcpIp("127.0.0.1", 9003);
            server.ServerOn();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("123");
        }
    }
}
