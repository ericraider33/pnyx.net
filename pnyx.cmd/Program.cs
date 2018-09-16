using System;
using System.IO;
using pnyx.net.filters;
using pnyx.net.processors;

namespace pnyx.cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream input = new FileStream("c:\\dev\\ee.txt", FileMode.Open, FileAccess.Read);
            TextReader reader = new StreamReader(input);

            Stream output = new FileStream("c:\\dev\\ee1.txt", FileMode.Open, FileAccess.Write);
            TextWriter writer = new StreamWriter(output);
            
            ReaderToLineProcessor a = new ReaderToLineProcessor { reader = reader };
            LineFilterProcessor b = new LineFilterProcessor { filter = new Grep { textToFind = "s" }};
            LineProcessorToWriter c = new LineProcessorToWriter { writer = writer };

            a.lineProcessor = b;
            b.processor = c;
            
            a.process();
            
            writer.Flush();
            input.Dispose();
            output.Dispose();
            
            Console.WriteLine("Done");
        }
    }
}