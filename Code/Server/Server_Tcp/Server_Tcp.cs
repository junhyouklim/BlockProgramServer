using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace Server_Tcp
{
    enum TYPE { MAIN_WRITE = 1, MAIN_READ }
    enum PROCESSSIGNAL { EXIST_ALL_CLIENT = 1, JOIN_CLIENT, ADD_REQUEST, DISTRIBUTE_REQUEST, WITHDRAWAL_REQUEST, WITHDRAWAL_PROCESS_COMPLETE, CANCEL_REQUEST, CANCEL_SUCCESS, SEND_PG_LIST, RECV_PG_LIST, RECV_LPG_LIST, EXIT}
    enum CONTROLSTATUS { DEFAULT = 1, WAITING_LIMIT, UNDER_CONTROL }
    enum MAIN_SIGNAL { REQUEST_LIMIT=1, REQUEST_PG_LIST, SEND_PG_LIST, SEND_LPG_LIST, WITHDRAWAL_REQUEST, WITHDRAWAL_PROCESS_COMPLETE, CANCEL_REQUEST, CANCEL_SUCCESS}
    enum FINDSTREAM { WRITE=1,READ}
    enum CHECK { SUCCESS=1,FAIL}
    class Server_Tcp
    {
        //전역 변수
        public static List<UserInfo> users = new List<UserInfo>();
        //통신 변수
        private TcpListener server = null;
        private Socket clientsock = null;
        private string bindIp = null;
        private NetworkStream basicStream = null;
        private UserInfo user = null;
        
        private const int BUF = 512;

        public Server_Tcp(int bindPort)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                for (int i = 0; i < host.AddressList.Length; i++)
                {
                    if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        Console.WriteLine("Address:{0}", host.AddressList[i].ToString());
                        bindIp = host.AddressList[i].ToString();
                        break;
                    }
                }

                IPEndPoint localAddress = new IPEndPoint(IPAddress.Parse(bindIp), bindPort);
                //IPEndPoint localAddress = new IPEndPoint(IPAddress.Any, bindPort);
                server = new TcpListener(localAddress);
                server.Start();
            }
            catch(SocketException e)
            {
                Console.WriteLine(e);
                server.Stop();
            }
        }
        ~Server_Tcp()
        {
            Console.WriteLine("소멸자 실행");
            foreach(UserInfo user in users)
            {
                user.readSock.Close();
                user.writeSock.Close();
            }
        }
        public void ServerOn()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("ServerOn");
                    clientsock = server.AcceptSocket();
                    basicStream = new NetworkStream(clientsock);
                    int type = RecvType(basicStream);
                    Console.WriteLine("Type:{0}", type);
                    string id = SetUserInfo(type);
                    Console.WriteLine("접속 클라:{0}", id);

                    if (type == (int)TYPE.MAIN_READ)
                    {
                        Console.WriteLine("read In");
                        SendOtherClientList_to_Client(id);
                        ClientHandler clientThread = new ClientHandler(id);

                        /*
                         * Client의 접속이 올때까지 Block되는 부분, 대게 이부분을 Thread로 만들어 보내 버린다.
                         * 백그라운드 Thread에 처리를 맡긴다.
                         */
                        Thread th = new Thread(new ThreadStart(clientThread.ProcessRun));
                        th.IsBackground = true;
                        th.Start();
                        Console.WriteLine("read End");
                    }
                    else
                    {
                        Console.WriteLine("write In");
                        SendId_to_AllClients(id); //접속 클라 Id를 다른 클라이언트들에게 보낸다
                        Console.WriteLine("write End");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ServerOn catch");
                Console.WriteLine(e);
            }
            finally
            {
                clientsock.Close();
                basicStream.Close();
                server.Stop();
            }
        }
        private string SetUserInfo(int type)
        {
            Console.WriteLine("SetUserInfo In");
            string id = null;
            
            switch(type)
            {
                case (int)TYPE.MAIN_WRITE:
                    id = SetReadInfo();
                    break;
                case (int)TYPE.MAIN_READ:
                    id = SetWriteInfo();
                    break;
            }
            Console.WriteLine("SetUserInfo End");
            return id;
        }
        private string SetWriteInfo()
        {
            Console.WriteLine("SetWriteInfo In");
            string id = RecvStr(basicStream);
            foreach(UserInfo user in users)
            {
                if(user.Id.Equals(id))
                {
                    user.writeSock = clientsock;
                    user.writeStream = basicStream;
                }
            }
            Console.WriteLine("SetWriteInfo End");
            return id;
        }
        private string SetReadInfo()
        {
            Console.WriteLine("SetReadInfo In");
            string id = RecvStr(basicStream);
            for(int i=0;i<users.Count;i++)
            {
                if(users[i].Id.Equals(id))
                {
                    Console.WriteLine("기존에 등록되어있는 Id");
                    users[i].readSock = clientsock;
                    users[i].readStream = basicStream;
                    return id;
                }
            }
            user = new UserInfo(basicStream, clientsock, id);
            users.Add(user);
            Console.WriteLine("SetReadInfo End");
            return id;
        }
        private void SendOtherClientList_to_Client(string id)
        {
            Console.WriteLine("SendOtherClientList_to_Client In");
            if (users.Count > 1)
            {
                SendInt(basicStream, (int)PROCESSSIGNAL.EXIST_ALL_CLIENT); //처리 시그널 전송
                Delay(30);
                SendInt(basicStream, users.Count - 1); //List count 전송
                Delay(30);
                foreach (UserInfo user in users)
                {
                    if(!user.Id.Equals(id))
                    {
                        Console.WriteLine("클라이언트 이름 전송");
                        SendStr(basicStream, user.Id);
                        Delay(30);
                    }    
                }
            }
            Console.WriteLine("SendOtherClientList_to_Client End");
        }
        private void SendId_to_AllClients(string id)
        {
            Console.WriteLine("SendId_to_AllClients In");
            
            if (users != null)
            {
                foreach (UserInfo temp in users) //기존에 저장되어 있는 클라이언트들
                {
                    if(!temp.Id.Equals(id))
                    {
                        List<string> Idlist = temp.controlIdList;
                        foreach (string controlid in Idlist)
                        {
                            if(controlid.Equals(id))
                            {
                                SendInt(temp.writeStream, (int)PROCESSSIGNAL.DISTRIBUTE_REQUEST);
                                Delay(20);
                                SendStr(temp.writeStream, id);
                                Delay(30);
                            }
                        }
                        SendInt(temp.writeStream, (int)PROCESSSIGNAL.JOIN_CLIENT);
                        Delay(20);
                        SendStr(temp.writeStream, id);
                        Delay(30);
                    }
                }
            }
            Console.WriteLine("SendId_to_AllClients End");
        }
        private void SendInt(NetworkStream stream,int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            stream.Write(data, 0, sizeof(int));
        }
        private void SendStr(NetworkStream stream, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }
        private string RecvStr(NetworkStream stream)
        {
            byte[] data = new byte[BUF];
            int bytes = stream.Read(data, 0, data.Length);
            string str = Encoding.UTF8.GetString(data, 0, bytes);
            return str;
        }
        private int RecvType(NetworkStream stream)
        {
            byte[] data = new byte[sizeof(int)];
            stream.Read(data, 0, data.Length);
            int type = BitConverter.ToInt32(data, 0);
            return type;
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
    class ClientHandler
    {
        private Socket myreadSock = null;
        private NetworkStream myreadStream = null;
        private Socket mywriteSock = null;
        private NetworkStream mywriteStream = null;
        private string myId = null;
        private const int BUF = 512;
        private string requestId = null; //요청 대상자 ID
        private string controlId = null; //제어 수락 ID
        private Mutex mutex = new Mutex();

        public ClientHandler(Socket sock)

        {
            this.myreadSock = sock;
            myreadStream = new NetworkStream(sock);
        }
        public ClientHandler(string id)
        {
            myId = id;
            Console.WriteLine("ClientHandler Set Id:{0}", id);
            foreach(UserInfo user in Server_Tcp.users)
            {
                if(user.Id.Equals(id))
                {
                    myreadSock = user.readSock; //클라이언트가 write한것
                    myreadStream = user.readStream; //클라이언트가 write한것
                    mywriteSock = user.writeSock; //클라이언트가 read할것
                    mywriteStream = user.writeStream; //클라이언트가 read할것
                }
            }
        }
        public ClientHandler(Socket writesock, NetworkStream writestream)
        {
            this.mywriteSock = writesock;
            this.mywriteStream = writestream;
        }
        public ClientHandler(Socket readsock, NetworkStream readstream, string id)
        {
            this.myreadSock = readsock;
            this.myreadStream = readstream;
            this.myId = id;
        }

        //모든 처리를 하는 쓰레드
        public void ProcessRun()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("스레드 실행");
                    int signal = RecvInt();
                    Console.WriteLine("signal:{0}", signal);

                    switch (signal)
                    {
                        //제어 요청
                        case (int)MAIN_SIGNAL.REQUEST_LIMIT:
                            RequestControl_to_Controller((int)PROCESSSIGNAL.ADD_REQUEST);
                            break;
                        //프로세스 리스트 요청
                        case (int)MAIN_SIGNAL.REQUEST_PG_LIST:
                            AcceptControl_from_Controller((int)PROCESSSIGNAL.SEND_PG_LIST);
                            break;
                        //프로세스 리스트 전송
                        case (int)MAIN_SIGNAL.SEND_PG_LIST:
                            Recv_and_SendProcessList_to_Controller((int)PROCESSSIGNAL.RECV_PG_LIST);
                            break;
                        //제어 프로세스등록 시그널
                        case (int)MAIN_SIGNAL.SEND_LPG_LIST:
                            Recv_and_SendControlList_to_RequestingClient((int)PROCESSSIGNAL.RECV_LPG_LIST);
                            break;
                        //요청 취소
                        case (int)MAIN_SIGNAL.WITHDRAWAL_REQUEST:
                            CancelRequest_to_Controller((int)PROCESSSIGNAL.WITHDRAWAL_REQUEST);
                            break;
                        //요청 취소 확인 시그널 전송
                        case (int)MAIN_SIGNAL.WITHDRAWAL_PROCESS_COMPLETE:
                            SendCancellationConfirmation_to_Client((int)PROCESSSIGNAL.WITHDRAWAL_PROCESS_COMPLETE);
                            break;
                        //전체 해제 신청
                        case (int)MAIN_SIGNAL.CANCEL_REQUEST:
                            RequestFullCancellation_to_Controller((int)PROCESSSIGNAL.CANCEL_REQUEST);
                            break;
                        //전체 해제 승인
                        case (int)MAIN_SIGNAL.CANCEL_SUCCESS:
                            ReleaseApproval_by_Controller((int)PROCESSSIGNAL.CANCEL_SUCCESS);
                            break;
                        default:
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Thread catch");
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("ProcessRun finally 실행");
                myreadSock.Close();
                myreadStream.Close();
                mywriteSock.Close();
                mywriteStream.Close();
                Console.WriteLine("usersCount:{0}", Server_Tcp.users.Count);
                for(int i=0;i<Server_Tcp.users.Count;i++)
                {
                    if (Server_Tcp.users[i].Id.Equals(myId))
                    {
                        Console.WriteLine("Remove In");
                        Server_Tcp.users[i].readSock = null;
                        Server_Tcp.users[i].readStream = null;
                        Server_Tcp.users[i].writeSock = null;
                        Server_Tcp.users[i].writeStream = null;
                    }
                    else
                    {
                        Console.WriteLine("Id 보내기:{0}, 보낸 Ip:{1}",myId, Server_Tcp.users[i].writeStream);
                        SendInt(Server_Tcp.users[i].writeStream, (int)PROCESSSIGNAL.EXIT);
                        Delay(30);
                        SendStr(Server_Tcp.users[i].writeStream, myId);
                    }
                }
            }
            
        }

        //다른 클라이언트에게 요청 보냄
        private void RequestControl_to_Controller(int signal)
        {
            foreach(UserInfo user in Server_Tcp.users)
            {
                Console.WriteLine("userID:{0}", user.Id);
            }
            Console.WriteLine("RequestControl_to_OtherClient In");
            Console.WriteLine("RequestControl_to_OtherClient Signal:{0}", signal);
            mutex.WaitOne();
            requestId = RecvStr(); //클라한테 받은 요청 대상자 ID
            SetRequestId(myId, requestId);
            Console.WriteLine("requestId:{0}", requestId);
            NetworkStream stream = FindNetworkStream(requestId, (int)FINDSTREAM.WRITE);
            //NetworkStream stream = FindWriteNetworkStream(requestId);
            mutex.ReleaseMutex();

            //클라한테 처리시그널 전송
            SendInt(stream, signal);

            //클라한테 ID 전송
            SendStr(stream, myId);

            SetStatus(myId, (int)CONTROLSTATUS.WAITING_LIMIT);
            Console.WriteLine("RequestControl_to_OtherClient End");
        }

        //요청 대상자에게 제어 수락 시그널 받음
        private void AcceptControl_from_Controller(int signal)
        {
            Console.WriteLine("AcceptControl_from_OtherClient In");
            controlId = RecvStr(); //클라한테 받은 요청자 ID
            Console.WriteLine("controlId:{0}", controlId);
            SetControlIdList(myId, controlId); // controlid 셋팅
            Console.WriteLine("id:{0}, myid:{1}", controlId, myId);
            
            NetworkStream stream = FindWriteNetworkStream(controlId);

            //클라한테 처리시그널 전송
            SendInt(stream, signal);

            SetStatus(controlId, (int)CONTROLSTATUS.UNDER_CONTROL);
            Console.WriteLine("AcceptControl_from_OtherClient End");
        }

        //요청 보낸 클라이언트에게 요청 취소 보냄
        private void CancelRequest_to_Controller(int signal)
        {
            Console.WriteLine("CancelRequest_to_OtherClient In");
            string requestid = FindRequestId(myId);
            Console.WriteLine("CancelRequest_to_OtherClient requestId:{0}", requestid);
            NetworkStream stream = FindNetworkStream(requestid, (int)FINDSTREAM.WRITE);
            //NetworkStream stream = FindWriteNetworkStream(requestid);

            //클라한테 처리시그널 전송
            SendInt(stream, signal);
            SendStr(stream, myId);
            Console.WriteLine("CancelRequest_to_OtherClient End");
        }

        //프로세스 리스트 전송
        private void Recv_and_SendProcessList_to_Controller(int signal)
        {
            Console.WriteLine("Recv_and_SendProcessList_to_Client In");
            List<string> processList = new List<string>();
            List<string> controlList = new List<string>();

            int Count = RecvInt(); //total list Count 받음
            Console.WriteLine("total count:{0}", Count);
            for (int i = 0; i < Count; i++)
            {
                string str = RecvStr();
                Console.WriteLine("for문:{0}, str :{1}", i,str);
                processList.Add(str);
            }
            Count = RecvInt(); //control list Count 받음
            Console.WriteLine("control count:{0}", Count);
            if(Count != 0)
            {
                for (int i = 0; i < Count; i++)
                {
                    string str = RecvStr();
                    Console.WriteLine("for문:{0}, str :{1}", i, str);
                    controlList.Add(str);
                }
            }
            SendList_to_Controller(ref processList, ref controlList ,signal);
            Console.WriteLine("Recv_and_SendProcessList_to_Client End");
        }

        private void SendList_to_Controller(ref List<string> list, ref List<string> controllist, int signal)
        {
            Console.WriteLine("SendList In");
            string requestid = FindRequestId(myId);
            Console.WriteLine("SendList requestId:{0}",requestid);
            NetworkStream stream = FindNetworkStream(requestid, (int)FINDSTREAM.WRITE);
            //NetworkStream stream = FindWriteNetworkStream(requestid);
            
            SendInt(stream, signal); // 처리 시그널 전송
            Delay(30);
            SendStr(stream, myId); // 리스트 준 id 전송
            Delay(50);
            Console.WriteLine("Id:{0}", myId);
            SendList(stream, list); //totallist 전송
            SendList(stream, controllist); //controllist 전송

            Console.WriteLine("SendList End");
        }
        
        //취소 확인 시그널 요청취소자한테 전송
        private void SendCancellationConfirmation_to_Client(int signal)
        {
            Console.WriteLine("SendCancellationConfirmation_to_Client In");
            string id = RecvStr();
            NetworkStream stream = FindNetworkStream(id, (int)FINDSTREAM.WRITE);

            SendInt(stream, signal); //처리 시그널 전송

            SetStatus(id, (int)CONTROLSTATUS.DEFAULT);
            Console.WriteLine("SendCancellationConfirmation_to_Client End");
        }

        //제어자가 요청자한테 제어리스트 전송
        private void Recv_and_SendControlList_to_RequestingClient(int signal)
        {
            Console.WriteLine("Recv_and_SendControlList_to_RequestingClient In");
            string Password = RecvStr(); //password 받음
            Console.WriteLine("Password:{0}, myId:{1}, controlId:{2}", Password, myId, controlId);
            mutex.WaitOne();
            SetControlInfo(myId, controlId, Password);
            SetControlPasswordList(myId, Password);
            mutex.ReleaseMutex();

            int Count = RecvInt(); //list Count 받음
            Console.WriteLine("count:{0}", Count);

            List<string> controlList = new List<string>();
            if(Count!=0)
            {
                for (int i = 0; i < Count; i++)
                {
                    string str = RecvStr();
                    Console.WriteLine("for문:{0}, str :{1}", i, str);
                    controlList.Add(str);
                }
            }
            SendControlList_to_RequestingClient(ref controlList, signal);
            Console.WriteLine("Recv_and_SendControlList_to_RequestingClient End");

        }

        private void SendControlList_to_RequestingClient(ref List<string> list,int signal)
        {
            Console.WriteLine("SendControlList In");
            mutex.WaitOne();
            NetworkStream stream = FindNetworkStream(controlId, (int)FINDSTREAM.WRITE);
            mutex.ReleaseMutex();
            //NetworkStream stream = FindWriteNetworkStream(requestid);

            SendInt(stream, signal); // 처리 시그널 전송
            Delay(30);
            SendList(stream, list); //controllist 전송
           
            Console.WriteLine("SendControlList End");
        }

        //제어자에게 전체 해제요청
        private void RequestFullCancellation_to_Controller(int signal)
        {
            Console.WriteLine("RequestFullCancellation_to_controller In");
            string requestid = FindRequestId(myId);
            Console.WriteLine("requestId:{0}", requestid);
            mutex.WaitOne();
            NetworkStream stream = FindNetworkStream(requestid, (int)FINDSTREAM.WRITE);
            //NetworkStream stream = FindWriteNetworkStream(requestid);

            SendInt(stream, signal); // 처리 시그널 전송
            Delay(30);
            SendStr(stream, myId);
            Delay(30);
            foreach (UserInfo user in Server_Tcp.users)
            {
                if (user.Id.Equals(requestid))
                {
                    string password = FindPassword(user, myId);
                    Console.WriteLine("requestid: {0}, password: {1}", requestid, password);
                    if (password != null)
                        SendStr(stream, password);
                }
            }
            mutex.ReleaseMutex();
            Console.WriteLine("RequestFullCancellation_to_controller End");
        }

        //제어자가 해제 승인 전송
        private void ReleaseApproval_by_Controller(int signal)
        {
            Console.WriteLine("ReleaseApproval_by_Controller In");
            string id = RecvStr(); //해제 요청자 Id
            Console.WriteLine("id:{0}", id);
            NetworkStream stream = FindNetworkStream(id, (int)FINDSTREAM.WRITE);
            string pw = RecvStr();
            Console.WriteLine("pw:{0}", pw);
            mutex.WaitOne();
            foreach(UserInfo user in Server_Tcp.users)
            {
                if(user.Id.Equals(myId))
                {
                    bool check = CheckPassword(user, id, pw);
                    Console.WriteLine(check);
                    if (check)
                    {
                        SendInt((int)CHECK.SUCCESS);
                        Delay(30);
                        SendInt(stream, signal);
                        user.controlInfo.Remove(id);
                        user.controlIdList.Remove(id);
                    }
                    else
                        SendInt((int)CHECK.FAIL);
                }
            }
            mutex.ReleaseMutex();
            Console.WriteLine("ReleaseApproval_by_Controller End");
        }
        
        private string FindPassword(UserInfo user, string id)
        {
            foreach (KeyValuePair<string, string> key in user.controlInfo)
            {
                if (key.Key.Equals(id))
                    return key.Value;
            }
            return null;
        }

        private bool CheckPassword(UserInfo user, string id ,string pw)
        {
            foreach(KeyValuePair<string ,string> key in user.controlInfo)
            {
                if (key.Key.Equals(id) && key.Value.Equals(pw))
                    return true;       
            }
            return false;
        }

        private void SetControlInfo(string id, string controlid, string pw)
        {
            Console.WriteLine("SetControlInfo In");
            mutex.WaitOne();
            foreach (UserInfo user in Server_Tcp.users)
            {
                if (user.Id.Equals(id))
                {
                    Console.WriteLine("foreach In");
                    if(user.controlInfo.ContainsKey(controlid))
                    {
                        return;
                    }
                    user.controlInfo.Add(controlid, pw);
                    user.PasswordList.Add(pw);
                    Console.WriteLine("foreach End");

                }
            }
            mutex.ReleaseMutex();
            Console.WriteLine("SetControlInfo End");

        }

        private void SetControlIdList(string id, string controlid)
        {
            bool IsFound = false;
            mutex.WaitOne();
            foreach (UserInfo user in Server_Tcp.users)
            {
                if (user.Id.Equals(id))
                {
                    IsFound = FindControlId(user.controlIdList, controlid);
                    if(IsFound)
                    {
                        return;
                    }
                    user.controlIdList.Add(controlid);
                }
            }
            mutex.ReleaseMutex();
        }

        private bool FindControlId(List<string> list,string id)
        {
            foreach (string cid in list)
            {
                if(cid.Equals(id))
                {
                    return true;
                }
            }
            return false;
        }

        private void SetControlPasswordList(string id, string password)
        {
            mutex.WaitOne();
            foreach (UserInfo user in Server_Tcp.users)
            {
                if (user.Id.Equals(id))
                {
                    //user.controlIdList = new List<string>();
                    user.PasswordList.Add(password);
                }
            }
            mutex.ReleaseMutex();
        }

        private List<string> GetControlIdList(string myid)
        {
            mutex.WaitOne();
            foreach (UserInfo user in Server_Tcp.users)
            {
                if (user.Id.Equals(myid))
                {
                    return user.controlIdList;
                }
            }
            mutex.ReleaseMutex();
            return null;
        }

        private void SetStatus(string id, int status)
        {
            mutex.WaitOne();
            foreach (UserInfo user in Server_Tcp.users)
            {
                if(user.Id.Equals(id))
                {
                    user.Status = status;
                }
            }
            mutex.ReleaseMutex();
        }

        private void SetRequestId(string myid,string requestid)
        {
            mutex.WaitOne();
            foreach(UserInfo user in Server_Tcp.users)
            {
                if(user.Id.Equals(myid))
                {                    
                    user.requestId = requestid;
                }
            }
            mutex.ReleaseMutex();
        }

        private string FindRequestId(string myid)
        {
            mutex.WaitOne();
            foreach(UserInfo user in Server_Tcp.users)
            {
                if(user.Id.Equals(myid))
                {
                    return user.requestId;
                }
            }
            mutex.ReleaseMutex();
            return null;
        }
        private int RecvInt()
        {
            byte[] data = new byte[sizeof(int)];
            myreadStream.Read(data, 0, data.Length);
            int signal = BitConverter.ToInt32(data, 0);
            return signal;
        }

        private string RecvStr()
        {
            byte[] data = new byte[BUF];
            int bytes = myreadStream.Read(data, 0, data.Length);
            string str = Encoding.UTF8.GetString(data, 0, bytes);
            return str;
        }
        private void SendInt(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            mywriteStream.Write(data, 0, sizeof(int));
        }
        private void SendStr(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            mywriteStream.Write(data, 0, data.Length);
        }
        private void SendInt(NetworkStream stream,int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            stream.Write(data, 0, sizeof(int));
        }
        private void SendStr(NetworkStream stream ,string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }
        private void SendList(NetworkStream stream, List<string> list)
        {
            SendInt(stream, list.Count); // list count 전송
            Console.WriteLine("listcount: {0}", list.Count);
            Delay(30);
            foreach (string process in list)
            {
                SendStr(stream, process);
                Delay(50);
            }
        }
        private NetworkStream FindNetworkStream(string id,int num)
        {
            mutex.WaitOne();
            //NetworkStream temp = new NetworkStream(mywriteSock);
            foreach (UserInfo user in Server_Tcp.users)
            {
                Console.WriteLine("userid:{0},id:{1}", user.Id, id);
                Console.WriteLine(user.Id.Equals(id));
                Console.WriteLine(user.Id == id);
                Console.WriteLine(user.Id.Trim() == id.Trim());
                if (user.Id.Equals(id))
                {
                    if (num == 1)
                    {
                        Console.WriteLine("userid:{0},writeStream", user.Id);
                        return user.writeStream;
                    }
                    else
                    {
                        Console.WriteLine("userid:{0},readStream", user.Id);
                        return user.readStream;
                    }
                }
            }
            mutex.ReleaseMutex();
            Console.WriteLine("null");
            return null;
        }
        private NetworkStream FindWriteNetworkStream(string id)
        {
            NetworkStream temp = null;
            foreach (UserInfo user in Server_Tcp.users)
            {
                Console.WriteLine("userid:{0},id:{1}", user.Id, id);
                Console.WriteLine(user.Id.Equals(id));
                if (user.Id.Equals(id))
                {
                    Console.WriteLine("userid:{0},writeStream", user.Id);
                    temp = user.writeStream;
                }
            }
            return temp;
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
