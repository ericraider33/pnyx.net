using System;
using System.IO;

namespace pnyx.net.processors
{
    public class ReaderToLineProcessor : IProcessor
    {
        public TextReader reader;
        public ILineProcessor lineProcessor;

        public void process()
        {
            String line;
            while ((line = reader.ReadLine()) != null)
            {
                lineProcessor.process(line);
            }
        }
    }
}