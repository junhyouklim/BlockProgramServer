using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockProgramServer
{
    class UserInfo
    {
        private int sock;
        public int Sock
        {
            get { return sock; }
            set { sock = value; }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        List<string> ctrlProcessList;
    }
}
