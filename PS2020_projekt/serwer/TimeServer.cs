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
    class TimeServer
    {
        public bool verbose = true;

        private Socket socket;

        private bool loopFlag;
        private Thread thread;

        public TimeServer(Socket s)
        {
            socket = s;
        }

        public void Run()
        {
            loopFlag = true;
            thread = new Thread(()=>{
                try
                {
                    while (loopFlag)
                    {
                        Loop();
                    }
                }catch(ThreadInterruptedException e)
                {

                }catch(ThreadAbortException e)
                {

                }

            });
            thread.Start();



        }


        public void Stop()
        {
            loopFlag = false;
            thread.Interrupt();
            thread.Abort();

        }

        private void Loop()
        {
            //receive msg
            string strData = Receive();
            if(strData == null)
            {
                //server closed connection
                Log("client closed connection");
                Stop();
            }
            //send system time
            TimeSpan time = DateTime.Now.TimeOfDay;
            Send(time.TotalMilliseconds.ToString());
        }


        public string Receive()
        {
            byte[] buffer = new byte[1024];

            int iRx = 0;
            System.String recv = "";

            //exception here when server is dead
            try
            {
                iRx = socket.Receive(buffer);

            }catch(Exception e)
            {
                return null;
            }
            char[] chars = new char[iRx];

            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
            recv = new System.String(chars);

            if (verbose)
            {
                Log("message received : '" + recv + "', bytes: " + recv.Length);
            }

            return recv;
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
                Log("message sent     : '" + msg + "', bytes: " + bytesSent);
            }
            return bytesSent;
        }

        public void Log(string text)
        {
            if (verbose)
            {
                Console.Out.WriteLine("TimeServer> " + text);
            }
        }




    }
}
