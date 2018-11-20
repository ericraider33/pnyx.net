using pnyx.net.api;

namespace pnyx.net.processors
{
    public class LineFilterProcessor : ILinePart, ILineProcessor
    {
        public ILineFilter filter;
        public ILineProcessor processor;

        public void processLine(string line)
        {
            if (filter.shouldKeepLine(line))
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