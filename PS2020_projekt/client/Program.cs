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
            while (!offerChosen)
            {
                Console.WriteLine("p - print offers");
                Console.WriteLine("[offer number] - choose offer");
                string choice = Console.ReadLine();

                if (choice.Equals("p"))
                {
                    //print offers
                    List<string> offers = ds.GetOffers();
                    for(int i = 0; i < offers.Count(); i++)
                    {
                        Console.WriteLine(String.Format("{0} - {1}\n", i, offers[i]));
                    }
                }
            }

           

            Console.ReadKey();
            //get responds
            //every 10 s print them
            //let user chosoe server
            //connect to server



    }
}
}
