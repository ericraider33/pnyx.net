using System;
using pnyx.net.fluent;

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

                p.readCsv("c:/dev/ee.txt", strict: false);
                p.write("c:/dev/ee1.txt");
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