using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace serwer
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("starting TimeListeners  ...");
            TimeListener timeListener = new TimeListener();
            timeListener.Run();
            
            Console.WriteLine("starting DiscoveryListeners  ...");
            foreach(string address in timeListener.GetAddresses())
            {
                DiscoveryListener ds = new DiscoveryListener(address);
                ds.Run();
            }
            
            
            Console.ReadKey();
 
        }
    }
}
