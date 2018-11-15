using System;

namespace pnyx.net.processors
{
    public class LineFilterFuncProcessor : ILinePart, ILineProcessor
    {
        public Func<String, bool> transform;
        public ILineProcessor processor;

        public void processLine(string line)
        {
            if (transform(line))
                processor.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }

        public void setNext(ILineProcessor next)
        {
            processor = next;
        }
    }
}