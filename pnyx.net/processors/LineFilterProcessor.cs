using pnyx.net.api;

namespace pnyx.net.processors
{
    public class LineFilterProcessor : ILineProcessor
    {
        public ILineFilter filter;
        public ILineProcessor processor;

        public void process(string line)
        {
            if (filter.shouldKeep(line))
                processor.process(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }
    }
}