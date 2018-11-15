using System;
using pnyx.net.api;

namespace pnyx.net.processors
{
    public class LineBufferingProcessor : ILinePart, ILineProcessor
    {
        public ILineBuffering transform;
        public ILineProcessor processor;

        public void processLine(string line)
        {
            forward(transform.bufferingLine(line));
        }

        public void endOfFile()
        {
            forward(transform.endOfFile());
            processor.endOfFile();
        }

        private void forward(String[] lines)
        {
            if (lines == null)
                return;
            
            foreach (String line in lines)
                processor.processLine(line);
        }

        public void setNext(ILineProcessor next)
        {
            processor = next;
        }        
    }
}