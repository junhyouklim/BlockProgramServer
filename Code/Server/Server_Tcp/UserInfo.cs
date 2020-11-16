using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server_Tcp
{
    class UserInfo
    {
        public Socket readSock { get; set; } //서버 read전용 readsocket 클라이언트가 write한것
        public NetworkStream readStream { get; set; } //서버 read전용 readStream 클라이언트가 write한것
        public Socket writeSock { get; set; } //서버 write전용 writesocket 클라이언트가 read할것
        public NetworkStream writeStream { get; set; } //서버 write전용 writeStream 클라이언트가 read할것
        public string Id { get; set; } //본인 Id
        public string requestId { get; set; } //본인을 제어해주는 Id
        public Dictionary<string,string> controlInfo { get; set; } //key:본인이 제어해주는 id, value:제어자 pw
        public List<string> controlIdList { get; set; } //본인이 제어해주는 id
        public List<string> PasswordList { get; set; } //제어자 pw
        public int Status { get; set; } //제어 상태
        public UserInfo(NetworkStream stream)
        {
            readStream = stream;
        }
        public UserInfo(NetworkStream readstream, Socket readsock,string id)
        {
            readStream = readstream;
            readSock = readsock;
            Id = id;
            controlInfo = new Dictionary<string, string>();
            controlIdList = new List<string>();
            PasswordList = new List<string>();
        }
        public UserInfo(NetworkStream stream, Socket sock, int num)
        {
            if(num==1)
            {
                writeStream = stream;
                writeSock = sock;
            }
            else
            {
                readStream = stream;
                readSock = sock;
            }
        }
    }
}
