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
    class TimeListener
    {
        public bool verbose = true;
        private string ip;
        private int port;
        private int backlog = 10;

        private List<Socket> listeningSockets = null;



        private List<Thread> listeningthreads = null;

        private List<Socket> connectionSockets = null;

        private bool loopFlag;

        private List<TimeServer> clientList = null;

        //private IPEndPoint localep;



        public TimeListener()
        {
            clientList = new List<TimeServer>();
            listeningSockets = new List<Socket>();
            connectionSockets = new List<Socket>();
        }
        public void Run()
        {
            StartServer();
            loopFlag = true;

            listeningthreads = new List<Thread>();

            foreach(Socket s in listeningSockets)
            {
                Thread listener = new Thread(() =>
                {
                    while (loopFlag)
                    {
                        WaitForClient(s);
                    }
                });
                listeningthreads.Add(listener);
                listener.Start();
            }

           
            
        }

        public List<string> GetAddresses ()
        {
            List<string> addresses = new List<string>();
            foreach (Socket s in listeningSockets)
            {
                addresses.Add(s.LocalEndPoint.ToString());
            }

            return addresses;
        }
        
        public List<TimeServer> GetClients()
        {
            return clientList;
        }
        
        public void StartServer()
        {
            Log("starting server...");

            foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (i.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up ||
                    i.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Loopback) continue;
                foreach (var ua in i.GetIPProperties().UnicastAddresses)
                {
                    if (ua.Address.AddressFamily.ToString().EndsWith("V6")) continue;
                    Console.WriteLine(ua.Address + " - " + ua.Address.AddressFamily);
                    
                    Socket listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //bind

                    IPAddress[] addresslist = Dns.Resolve(ua.Address.ToString()).AddressList;
                    IPAddress hostIP = addresslist[0];
                    IPEndPoint ep = new IPEndPoint(hostIP, 0);
                    listeningSocket.Bind(ep);

                    listeningSocket.Listen(backlog);
                    //ip = listeningSocket.LocalEndPoint.ToString().Split(':')[0];
                    //port = Int32.Parse(listeningSocket.LocalEndPoint.ToString().Split(':')[1]);
                    //port = ep.Port;
                    Log("server started, " + listeningSocket.LocalEndPoint.ToString());

                    listeningSockets.Add(listeningSocket);

                }
            }

            
        }
        public void WaitForClient(Socket s)
        {
            Log("waiting for next client to connect ... ");

            Socket connectionSocket = s.Accept();
            connectionSockets.Add(connectionSocket);

            Log("new client has connected, start TimerServer for him");

            // Or lambda expressions if you are using C# 3.0
            TimeServer client = new TimeServer(connectionSocket);
            clientList.Add(client);
            client.Run();

        }

        public void CloseServer()
        {
            Log("closing server");
            try
            {
                foreach (Thread t in listeningthreads)
                    t.Abort();

                foreach (Socket s in listeningSockets)
                    s.Close();
                
            }
            catch (Exception ignored) { }

        }



        public void Log(string text)
        {
            if (verbose)
            {
                Console.Out.WriteLine("TimeListener> " + text);
            }
        }
    }
}
