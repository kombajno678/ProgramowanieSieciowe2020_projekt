using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        private static string multicastAddress = "239.0.0.222";

        private static string lastConnectedServerFile = "previous_server.txt";

        static void Main(string[] args)
        {

            bool loopFlag = true;
            while (loopFlag)
            {
                
                //send discovery multicast udp
                DiscoverySender ds = new DiscoverySender();
                ds.Run();
            

                bool offerChosen = false;
                List<string> offers = null;
                int index = -1;
                bool quitFlag = false;
                while (!offerChosen)
                {
                    Console.WriteLine("q - quit program");
                    Console.WriteLine("p - print offers");
                    Console.WriteLine("[offer number] - choose offer");
                    string choice = Console.ReadLine();

                    if (choice.Equals("q"))
                    {
                        loopFlag = false;
                        quitFlag = true;
                        break;
                    }
                    else if (choice.Equals("p"))
                    {
                        //read ip of server, client was previously connected to 
                        string lastIp = ReadLastServer();
                        
                        //print offers
                        offers = ds.GetOffers();
                        Console.WriteLine("server offers : ");
                        for (int i = 0; i < offers.Count(); i++)
                        {
                            if (lastIp.Equals(offers[i].Split(':')[0]))
                            {
                                Console.WriteLine(String.Format("[{0}] - {1} <- default", i, offers[i]));
                           
                            }
                            else
                            {
                                Console.WriteLine(String.Format("[{0}] - {1}", i, offers[i]));

                            }
                        }
                    }
                    else
                    {
                        offers = ds.GetOffers();
                        index = -1;
                        if(!Int32.TryParse(choice, out index))
                        {
                            continue;
                        }
                        if(index < 0 ||  (offers != null && index > offers.Count()))
                        {
                            continue;
                        }
                        Console.WriteLine("offer choosen : " + offers[index]);
                        break;
                    }
                }
                if (quitFlag)
                {
                    ds.Stop();
                    break;
                }
                Console.WriteLine("input delay between requests: [ms]");
                int delay = Int32.Parse(Console.ReadLine());
                Console.WriteLine("connecting to time server ...");
                ds.Stop();



                string ip = offers[index].Split(':')[0];
                int port = Int32.Parse(offers[index].Split(':')[1]);

                TimeClient tc = new TimeClient(ip, port, delay);

                //check conenction
                bool canConnectToServer = tc.CheckConnection();
                if(!canConnectToServer){
                    Console.WriteLine("Error: cannot establish connection to server");
                    continue;
                }
                //if good, save, connect
                // save servers ip
                WriteLastServer(ip);

                tc.Run();

                //check if still connected to server every x seconds
                while (true)
                {
                    if (!tc.IsRunning())
                    {
                        Console.WriteLine("Error: lost connection to server");
                        break;
                    }
                    Thread.Sleep(1);
                }

            }
            Console.WriteLine("exiting ... ");



        }


        public static string ReadLastServer()
        {
            
            string ip = "";
            try
            {
                using (StreamReader sr = new StreamReader(lastConnectedServerFile))
                {
                    string line;
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        ip = line;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning: could not read ip of previous server from text file (\"" + lastConnectedServerFile + "\")");
                //Console.WriteLine(e.Message);
            }
            return ip;
        }


        public static void WriteLastServer(string ip)
        {

            File.Create(lastConnectedServerFile).Close();
            try
            {
                using (StreamWriter sw = new StreamWriter(lastConnectedServerFile))
                {

                    sw.WriteLine(ip);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Warning: could not save ip of server to text file  (\"" + lastConnectedServerFile + "\")");
            }
            
            
        }
    }
}
