using System;

namespace pnyx.net.processors.converters
{
    public class LinePassProcessor : ILineProcessor, ILinePart
    {
        public ILineProcessor processor;
        
        public void processLine(String line)
        {
            processor.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }

        public void setNextLineProcessor(ILineProcessor next)
        {
            processor = next;
        }
    }
}