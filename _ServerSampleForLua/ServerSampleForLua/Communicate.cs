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
    class Communicate {

        string str_Read = null;
        bool isConnected = false;
        NetworkStream netStream = null;    //  网络流
        TextReader txtReader = null;
        Thread thread_AcceptMsg = null;

        public void StartNewComunicateThread(Socket clientSocket) {
            thread_AcceptMsg = new Thread(RcvMsg);
            thread_AcceptMsg.Start(clientSocket);
        }

        private void RcvMsg(object clientSocket) {
            Socket client = clientSocket as Socket;

            if (client == null) {
                Console.WriteLine("client is null");
                return;
            }
            isConnected = true;
            Console.WriteLine("Connected with client: " + client.RemoteEndPoint.ToString());

            netStream = new NetworkStream(client);
            txtReader = new StreamReader(netStream);

            while (isConnected) {
                try {
                    str_Read = txtReader.ReadLine();
                    if (str_Read.Length > 0) {
                        lock (this) {
                            string msg = client.RemoteEndPoint + ":" + str_Read;
                            Console.WriteLine(msg);
                        }
                    }
                } catch {
                    thread_AcceptMsg.Abort();
                }
            }
        }
    }
}
