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
                p.read("c:/dev/pnyx.net/pnyx.net.test/files/tab/icd10.txt");
                p.parseTab();
                p.writeSplit("icd10.$0.txt", 99, "c:/dev/pnyx.net/pnyx.net.test/files/tab");
                p.process();
            }
           
            Console.WriteLine("Done");
        }
    }
}