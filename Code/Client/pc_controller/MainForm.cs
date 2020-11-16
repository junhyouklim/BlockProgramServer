using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Win32;

namespace pc_controller
{
    public partial class MainForm : Form
    {
        private String PROGRAM_KEY_NAME = "PC_Blocker"; // 프로그램 키 이름
        private String ASSEMBLY_PATH = Assembly.GetExecutingAssembly().Location; // 어쎔블리경로

        MainForm_Tcp write_mainForm_Tcp,read_mainForm_Tcp; // 서버에 전달용 객체
        string id; // ip
        int status; // 1 : 기본값 (요청전) 2 : 요청중 , 3 : 제어중

        List<string> programs_available;
        List<string> programs_limits;

        enum TYPE { MAIN_WRITE = 1, MAIN_READ }
        enum MAINFORM_STATUS { DEFAULT = 1 , WAITING_LIMIT , UNDER_CONTROL}

        public MainForm()
        {
            if (IsAdministrator() == false)
            {
                try
                {
                    ProcessStartInfo procInfo = new ProcessStartInfo();
                    procInfo.UseShellExecute = true;
                    procInfo.FileName = Application.ExecutablePath;
                    procInfo.WorkingDirectory = Environment.CurrentDirectory;
                    procInfo.Verb = "runas";
                    Process.Start(procInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Application.Exit();
            }
            InitializeComponent();
            programs_available = new List<string>();
            programs_limits = new List<string>();

            btn_request.Visible = false;

            Use_Notify();

            // Get the IP  
            string hostName = Dns.GetHostName(); 
            id = Dns.GetHostByName(hostName).AddressList[0].ToString();
            status = (int)MAINFORM_STATUS.DEFAULT;

            // 서버 쓰기용
            write_mainForm_Tcp = new MainForm_Tcp();
            write_mainForm_Tcp.Connect_to_Server((int)TYPE.MAIN_WRITE);
            // 연결한 클라이언트 id 서버에 전달
            write_mainForm_Tcp.Write_String_to_Server(id);

            // 스레드 읽기용
            read_mainForm_Tcp = new MainForm_Tcp();
            
            read_mainForm_Tcp.Connect_to_Server((int)TYPE.MAIN_READ);
            // 연결한 클라이언트 id 서버에 전달
            read_mainForm_Tcp.Write_String_to_Server(id);

            // 실행가능한 프로그램 목록만들기
            RegistryUtill.Create_Executable_List(programs_available);

            // 읽기 스레드 생성 및 실행
            Thread read_th = new Thread(MainForm_Read_Thread); // 커서 Read 쓰레드 생성 
            read_th.Start();

            if (!RegistryUtill.IsAutoStartEnabled(PROGRAM_KEY_NAME, ASSEMBLY_PATH)) // 자동실행 설정 여부
                RegistryUtill.SetAutoStart(PROGRAM_KEY_NAME, ASSEMBLY_PATH); // 자동 실행

        }
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            if (null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                Console.WriteLine("권한: {0}", principal.IsInRole(WindowsBuiltInRole.Administrator));
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            return false;
        }
        private void Use_Notify()
        {
            icon_hidden.Visible = false;
            icon_hidden.BalloonTipText = "Program Blocker를 사용해 주셔서 감사합니다.";
            icon_hidden.BalloonTipTitle = "Program Blocker";
            icon_hidden.ShowBalloonTip(1);
        }
   
        // 서버로부터 시그널 + 리스트전달 + 리스트받기 등을 수행하는 스레드
        private void MainForm_Read_Thread()
        {
            while(true)
            {
                read_mainForm_Tcp.Get_Info_From_Server(this,
                    label_status,
                    list_pg_limited,
                    list_available,
                    list_waiting,
                    list_controled,
                    btn_request,
                    icon_hidden, 
                    write_mainForm_Tcp,
                    programs_available,
                    ref programs_limits,
                    ref status);
                Thread.Sleep(1000);
            }
        }


        private void btn_request_Click(object sender, EventArgs e)
        {
            if(status == (int)MAINFORM_STATUS.WAITING_LIMIT)
            {
                write_mainForm_Tcp.Withdrawal_Request();
                btn_request.Text = "요청철회 처리중...";
                btn_request.Enabled = false;
            }
            else if(status == (int)MAINFORM_STATUS.UNDER_CONTROL)
            {
                write_mainForm_Tcp.Cancel_Request();
                btn_request.Text = "취소요청 처리중...";
                btn_request.Enabled = false;
            }
            else
            {
                MessageBox.Show("처리대기중...");
            }
        }

        private void list_waiting_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_index = -1;

            // 더블클릭한 마우스 포인터의 위치
            Point point = e.Location;

            // 요청목록리스트의 IndexFromPoint 메서드호출로 인덱스 추출
            selected_index = list_waiting.IndexFromPoint(point);

            Console.WriteLine($"선택된 인덱스 : {selected_index}");

            if(selected_index != -1)
            {
                string id_to_request_list = list_waiting.Items[selected_index].ToString();
                Console.WriteLine($"선택된 ip : {id_to_request_list}");
                if(id_to_request_list != null)
                {
                    write_mainForm_Tcp.Request_Exe_ProgramList(id_to_request_list);
                }
            }
        }

        private void list_controled_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_index = -1;

            // 더블클릭한 마우스 포인터의 위치
            Point point = e.Location;

            // 요청목록리스트의 IndexFromPoint 메서드호출로 인덱스 추출
            selected_index = list_controled.IndexFromPoint(point);

            Console.WriteLine($"선택된 인덱스 : {selected_index}");

            if (selected_index != -1)
            {
                string id_to_request_list = list_controled.Items[selected_index].ToString();
                Console.WriteLine($"선택된 ip : {id_to_request_list}");
                if (id_to_request_list != null)
                {
                    write_mainForm_Tcp.Request_Exe_ProgramList(id_to_request_list);
                }
            }
        }

