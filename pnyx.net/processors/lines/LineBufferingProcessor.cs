using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.lines
{
    public class LineBufferingProcessor : ILinePart, ILineProcessor
    {
        public ILineBuffering buffering;
        public ILineProcessor processor;

        public void processLine(String line)
        {
            forward(buffering.bufferingLine(line));
        }

        public void endOfFile()
        {
            forward(buffering.endOfFile());
            processor.endOfFile();
        }

        private void forward(List<String> lines)
        {
            if (lines == null)
                return;
            
            foreach (String line in lines)
                processor.processLine(line);
        }

        public void setNextLineProcessor(ILineProcessor next)
        {
            processor = next;
        }        
    }
}