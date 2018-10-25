using pnyx.net.api;

namespace pnyx.net.processors
{
    public class LineFilterProcessor : ILineProcessor
    {
        public ILineFilter transform;
        public ILineProcessor processor;

        public void processLine(string line)
        {
            if (transform.shouldKeepLine(line))
                processor.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }
    }
}