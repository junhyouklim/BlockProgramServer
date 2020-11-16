using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server_Tcp
{
    class MainApp
    {
        private static void Main(string[] args)
        {
            Server_Tcp server = new Server_Tcp(9043);
            server.ServerOn();
        }
    }
}
