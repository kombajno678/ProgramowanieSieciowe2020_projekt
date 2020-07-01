using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        private bool loopFlag;
        private Thread thread;

        private int delay;

        public TimeClient(string ip, int port, int delay)
        {
            SetAddress(ip);
            SetPort(port);
            this.delay = delay;
        }

        public bool IsRunning()
        {
            return thread.IsAlive;
        }

        public bool CheckConnection()
        {
            bool connected = false;

            verbose = false;
            try
            {
                Connect();
                Disconnect();
                connected = true;
            }
            catch(Exception e)
            {
                connected = false;
            }
            
            verbose = true;

            return connected;

        }

        public void Run()
        {
            

            loopFlag = true;

            thread = new Thread(()=>
            {
                try
                {

                    Connect();
                    while (loopFlag)
                    {
                        //remember time T1[ms]
                        double T1 = DateTime.Now.TimeOfDay.TotalMilliseconds;

                        //ask server for its time
                        try
                        {
                            Send("GET");
                        }
                        catch (Exception e)
                        {
                            Log("server closed connection");
                            break;
                        }
                        //receive servers time
                        string received = Receive();
                        double Tserv = Double.Parse(received.Replace(',', '.'));

                        //remember time Tcli = T2[ms]
                        double T2 = DateTime.Now.TimeOfDay.TotalMilliseconds;
                        double Tcli = T2;

                        double delta = Tserv + ((T2 - T1) / 2) - Tcli;

                        //print Tcli + delta

                        TimeSpan ts = TimeSpan.FromMilliseconds(Tcli + delta);
                        Console.WriteLine();

                        Log(" ... T1 = " + T1 + " ms");
                        Log(" ... Tserv = " + Tserv + " ms");
                        Log(" ... T2/Tcli = " + T2 + " ms");

                        Log("server time (Tcli + delta) = " + ts.ToString());
                        //Log("server time (Tcli + delta) = " + ts.ToString(@"hh\:mm\:ss"));
                        //print delta
                        Log(String.Format("delta = {0:0.000}ms", delta));

                        if (!loopFlag)
                        {
                            break;
                        }
                        Thread.Sleep(delay);
                        //slep
                    }
                }catch(ThreadInterruptedException e)
                {
                    //brr
                }catch(ThreadAbortException e)
                {
                    //brr
                }
                
                Disconnect();
            });
            thread.Start();


        }
        public void Stop()
        {
            loopFlag = false;

            thread.Interrupt();

            thread.Abort();
        }

        private void SetAddress(string address)
        {
            //todo: validate address
            this.serversAddress = address;
        }


        private void SetPort(int port)
        {

            if (port < System.Net.IPEndPoint.MinPort || port > System.Net.IPEndPoint.MaxPort)
            {
                throw new ArgumentException("incorrect port number");
            }

            this.serversPort = port;
        }
        private void SetBufferSize(int size)
        {
            //todo: validate size
            this.bufferSize = size;
        }

        private void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Log("resolving ip address of: " + serversAddress);
            Log("establishing connection to " + serversAddress + ":" + serversPort);
            var ipAdd = Dns.GetHostEntry(serversAddress);
            //find ipv4 address
            int i = 0;
            for (; i < ipAdd.AddressList.Length; i++)
            {
                if (ipAdd.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ipAdd.AddressList[i].ToString().Equals(serversAddress))
                {
                    break;
                }
            }

            System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd.AddressList[i], serversPort);

            socket.Connect(remoteEP);
            Log("connection established");
        }

        private void Disconnect()
        {
            socket.Close();
            /*
            if (verbose)
            {
                Console.Out.WriteLine(" client> disconnected from server");
            }
            */

        }

        private int Send(string msg)
        {
            if (msg.Length <= 0)
            {
                return 0;
            }
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);
            int bytesSent = 0;

            bytesSent = socket.Send(byData);
            /*
            if (verbose)
            {
                Log("message sent     : '" + msg + "', bytes: " + bytesSent);
            }
            */
            return bytesSent;
        }

        private string Receive()
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
            /*
            if (verbose)
            {
                Log(" client> message received : '" + recv + "', bytes: " + recv.Length);
            }
            */
            return recv;
        }

        public void Log(string text)
        {
            if (verbose)
            {
                Console.Out.WriteLine(DateTime.Now.TimeOfDay.ToString() + " TimeClient> " + text);
            }
        }

        


    }
}
