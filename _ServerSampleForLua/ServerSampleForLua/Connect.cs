using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServerSampleForLua {
    class Connect {
        #region global vars
        IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
        int portNum = 12121;
        IPEndPoint ipEdp = null;

        Socket svrSocket = null;    //  服务端套接字
        Socket clientSocket = null; //  客户端套接字

        Thread thread_Listenning = null;

        Communicate comObj = null;

        //  保存服务器端所有与客户端通信的套接字
        Dictionary<string, Socket> Dic_Socket = new Dictionary<string, Socket>();

        //  保存服务器端所有调用AcceptMsg()方法的线程
        Dictionary<string, Thread> Dic_Thread = new Dictionary<string, Thread>();
        #endregion
        /// <summary>
        /// 启动
        /// </summary>
        public void StartServer() {
            ipEdp = new IPEndPoint(ipAdr, portNum);
            svrSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            svrSocket.Bind(ipEdp);

            svrSocket.Listen(0);
            Console.WriteLine("---Start server---");

            comObj = new Communicate();

            //  start listenning thread
            thread_Listenning = new Thread(ListenningConnection);
            thread_Listenning.Start();
        }

        /// <summary>
        /// 监听连接请求
        /// </summary>
        private void ListenningConnection() {
            while (true) {
                clientSocket = svrSocket.Accept();
                if (Dic_Socket.ContainsValue(clientSocket)) {
                    return;
                }

                Dic_Socket.Add(clientSocket.RemoteEndPoint.ToString(), clientSocket);
                comObj.StartNewComunicateThread(clientSocket);
            }
        }
    }
}
