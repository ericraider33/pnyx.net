using System;
using System.IO;

namespace pnyx.net.processors
{
    public class ReaderToLineProcessor : ILineProcessorPlug
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

        public ILineProcessor getLineProcessor()
        {
            return lineProcessor;
        }

        public void setLineProcess(ILineProcessor processor)
        {
            lineProcessor = processor;
        }
    }
}