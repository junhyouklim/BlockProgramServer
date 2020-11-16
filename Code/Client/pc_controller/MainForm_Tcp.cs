using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

namespace pc_controller
{
    public class MainForm_Tcp
    {
        const string serverIP = "10.10.20.55"; // 서버 IP
        //const string serverIP = "192.36.90.239";
        const int serverPort = 9043; // 서버와 통신할 Port번호
        IPEndPoint serv_addr; // 서버의 정보를 저장할 클래스
        TcpClient clnt; // 서버와 통신할 클라이언트 객체
        NetworkStream stream; // 서버와 통신할 stream 객체
        const int BUF = 256;

        RegisterForm registerForm;

        enum MAIN_SIGNAL { REQUEST_LIMIT = 1, REQUEST_PG_LIST, SEND_PG_LIST, SEND_LPG_LIST, WITHDRAWAL_REQUEST, WITHDRAWAL_PROCESS_COMPLETE, CANCEL_REQUEST , CANCEL_SUCESS};
        enum READ_THREAD { EXIST_ALL_CLIENT = 1,
            JOIN_CLIENT,
            ADD_REQUEST,
            DISTRIBUTE_REQUEST,
            WITHDRAWAL_REQUEST,
            WITHDRAWAL_PROCESS_COMPLETE,
            CANCEL_REQUEST,
            CANCEL_SUCESS,
            SEND_PG_LIST,
            RECV_PG_LIST,
            RECV_LPG_LIST,
            EXIT}
        enum MAINFORM_STATUS { DEFAULT = 1, WAITING_LIMIT, UNDER_CONTROL }

        public MainForm_Tcp()
        { 
           string bindIP = null; // 실습실 
           //const string bindIP = "192.36.90.239"; // 기숙사 
           const int bindPort = 0;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine("Address:{0}", host.AddressList[i].ToString());
                    bindIP = host.AddressList[i].ToString();
                    break;
                }
            }
            // clnt_addr 정보를 가지는 클라이언트 생성
            IPEndPoint clnt_addr = new IPEndPoint(IPAddress.Parse(bindIP), bindPort);
            clnt = new TcpClient(clnt_addr);

