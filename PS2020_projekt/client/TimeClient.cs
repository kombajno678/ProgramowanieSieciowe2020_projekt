using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class TimeClient
    {

        private Socket socket;
        private int timeOut = 5;
        private string serversAddress = "";
        private int serversPort = 0;

        public bool verbose = true;
        private int bufferSize = 1024;

        public TimeClient(string ip, int port)
        {
            serversAddress = ip;
            serversPort = port;
        }

        public void SetAddress(string address)
        {
            //todo: validate address
            this.serversAddress = address;
        }
        public void SetPort(int port)
        {

            if (port < System.Net.IPEndPoint.MinPort || port > System.Net.IPEndPoint.MaxPort)
            {
                throw new ArgumentException("incorrect port number");
            }

            this.serversPort = port;
        }
        public void SetBufferSize(int size)
        {
            //todo: validate size
            this.bufferSize = size;
        }

        public void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (verbose) Console.WriteLine(" client> resolving ip address of: " + serversAddress);
            var ipAdd = Dns.GetHostEntry(serversAddress);
            //find ipv4 address
            int i = 0;
            for (; i < ipAdd.AddressList.Length; i++)
            {
                if (ipAdd.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    break;
                }
            }

            System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd.AddressList[i], serversPort);

            if (verbose) Console.WriteLine(" client> establishing connection to {0}:{1}", ipAdd.AddressList[i].ToString(), serversPort);
            socket.Connect(remoteEP);
            if (verbose) Console.WriteLine(" client> connection established");
        }

        public void Disconnect()
        {
            socket.Close();
            if (verbose)
            {
                Console.Out.WriteLine(" client> disconnected from server");
            }

        }

        public int Send(string msg)
        {
            if (msg.Length <= 0)
            {
                return 0;
            }
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);
            int bytesSent = 0;

            bytesSent = socket.Send(byData);
            if (verbose)
            {
                Console.Out.WriteLine(" client> message sent     : '" + msg + "', bytes: " + bytesSent);
            }
            return bytesSent;
        }

        public string Receive()
        {
            byte[] buffer = new byte[1024];

            int iRx = 0;
            System.String recv = "";

            //exception here when server is dead
            iRx = socket.Receive(buffer);
            char[] chars = new char[iRx];

            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
            recv = new System.String(chars);

            if (verbose)
            {
                Console.Out.WriteLine(" client> message received : '" + recv + "', bytes: " + recv.Length);
            }

            return recv;
        }


    }
}
