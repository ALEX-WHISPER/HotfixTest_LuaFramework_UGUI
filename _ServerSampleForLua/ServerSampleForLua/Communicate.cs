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
        bool isConnected = false;
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

            while (isConnected) {
                try {
                    byte[] readBuffer = new byte[100];
                    int count = client.Receive(readBuffer);
                    if (count == 0) {
                        isConnected = false;
                        Console.WriteLine("---Disconnected---");
                        return;
                    }

                    DispByteStream(readBuffer, count);
                    DecodeProtocol(readBuffer);
                    Echo(client, readBuffer, count);
                } catch {
                    thread_AcceptMsg.Abort();
                }
            }
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        //  显示字节流
        private void DispByteStream(byte[] buffer, int count) {
            string showStr = "";
            for (int i = 0; i < count; i++) {
                int b = (int)buffer[i];
                showStr += b.ToString() + " ";
            }

            Console.WriteLine("ByteStream：" + showStr);
        }

        //  解析协议
        private void DecodeProtocol(byte[] buffer) {
            Int16 messageLen = BitConverter.ToInt16(buffer, 0);
            Int16 protocol = BitConverter.ToInt16(buffer, 2);
            if (protocol.Equals(Protocol.ProtocolNum.Exception) || protocol.Equals(Protocol.ProtocolNum.Disconnect)) {
                isConnected = false;
                return;
            }

            Int16 strLen = BitConverter.ToInt16(buffer, 4);
            string str = System.Text.Encoding.UTF8.GetString(buffer, 6, strLen);
            Console.WriteLine("Length: " + messageLen);
            Console.WriteLine("Protocol：" + protocol);
            Console.WriteLine("Content：" + str);
        }

        //  echo
        private void Echo(Socket client, byte[] originBuffer, int count) {
            byte[] writeBuffer = new byte[count];
            Array.Copy(originBuffer, writeBuffer, count);
            client.Send(writeBuffer);
        }
    }
}
