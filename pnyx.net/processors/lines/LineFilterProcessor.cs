using System;
using pnyx.net.api;

namespace pnyx.net.processors.lines
{
    public class LineFilterProcessor : ILinePart, ILineProcessor
    {
        public ILineFilter filter;
        public ILineProcessor processor;

        public void processLine(String line)
        {
            if (filter.shouldKeepLine(line))
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