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
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

            
            
            Console.WriteLine("starting DiscoveryListener  ...");
            DiscoveryListener ds = new DiscoveryListener();
            ds.Run();


            Console.WriteLine("starting TimeListener  ...");
            TimeListener ls = new TimeListener();
            ls.Run();



            Console.ReadKey();

            ds.Stop();
            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }
}
