using System;
using System.IO;
using pnyx.net.filters;
using pnyx.net.fluent;
using pnyx.net.processors;

namespace pnyx.cmd
{
    class Program
    {
        static void Main(string[] args)
        {            
            using (Pnyx p = new Pnyx())
            {                
                p.read("c:\\dev\\ee.txt");
                p.grep("s");
                p.write("c:\\dev\\ee1.txt");
                p.process();
            }
           
           /* 
            using (new Pnyx().read("c:\\dev\\ee.txt").grep("s").write("c:\\dev\\ee1.txt").process())
            {                
            }
            */
            Console.WriteLine("Done");
        }
    }
}