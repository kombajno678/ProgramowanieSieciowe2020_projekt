using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        private static string multicastAddress = "239.0.0.222";
        static void Main(string[] args)
        {

            //send discovery multicast udp
            DiscoverySender ds = new DiscoverySender();
            ds.Run();




            bool offerChosen = false;
            List<string> offers = null;
            int index = -1;
            while (!offerChosen)
            {
                Console.WriteLine("p - print offers");
                Console.WriteLine("[offer number] - choose offer");
                string choice = Console.ReadLine();

                if (choice.Equals("p"))
                {
                    //print offers
                    offers = ds.GetOffers();
                    Console.WriteLine("server offers : ");
                    for (int i = 0; i < offers.Count(); i++)
                    {
                        Console.WriteLine(String.Format("[{0}] - {1}", i, offers[i]));
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
                    Console.WriteLine("offert choosen : " + offers[index]);
                    break;
                }
            }
            Console.WriteLine("input delay between requests: [ms]");
            int delay = Int32.Parse(Console.ReadLine());
            Console.WriteLine("connecting to time server ...");
            ds.Stop();



            string ip = offers[index].Split(':')[0];
            int port = Int32.Parse(offers[index].Split(':')[1]);

            TimeClient tc = new TimeClient(ip, port, delay);
            tc.Run();



            Console.ReadKey(); 

    }
}
}
