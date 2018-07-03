using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerSampleForLua {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("---This is a sample server for LuaFramework_UGUI---");
            
            Connect conn = new Connect();
            conn.StartServer();
        }
    }
}
