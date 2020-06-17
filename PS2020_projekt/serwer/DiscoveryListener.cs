using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace serwer
{
    class DiscoveryListener
    {
        public bool verbose = true;
        private static string multicastAddress = "239.0.0.222";

        public int port = 7;
        

        private string offer;

        private bool loopFlag;

        private Thread discoveryThread;

        private UdpClient client;
        private IPEndPoint localEp;




        public DiscoveryListener()
        {
            offer = "OFFER 127.0.0.1 4562";
        }
        public void Run()
        {
            
            discoveryThread = new Thread(() => {
                StartServer();
                while (loopFlag)
                {
                    Loop();
                }
                CloseServer();
            });

            discoveryThread.Start();
            
        }
        public void Stop()
        {
            loopFlag = false;
            discoveryThread.Interrupt();
            discoveryThread.Abort();
        }

        private void StartServer()
        {
            client = new UdpClient();

            client.ExclusiveAddressUse = false;
            localEp = new IPEndPoint(IPAddress.Any, port);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            client.Client.Bind(localEp);

            IPAddress multicastaddress = IPAddress.Parse(multicastAddress);
            try
            {
                client.JoinMulticastGroup(multicastaddress);
            }
            catch (Exception e)
            {
                Log("error: cant join multicast group");
                return;
            }
            Log("now waiting for incoming messages...");
            loopFlag = true;
        }
        private void Loop()
        {
            Byte[] data = client.Receive(ref localEp);
            string strData = Encoding.Unicode.GetString(data);
            Log("'" + strData + "', from: " + localEp.ToString());
        }

        public void CloseServer()
        {
            Log("closing server");
            loopFlag = false;
            discoveryThread.Interrupt();
            discoveryThread.Abort();
        }

        private void Log(string text)
        {
            if (verbose)
            {
                Console.Out.WriteLine("discovery listener> " + text);
            }
        }
        
    }
}
