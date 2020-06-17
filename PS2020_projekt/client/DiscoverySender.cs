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
    class DiscoverySender
    {
        private static string MulticastAddress = "239.0.0.222";
        private static int Port = 7;

        private bool loopFlag;

        private Thread threadListener;
        private Thread threadSender;

        UdpClient clientListener;
        IPEndPoint localEpListener;

        UdpClient udpclientSender;
        IPEndPoint remoteepSender;

        List<string> offers;

        public DiscoverySender()
        {
            offers = new List<string>();
        }

        public List<string> GetOffers()
        {
            return new List<string>(offers);
        }
        public void Run()
        {
            loopFlag = true;

            //send discovery every 10s
            //listen for responses
            SendSetup();
            ListenSetup();

            threadListener = new Thread(()=> {
                while (loopFlag)
                {
                    ListenLoop();
                }
            });
            threadSender = new Thread(() => {
                while (loopFlag)
                {
                    SendLoop();
                    //sleep for 10s
                    Thread.Sleep(10 * 1000);
                }
            });

            threadListener.Start();
            threadSender.Start();


        }
        public void Stop()
        {
            //TODO
            loopFlag = false;

            threadListener.Interrupt();
            threadSender.Interrupt();

            threadListener.Abort();
            threadSender.Abort();


        }
        private void SendSetup()
        {
            IPAddress multicastaddress = IPAddress.Parse(MulticastAddress);
            udpclientSender = new UdpClient();
            try
            {
                udpclientSender.JoinMulticastGroup(multicastaddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("error: cant join multicast group");
                return;
            }
            remoteepSender = new IPEndPoint(multicastaddress, Port);
        }
        private void SendLoop()
        {
            //send discovery
            string msg = "DISCOVERY";

            Byte[] buffer = null;
            buffer = Encoding.Unicode.GetBytes(msg);
            udpclientSender.Send(buffer, buffer.Length, remoteepSender);
            //sleep for 10s

        }
        private void ListenSetup()
        {
            clientListener = new UdpClient();

            clientListener.ExclusiveAddressUse = false;
            localEpListener = new IPEndPoint(IPAddress.Any, Port);
            clientListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            clientListener.Client.Bind(localEpListener);

            IPAddress multicastaddress = IPAddress.Parse(MulticastAddress);
            try
            {
                clientListener.JoinMulticastGroup(multicastaddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("error: cant join multicast group");
                return;
            }
            Console.WriteLine("UdpMulticastListener> now waiting for incoming messages...");
        }
        private void ListenLoop()
        {
            Byte[] data = clientListener.Receive(ref localEpListener);
            string strData = Encoding.Unicode.GetString(data);
            if (strData.Split(' ')[0].Equals("OFFER"))
            {
                Console.WriteLine("UdpListener> '" + strData + "', from: " + localEpListener.ToString());

                string offer = strData.Split(' ')[1] + ":" + strData.Split(' ')[2];
                
                offers.Add(offer);
            }
        }


    }
}