            RegistryUtill.CreateControlRegistry();
        }

        public void Close_Connect()
        {
            stream.Close();
            clnt.Close();
        }

        public void Write_list_available(List<string> list_available)
        {
            // 실행가능 프로그램리스트 전달시그널
            Write_int_to_Server((int)MAIN_SIGNAL.SEND_PG_LIST);
            Delay(30);
            // 리스트의 길이 전달
            int list_length = list_available.Count;
            Write_int_to_Server(list_length);
            Delay(30);
            // 리스트의 요소 전달
            foreach (string program_name in list_available)
            {
                Console.WriteLine($"Write_list_available : {program_name}");
                Write_String_to_Server(program_name);
                Delay(60);
            }
            //준혁 - 수정
            List<string> controls = RegistryUtill.GetControlAllRegistryValue();
            list_length = controls.Count;
            Console.WriteLine("list_length:{0}", list_length);
            Write_int_to_Server(list_length);
            Delay(30);
            foreach (string program_name in controls)
            {
                Console.WriteLine($"controls : {program_name}");
                Write_String_to_Server(program_name);
                Delay(60);
            }
        }

        // 대리자
        delegate bool Process_Client_Info(ListBox listbox,string ip);
        delegate void Set_ExistClient(ListBox listbox, List<string> list);
        delegate void Effect_Request(object sender, int type);
        
        public void Effect_Withdrawal_Request(object sender,int type)
        {
            if(type == 1)
            {
                Button btn = sender as Button;
                btn.Visible = false;
            }
            else if(type == 2) // available list
            {
                ListBox listBox = sender as ListBox;
                listBox.Visible = true;
            }         
            else if(type == 3) // limited list
            {
                ListBox listBox = sender as ListBox;
                listBox.Items.Clear();
                listBox.Visible = false;
            }
            else if(type == 4) // label
            {
                Label label = sender as Label;
                label.Text = "요청 가능한 클라이언트";
            }
        }




        public void Effect_UnderControl_Request(object sender, int type)
        {
            if(type==1)
            {           
                Button btn = sender as Button;
                btn.Text = "제어해제요청";
                btn.Enabled = true;
            }
            else if(type==2)
            {
                ListBox listbox = sender as ListBox;
                listbox.Visible = true;
                // 레지스트리에 등록된 제한 프로그램을 표시한다. 
                List<string> limited_pg_list = RegistryUtill.GetControlAllRegistryValue();
                if (listbox.Items.Count > 0)
                    listbox.Items.Clear();
                foreach (string program in limited_pg_list)
                    listbox.Items.Add(program.ToString());
            }
            else if(type==3)
            {
                Label label = sender as Label;
                label.Text = "차단중인 프로그램";
            }
        }

        public void Set_ExistClient_Listbox(ListBox available_box, List<string> client_list)
        {
            foreach(string client_ip in client_list)
            {
                Console.WriteLine($"Set_ExistClient_Listbox : {client_ip}");
                available_box.Items.Add(client_ip);
            }
        }

        public bool Set_ClientData_Listbox(ListBox listbox, string client_ip)
        {
            listbox.Items.Add(client_ip);
            return true;
        }

        public bool Remove_ClientData_Listbox(ListBox listBox,string client_ip)
        {
            bool result = false;
            string waiting_client_ip;
            // 리스트박스 전체를 훑는다.
            for(int i=0; i<listBox.Items.Count; i++)
            {
                // client_ip와 같은 ip를 찾으면 취소처리의 의미로 삭제한다. 
                waiting_client_ip = listBox.GetItemText(listBox.Items[i].ToString());
                if (client_ip == waiting_client_ip)
                {
                    listBox.Items.RemoveAt(i);
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void Withdrawal_Request()
        {
            // 요청철회 시그널을 전달한다. 
            Write_int_to_Server((int)MAIN_SIGNAL.WITHDRAWAL_REQUEST);
        }

        public void Cancel_Request()
        {
            // 제어취소요청 시그널을 전달한다.
            Write_int_to_Server((int)MAIN_SIGNAL.CANCEL_REQUEST);
        }

        // 레지스트리의 제한된 모든 프로그램을 해제한다.
        public void Lift_All_Restrictions(string[] program_list)
        {
            foreach(string str in program_list)
            {
                Console.WriteLine($"program : {str}");
            }
            RegistryUtill.DeleteControlAllRegistryValue(program_list);
        }

        delegate void Renew_Listbox(ListBox listBox, ListBox listBox2);
        delegate void Renew_Listbox2(ListBox listBox1,ListBox listBox2, string str);

        //준혁
        private bool FindId_in_Listbox(ListBox listBox, string id)
        {
            bool check = listBox.Items.Contains(id);
            if (check) return true;
            else return false;
        }
        private bool CheckedId_in_Listbox(ListBox listBox, string id)
        {
            if (listBox.InvokeRequired)
            {
                Process_Client_Info dele = FindId_in_Listbox;
                bool Result = (bool)listBox.Invoke(dele, listBox, id);
                return Result;
            }
            else { }
            return false;
        }
        private void RemoveIncludedProcesses_in_ProcessList(ref List<string> totallist, ref List<string> controllist)
        {
            for(int i=0;i<totallist.Count;i++)
            {
                for(int j=0;j<controllist.Count;j++)
                {
                    if(totallist[i].Equals(controllist[j]))
                    {
                        Console.WriteLine("process:{0}, control:{1}", totallist[i], controllist[j]);
                        totallist.Remove(totallist[i]);
                    }
                }
            }
        }
        private void RefreshRegistry()
        {
            string[] controllist = RegistryUtill.GetControlRegistryList();
            if(controllist.Length != 0)
            {
                RegistryUtill.DeleteControlAllRegistryValue(controllist);
            }
        }
        //////////////
        public void Renew_Waiting_Listbox(ListBox origin_listbox,ListBox current_listbox)
        {
            string origin_ip = origin_listbox.GetItemText(origin_listbox.Items[origin_listbox.SelectedIndex]);
            // 요청대기중인 클라이언트 ip를 처리완료리스트박스로 옮긴다. 
            origin_listbox.Items.RemoveAt(origin_listbox.SelectedIndex);
            current_listbox.Items.Add(origin_ip);
        }

        public void Renew_Controlled_Listbox(ListBox available_listbox,ListBox controlled_listbox, string ip)
        {
            // controlled_box의 제어중인 클라ip삭제시키기
            for(int i=0; i<controlled_listbox.Items.Count; i++)
            {
                if(controlled_listbox.Items[i].ToString() == ip)
                {
                    controlled_listbox.Items.RemoveAt(i);
                }
            }
            // available_box에 클라ip추가
            //available_listbox.Items.Add(ip);
        }

        public void Processing_Existing_All_Client(int sig_result,ListBox available_box)
        {
            List<string> list_exist_client = new List<string>();
            // 이미 접속해있던 클라이언트 ip를 읽어 리스트박스에 표시한다.
            // 접속해있는 인원수를 읽는다. 
            sig_result = Read_Int_From_Serv();

            Console.WriteLine($"접속인원수 : {sig_result}");

            // 접속되어 있는 모든 유저의 ip를 읽는다. 
            for (int i = 0; i < sig_result; i++)
            {
                Console.WriteLine($"i : {i}");
                list_exist_client.Add(Read_String_From_Serv());
            }
            // 접속되어 있는 유저들의 ip데이터를 리스트박스에 추가한다.                
            if (available_box.InvokeRequired)
            {
                Set_ExistClient del = Set_ExistClient_Listbox;
                available_box.Invoke(del, available_box, list_exist_client);
            }
            else
            {
            }
        }

        public void Processing_JoinedClient(ListBox available_box)
        {
            // 접속한 클라이언트 IP를 읽는다. 
            string connect_client_ip = Read_String_From_Serv();
            Console.WriteLine($"접속한 클라 : {connect_client_ip}");
            // 클라이언트 IP데이터를 리스트박스에 추가한다. 
            if (available_box.InvokeRequired)
            {
                Process_Client_Info dele = Set_ClientData_Listbox;
                available_box.Invoke(dele, available_box, connect_client_ip);
            }
        }

        public void Processing_AddRequest(ListBox waiting_box)
        {
            // 요청한 클라이언트 아이디를 읽다. 
            string requested_client = Read_String_From_Serv();
            Console.WriteLine($"요청한 클라 : {requested_client}");
            // 요청한 클라이언트 데이터를 요청대기리스트에 추가한다. - Invoke()
            if (waiting_box.InvokeRequired)
            {
                Process_Client_Info dele = Set_ClientData_Listbox;
                waiting_box.Invoke(dele, waiting_box, requested_client);
            }
            else
            {
                Console.WriteLine($"클라이언트 요청대기목록 추가실패");
            }
        }

        public void Processing_Withdrawal_Request(MainForm_Tcp write_tcp,ListBox waiting_box)
        {
            bool invokeResult = false;
            // 요청대기목록에 있는 클라이언트가 요청철회시그널 전달함
            // 요청철회한 클라이언트 ip를 읽는다. 
            string withdrawal_client = Read_String_From_Serv();
            Console.WriteLine($"요청철회한 클라 : {withdrawal_client}");

            // 요청대기목록에서 요청철회한 클라이언트 ip를 지운다. 
            if (waiting_box.InvokeRequired)
            {
                Process_Client_Info dele = Remove_ClientData_Listbox;
                invokeResult = (bool)waiting_box.Invoke(dele, waiting_box, withdrawal_client);
            }
            else { }
            // 요청철회한 클라이언트에게 확인시그널을 전달한다. + 철회된 클라이언트ip를 전달한다.  
            if (invokeResult == true)
            {
                write_tcp.Write_int_to_Server((int)MAIN_SIGNAL.WITHDRAWAL_PROCESS_COMPLETE);
                write_tcp.Write_String_to_Server(withdrawal_client);
            }
            else { Console.WriteLine("철회실패"); }
        }

        public void Processing_Withdrawal_Complete(Label limited_label,ListBox limited_box,ListBox available_box,Button request_btn,ref int status)
        {
            // 요청철회버튼을 사라지게 한다, 요청대기목록 리스트박스를 보여지게 한다.
            if (available_box.InvokeRequired &&
                request_btn.InvokeRequired &&
                limited_label.InvokeRequired &&
                limited_box.InvokeRequired)
            {
                Effect_Request dele = Effect_Withdrawal_Request;
                request_btn.Invoke(dele, request_btn, 1);
                available_box.Invoke(dele, available_box, 2);
                limited_box.Invoke(dele, limited_box, 3);
                limited_label.Invoke(dele, limited_label, 4);
                status = (int)MAINFORM_STATUS.DEFAULT;
                MessageBox.Show("요청철회 성공!");
            }
            else
            { Console.WriteLine($"요청철회 위젯처리실패"); }
        }

        public void Processing_Cancel_Request(MainForm_Tcp write_tcp,ListBox available_box,ListBox controlled_box)
        {
            // 취소요청 한 클라이언트 ip를 읽는다. 
            string ip_cancel_requested = Read_String_From_Serv();
            Console.WriteLine($"취소요청한 ip : {ip_cancel_requested}");
            string registered_pwd = Read_String_From_Serv();
            // 비밀번호를 입력한다.
            while (true)
            {
                // 비밀번호찾기 시그널 전달할 tcp객체 , 취소요청한 클라ip전달
                PwInput input = new PwInput(registered_pwd);
                input.Text = ip_cancel_requested + "의 취소요청";
                input.ShowDialog();

                // IP,비번순으로 전달한다. 
                string pwd = input.wroted_pwd;
                Console.WriteLine($"입력한 비번 : {pwd}");

                // 시그널을 전달한다.
                write_tcp.Write_int_to_Server((int)MAIN_SIGNAL.CANCEL_SUCESS);
                Delay(50);
                // ip,pwd를 전달한다.
                write_tcp.Write_String_to_Server(ip_cancel_requested);
                Delay(50);
                write_tcp.Write_String_to_Server(pwd);
                // pwd일치여부 결과를 읽는다. 
                int pwd_result = Read_Int_From_Serv();
                Console.WriteLine($"pwd_result : {pwd_result}");
                if (pwd_result == 1) // 일치
                {
                    AutoClosingMessageBox.Show("비밀번호 일치, 제한해제완료", "알림", 500);
                    // 취소처리된 ip_cancel_requested 를 list_controled에서 삭제한다. 
                    if (controlled_box.InvokeRequired)
                    {
                        Renew_Listbox2 dele = Renew_Controlled_Listbox;
                        controlled_box.Invoke(dele,available_box ,controlled_box, ip_cancel_requested);
                        /////////////////////////


                    }
                    else { }



                    break;
                }
                else if (pwd_result == 2)
                {
                    MessageBox.Show("비밀번호 불일치, 재입력");
                }
            }
        }

        public void Processing_Recv_LPG_List(Label label_limited,ListBox list_limited,Button btn_request,ref int status)
        {
            Console.WriteLine("Processing_Recv_LPG_List In");
            RefreshRegistry(); //준혁
            int list_length = Read_Int_From_Serv();
            Console.WriteLine("list_length:{0}", list_length);
            string program_name;
            // 제어프로그램리스트를 전달받는다. 
            // 리스트내용을 레지스트리에 적용시킨다. 
            for (int i = 0; i < list_length; i++)
            {
                program_name = Read_String_From_Serv();
                RegistryUtill.SetControlRegistryValue(program_name);
            }
            // status를 "차단중" (3)으로 변경한다.
            status = (int)MAINFORM_STATUS.UNDER_CONTROL;
            // 요청철회 버튼을 요청해제 버튼으로 변경한다.
            if (btn_request.InvokeRequired &&
                label_limited.InvokeRequired &&
                list_limited.InvokeRequired)
            {
                Effect_Request dele = Effect_UnderControl_Request;
                btn_request.Invoke(dele, btn_request, 1);
                list_limited.Invoke(dele, list_limited, 2);
                label_limited.Invoke(dele, label_limited, 3);
                AutoClosingMessageBox.Show("프로그램 등록완료!", "알림", 500);
            }
            else { }
        }

        public void Processing_Cancel_Sucess(Label limited_label,ListBox available_box, ListBox limited_box,Button request_btn, ref int status)
        {
            string[] program_list = RegistryUtill.GetControlRegistryList();
            // 레지스트리에 제한된 프로그램 전체를 제한해제한다. 
            Console.WriteLine($"limit_pg_list : {program_list.Length}");
            Lift_All_Restrictions(program_list);
            // 버튼이 사라진다,차단프로그램 listbox가 사라진다. 
            // 요청가능 클라이언트 listbox를 보여지게 한다. label 글을 수정한다. 
            if (available_box.InvokeRequired &&
                request_btn.InvokeRequired &&
                limited_label.InvokeRequired &&
                limited_box.InvokeRequired )
            {
                Effect_Request dele = Effect_Withdrawal_Request;
                limited_label.Invoke(dele, limited_label, 4);
                available_box.Invoke(dele, available_box, 2);
                request_btn.Invoke(dele, request_btn, 1);
                limited_box.Invoke(dele, limited_box, 3);
                
            }
            status = (int)MAINFORM_STATUS.DEFAULT;
            AutoClosingMessageBox.Show("프로그램 제한해제완료", "알림", 500);
        }

        public void Processing_Recv_PgList(ListBox waiting_box,ListBox controlled_box,MainForm_Tcp write_tcp)
        {
            // 나에게 제어요청한 클라이언트의 실행가능프로그램리스트를 받는 시그널
            List<string> list_executable = new List<string>();
            //준혁
            List<string> list_control = new List<string>();
            // 제어요청한 클라ip
            string client_ip = Read_String_From_Serv();
            Console.WriteLine($"client_ip : {client_ip}");
            // 리스트길이를 읽는다. 
            int list_len = Read_Int_From_Serv();
            Console.WriteLine($"프로그램개수 : {list_len}");

            // 길이만큼 리스트에 프로그램명을 추가한다. 
            for (int i = 0; i < list_len; i++)
            {
                list_executable.Add(Read_String_From_Serv());
                Console.WriteLine($"추가된 프로그램 :{list_executable[i].ToString()}");
            }
            //준혁
            // 리스트길이를 읽는다. 
            list_len = Read_Int_From_Serv();
            Console.WriteLine($"Control 프로그램개수 : {list_len}");
            if (list_len != 0)
            {
                // 길이만큼 리스트에 프로그램명을 추가한다. 
                for (int i = 0; i < list_len; i++)
                {
                    list_control.Add(Read_String_From_Serv());
                    Console.WriteLine($"Control 프로그램 :{list_control[i]}");
                }
            }
            bool IsFound = CheckedId_in_Listbox(waiting_box, client_ip);
            Console.WriteLine("IsFound:{0}", IsFound);
            if (!IsFound)
            {
                RemoveIncludedProcesses_in_ProcessList(ref list_executable, ref list_control);
                Console.WriteLine($"RegisterForm 창 띄우기");
                // RegisterForm을 띄우고 실행가능프로그램리스트를 넘겨준다
                registerForm = new RegisterForm(client_ip, list_executable, list_control, write_tcp);
                registerForm.ShowDialog();
            }
            else
            {
                Console.WriteLine($"RegisterForm 창 띄우기");
                // RegisterForm을 띄우고 실행가능프로그램리스트를 넘겨준다
                registerForm = new RegisterForm(client_ip, list_executable, write_tcp);
                registerForm.ShowDialog();
            }

            // 제한프로그램 등록시 요청대기중인 ip를 waiting_box에서 삭제하고
            // 요청대기중인 ip를 controlled_Box에 옮긴다.
            if (registerForm.IsLimited == true && IsFound)
            {
                Renew_Listbox dele = Renew_Waiting_Listbox;
                if (waiting_box.InvokeRequired)
                {
                    waiting_box.Invoke(dele, waiting_box, controlled_box);
                }
            }
            Console.WriteLine("RegisterForm 닫힘");
        }

        delegate void Processing_Exit(string str,ListBox listBox);

        public void Processing_Remove_Client(string exit_client,ListBox listBox)
        {
            
            for(int i = 0; i<listBox.Items.Count; i++)
            {
                if (exit_client == listBox.Items[i].ToString())
                    listBox.Items.RemoveAt(i);
            }
        }

        public void Processing_Exit_Client(ListBox list_available,ListBox list_waiting,ListBox list_controlled)
        {
            // 나간 클라이언트 ip를 읽는다. 
            string client_ip_exited = Read_String_From_Serv();
            Console.WriteLine($"나간 클라이언트 : {client_ip_exited}");
            // 메인폼의 3개의 리스트박스에서 나간 클라이언트ip를 삭제한다. 
            if(list_available.InvokeRequired &&
                list_waiting.InvokeRequired &&
                list_controlled.InvokeRequired)
            {
                Processing_Exit dele = Processing_Remove_Client;
                list_available.Invoke(dele, client_ip_exited, list_available);
                list_waiting.Invoke(dele, client_ip_exited, list_waiting);
                list_controlled.Invoke(dele, client_ip_exited, list_controlled);
            }           
        }

        delegate void Check_MainForm (MainForm main, NotifyIcon icon);
        delegate void Add_Listbox(ListBox listBox, string str);

        public void Add_Controlled_box(ListBox listBox,string ip)
        {
            listBox.Items.Add(ip);
        }

        public void Check_MainForm_Status(MainForm mainform,NotifyIcon icon)
        {
            if(mainform.Visible == false && icon.Visible == true)
            {
                mainform.Visible = true;
                icon.Visible = false;
                mainform.Activate();
            }
        }

        public void Processing_DistributeRequest(ListBox controlled_listbox)
        {
            string client_ip = Read_String_From_Serv();
            if(controlled_listbox.InvokeRequired)
            {
                Add_Listbox dele = Add_Controlled_box;
                controlled_listbox.Invoke(dele, controlled_listbox, client_ip);
            }
        }

        public void Get_Info_From_Server(
            MainForm main,
            Label label,
            ListBox limited_box,
            ListBox available_box,
            ListBox waiting_box,
            ListBox controlled_box,
            Button btn,
            NotifyIcon icon,
            MainForm_Tcp write_tcp,
            List<string> list_available,
            ref List<string> list_limits,
            ref int status)
        {
            
            // 시그널을 전달받음 
            int result = Read_Int_From_Serv();
            
            switch (result)
            {
                case (int)READ_THREAD.EXIST_ALL_CLIENT:
                    // 이미 접속되어 있는 클라받기 시그널
                    Console.WriteLine($"Read Thread - EXIST_ALL_CLIENT");
                    Processing_Existing_All_Client(result, available_box);
                    break;
                case (int)READ_THREAD.JOIN_CLIENT:
                    // 새로운 클라이언트 접속시 받는 시그널
                    Console.WriteLine($"Read Thread - JOIN_CLIENT");
                    Processing_JoinedClient(available_box);
                    break;
                case (int)READ_THREAD.ADD_REQUEST:
                    // 요청시그널
                    Console.WriteLine($"Read Thread - ADD_REQUEST");
                    Processing_AddRequest(waiting_box);
                    break;
                case (int)READ_THREAD.DISTRIBUTE_REQUEST:
                    // 제어중인 클라이언트 읽기 시그널
                    Processing_DistributeRequest(controlled_box);
                    break;
                case (int)READ_THREAD.WITHDRAWAL_REQUEST:
                    // 요청철회 시그널
                    Console.WriteLine($"Read Thread - WITHDRAWAL_REQUEST");
                    Processing_Withdrawal_Request(write_tcp, waiting_box);
                    break;               
                case (int)READ_THREAD.WITHDRAWAL_PROCESS_COMPLETE:
                    // 요청철회처리완료 시그널을 전달받는다.
                    Console.WriteLine($"Read Thread - WITHDRAWAL_PROCESS_COMPLETE");
                    Processing_Withdrawal_Complete(label,limited_box,available_box, btn, ref status);
                    break;
                // 제어요청 취소 시그널을 전달받는다. 
                case (int)READ_THREAD.CANCEL_REQUEST:
                    // 백그라운드 아이콘상태라면 메인폼을 띄운다. 
                    if(main.InvokeRequired)
                    {
                        Check_MainForm dele = Check_MainForm_Status;
                        main.Invoke(dele, main, icon);
                    }
                    Console.WriteLine($"Read Thread - CANCEL_REQUEST");
                    Processing_Cancel_Request(write_tcp,available_box ,controlled_box);
                    break;
                case (int)READ_THREAD.CANCEL_SUCESS:
                    Console.WriteLine($"Read Thread - CANCEL_SUCESS");
                    Processing_Cancel_Sucess(label,available_box,limited_box ,btn, ref status);
                    break;
                case (int)READ_THREAD.SEND_PG_LIST:
                    Console.WriteLine($"Read Thread - SEND_PG_LIST");
                    // 실행가능 프로그램리스트를 서버에 전달한다.
                    write_tcp.Write_list_available(list_available);
                    Console.WriteLine($"SEND_PG_LIST 완료");
                    break;
                case (int)READ_THREAD.RECV_PG_LIST:
                    Console.WriteLine($"Read Thread - RECV_PG_LIST");
                    Processing_Recv_PgList(waiting_box, controlled_box, write_tcp);
                    break;
                case (int)READ_THREAD.RECV_LPG_LIST:
                    Console.WriteLine($"Read Thread - RECV_LPG_LIST");
                    Processing_Recv_LPG_List(label, limited_box,btn, ref status);
                    if (MessageBox.Show("프로그램 실행차단을 위해 다시시작?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {                       
                        write_tcp.Close_Connect();
                        this.Close_Connect();
                        // 프로그램을 다시시작하다. 
                        ProcessStartInfo startinfo = new ProcessStartInfo("shutdown.exe", "-r");
                        Process.Start(startinfo);
                        Process.GetCurrentProcess().Kill();
                    }
                    else { }
                    break;
                case (int)READ_THREAD.EXIT:
                    Console.WriteLine($"Read Thread - EXIT");
                    Processing_Exit_Client(available_box, waiting_box, controlled_box);
                    break;
                default:
                    break;
            }
        }

        public void Connect_to_Server(int type)
        {
            // serv_addr 정보를 가지는 서버 생성
            serv_addr = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            // 클라이언트가 서버에 접속시도
            
            clnt.Connect(serv_addr);
            // 서버에 접속된 클라이언트는 서버와의 통신할 통로생성
            stream = clnt.GetStream();
            // 타입전달
            Write_int_to_Server(type);
        }

        public void Request_Exe_ProgramList(string request_client_id)
        {
            // 실행가능프로그램리스트 요청 시그널을 전달한다.
            Write_int_to_Server((int)MAIN_SIGNAL.REQUEST_PG_LIST);
            // 프로그램실행리스트를 요청할 클라ip를 전달한다.
            Write_String_to_Server(request_client_id);
        }

        public void Request_limit_program(string id_to_request)
        {
            // 프로그램제한요청 시그널을 전달한다.
            Write_int_to_Server((int)MAIN_SIGNAL.REQUEST_LIMIT);
            // 프로그램제한요청할 클라이언트를 지정하여 전달한다.
            Write_String_to_Server(id_to_request);        
        }

        public void Write_int_to_Server(int signal)
        {
            byte[] data = BitConverter.GetBytes(signal);
            stream.Write(data, 0, data.Length);
        }

        public void Write_String_to_Server(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            stream.Write(data, 0, data.Length);
        }

        public int Read_Int_From_Serv()
        {
            byte[] data = new byte[sizeof(int)];
            stream.Read(data, 0, data.Length);
            int result = BitConverter.ToInt32(data, 0);
            return result;
        }

        public string Read_String_From_Serv()
        {
            byte[] data = new byte[50];
            int bytes = stream.Read(data, 0, data.Length);
            string str = Encoding.UTF8.GetString(data, 0, bytes);
            return str;
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

    }
}
