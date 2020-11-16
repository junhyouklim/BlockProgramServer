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
    public partial class RegisterForm : Form
    {

        PwInput pwd_Form;
        List<string> list_limited_program;
        string ip;
        string pwd;
        MainForm_Tcp registerForm_write_tcp;
        public bool IsLimited;

        enum MAIN_SIGNAL { REQUEST_LIMIT = 1, REQUEST_PG_LIST, SEND_PG_LIST, SEND_LPG_LIST, WITHDRAWAL_REQUEST, WITHDRAWAL_PROCESS_COMPLETE, CANCEL_REQUEST };
        public RegisterForm(string client_ip,List<string> list_program,MainForm_Tcp write_tcp)
        {
            InitializeComponent();
            ip = client_ip;
            list_limited_program = new List<string>();
            registerForm_write_tcp = write_tcp;
            IsLimited = false;

            // 실행가능 프로그램리스트를 리스트박스에 추가한다.
            foreach (string program in list_program)
            {
                listbox_able.Items.Add(program);    
            }
        }
        public RegisterForm(string client_ip, List<string> list_program, List<string> list_control,MainForm_Tcp write_tcp)
        {
            InitializeComponent();
            ip = client_ip;
            list_limited_program = new List<string>();
            registerForm_write_tcp = write_tcp;
            IsLimited = false;

            // 실행가능 프로그램리스트를 리스트박스에 추가한다.
            foreach (string program in list_program)
            {
                listbox_able.Items.Add(program);
            }
            foreach(string control in list_control)
            {
                listbox_limit.Items.Add(control);
            }
        }
        private void picturebox_right_Click(object sender, EventArgs e)
        {
            // listbox_able에 선택된 항목이 있는지 확인한다. 
            if(listbox_able.SelectedIndex != -1)
            {
                // 선택된 항목을 listbox_limit으로 옮긴다. 
                listbox_limit.Items.Add(listbox_able.GetItemText(listbox_able.SelectedItem));
                // 선택된 항목을 listbox_able에서 지운다. 
                listbox_able.Items.RemoveAt(listbox_able.SelectedIndex);
            }
        }

        private void pictureBox_left_Click(object sender, EventArgs e)
        {
            if(listbox_limit.SelectedIndex != -1)
            {
                listbox_able.Items.Add(listbox_limit.GetItemText(listbox_limit.SelectedItem));
                listbox_limit.Items.RemoveAt(listbox_limit.SelectedIndex);
            }
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            // 해당 요청에 대한 비밀번호를 설정한다.

            pwd_Form = new PwInput();
            pwd_Form.Text = ip + "의 제어비밀번호 설정";
            pwd_Form.ShowDialog();
            pwd = pwd_Form.wroted_pwd;

            Console.WriteLine("패스워드입력창 닫음");
            Console.WriteLine($"입력한 pwd : {pwd}");

            // 제어리스트에 등록된 프로그램을 담는다. 
            for(int i =0; i<listbox_limit.Items.Count; i++)
            {
                list_limited_program.Add(listbox_limit.GetItemText(listbox_limit.Items[i]));
            }

            // 제어리스트전달 시그널을 보낸다. 
            registerForm_write_tcp.Write_int_to_Server((int)MAIN_SIGNAL.SEND_LPG_LIST);
            Delay(30);
            // 입력한 비밀번호를 전달한다. 
            registerForm_write_tcp.Write_String_to_Server(pwd);
            pwd = null;

            Delay(50);

            // 제어리스트 길이를 보낸다. 
            Console.WriteLine($"listbox_limit.Items.Count : {listbox_limit.Items.Count}");
            registerForm_write_tcp.Write_int_to_Server(listbox_limit.Items.Count);
                
            // 제어리스트를 전달한다.
            foreach(string limited_pg in list_limited_program)
            {
                registerForm_write_tcp.Write_String_to_Server(limited_pg);
                Delay(50);
            }
            IsLimited = true;
            // 창을 닫는다. 
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("등록한 프로그램정보가 사라집니다 취소하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("아니요 클릭");
            }
            
        }

        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

        private void btn_direct_Click(object sender, EventArgs e)
        {
            bool overlap = false;
            string wroted_program = textBox_direct.Text;
            if (wroted_program != null)
            {
                if(wroted_program.Contains(".exe") || wroted_program.Contains(".EXE"))
                {
                    //  등록되어 있지 않은 프로그램이면 추가한다. 
                    foreach(var item in listbox_limit.Items)
                    {
                        if(item.ToString() == wroted_program)
                        {
                            MessageBox.Show("이미 등록된 프로그램");
                            overlap = true;
                            break;
                        }
                    }
                    if (overlap != true)
                        listbox_limit.Items.Add(wroted_program);
                }else { MessageBox.Show("입력형식이 맞지 않습니다.[.exe or .EXE]"); }
            }
            else { MessageBox.Show("빈칸은 등록불가"); }
        }
    }
}
