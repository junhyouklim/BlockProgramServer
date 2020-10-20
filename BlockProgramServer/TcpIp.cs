using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace BlockProgramServer
{
    class TcpIp
    {
        private TcpListener server = null;
        private NetworkStream stream;
        private List<UserInfo> users = null;
        public TcpIp(string bindIp, int bindPort)
        {
            try
            {
                IPEndPoint localAddress = new IPEndPoint(IPAddress.Parse(bindIp), bindPort);
                server = new TcpListener(localAddress);
                server.Start();
            }
            catch(SocketException e)
            {
                Console.WriteLine(e);
            }
        }
        public void ServerOn()
        {
            while(true)
            {
                //Socket sock = server.AcceptSocket();
                TcpClient clnt = server.AcceptTcpClient();
                Console.WriteLine("클라이언트 접속 : {0}", ((IPEndPoint)clnt.Client.RemoteEndPoint).ToString());
                stream = clnt.GetStream();
                UserInfo user = new UserInfo();
                user.Sock = sock;
                users.Add(user);
                Console.WriteLine("클라이언트 Stream : {0}", stream.ToString());
            }
        }
        public void ReceiveThread()
        {
            int signal;

        }
        public void SendThread()
        {

        }
        public void SendInt(int num)
        {
            byte[] data = BitConverter.GetBytes(num);
            stream.Write(data, 0, data.Length);
        }
        public void SendStr(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            stream.Write(data, 0, data.Length);
        }
        public int ReceiveInt()
        {
            byte[] data = new byte[sizeof(int)];
            stream.Read(data, 0, data.Length);
            int result = BitConverter.ToInt32(data, 0);
            return result;
        }
        public string ReceiveStr()
        {
            byte[] data = new byte[512];
            stream.Read(data, 0, data.Length);
            string result = Encoding.UTF8.GetString(data);
            return result;
        }
        public void ServerClose()
        {
            server.Stop();
        }
    }
}
