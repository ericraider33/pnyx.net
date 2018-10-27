using System;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd
{
    class Program
    {
        static void Main(string[] args)
        {            
            using (Pnyx p = new Pnyx())
            {                
//                p.read("c:/dev/ee.txt");
//                p.grep("s");
//                p.sedAppend("nextLine");
//                p.write("c:/dev/ee1.txt");
//                p.process();

                p.readCsv("c:/dev/pnyx.net/pnyx.net.test/files/csv/books.csv");
//                p.grep("dickens");
                p.lineFilter(x => x.containsIgnoreCase("dickens"));
                p.write("c:/dev/pnyx.net/pnyx.net.test/out/csv/dickens.csv");
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