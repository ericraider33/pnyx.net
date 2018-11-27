using System;
using pnyx.net.fluent;
using pnyx.net.impl;

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

//                p.read("c:/dev/pnyx.net/pnyx.net.test/files/csv/books.csv");
//                p.parseCsv();
//                p.grep("dickens");
//                p.write("c:/dev/pnyx.net/pnyx.net.test/out/csv/dickens.csv");
//                p.process();

                p.read("C:/dev/asclepius/prod_import/Medicare_Provider_Util_Payment_PUF_CY2016.txt");
                p.lineFilter(new LineNumberSkip(2));
                p.parseTab();
                p.columnDefinition(hasHeaderRow: true, nullable: true);
                p.swapColumnsAndRows();
                p.writeStdout();
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