        private void icon_hidden_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            icon_hidden.Visible = false;
            this.Activate();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            icon_hidden.Visible = false;
            this.Activate();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            // 레지스트리에 등록된 제한프로그램 전체를 삭제한다.
            string[] program_list = RegistryUtill.GetControlRegistryList();
            foreach (string str in program_list)
            {
                Console.WriteLine($"program : {str}");
            }
            RegistryUtill.DeleteControlAllRegistryValue(program_list);

            // 아이콘리소스해제 ,백그라운드프로세스를 없앤다
            this.icon_hidden.Dispose();

            read_mainForm_Tcp.Close_Connect();
            write_mainForm_Tcp.Close_Connect();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Diagnostics.Process.GetCurrentProcess().Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("숨겨진 아이콘으로 활성화?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                e.Cancel = true;
                icon_hidden.Visible = true;
                this.Visible = false;
            }
            else
            {
                // 레지스트리에 등록된 제한프로그램 전체를 삭제한다.
                string[] program_list = RegistryUtill.GetControlRegistryList();
                foreach (string str in program_list)
                {
                    Console.WriteLine($"program : {str}");
                }
                RegistryUtill.DeleteControlAllRegistryValue(program_list);
                // 스트림,클라이언트연결종료
                write_mainForm_Tcp.Close_Connect();
                read_mainForm_Tcp.Close_Connect();

                // 아이콘리소스 해제 , 백그라운드프로세스를 지운다.
                this.icon_hidden.Dispose();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                System.Diagnostics.Process.GetCurrentProcess().Close();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] limited_program_list = RegistryUtill.GetControlRegistryList();
            if(limited_program_list.Length > 0)
            {
                // 컨트롤 설정
                status = (int)MAINFORM_STATUS.UNDER_CONTROL;
                label_status.Text = "차단중인 프로그램";
                list_pg_limited.Visible = true;
                btn_request.Visible = true;
                btn_request.Text = "제어해제요청";
                // 차단중인 프로그램 listbox에 출력
                foreach (string program in limited_program_list)
                {
                    list_pg_limited.Items.Add(program);
                }
            }
        }

        private void list_available_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_index = -1;

            // 더블클릭한 마우스 포인터의 위치
            Point point = e.Location;

            // 요청목록리스트의 IndexFromPoint 메서드호출로 인덱스 추출
            selected_index = list_available.IndexFromPoint(point);

            if (selected_index != -1)
            {
                string id_to_request = list_available.Items[selected_index].ToString();
                // Mainform의 status가 요청중이 아니라며 더블클릭한 아이템(클라ip)이 비어있지 않으면
                if (status==(int)MAINFORM_STATUS.DEFAULT && id_to_request != null)
                {
                    Console.WriteLine($"[list_available_MouseDoubleClick]");
                    // 선택된 컴퓨터id에 컴퓨터 제어를 요청하다.
                    write_mainForm_Tcp.Request_limit_program(id_to_request);
                    // Mainform status를 "요청중"으로 초기화한다.
                    status = (int) MAINFORM_STATUS.WAITING_LIMIT;
                    // 요청대기 리스트박스를 사라지게한다. 
                    // list_available.Visible = false;
                    // 요청철회 택스트로 변경 , 버튼활성화
                    btn_request.Text = "요청철회";
                    btn_request.Enabled = true;
                    // 요청취소버튼을 보여지게한다.
                    btn_request.Visible = true;
                }
            }
        }
    }
}
