using pnyx.net.api;

namespace pnyx.net.processors
{
    public class LineFilterProcessor : ILineProcessor
    {
        public ILineFilter transform;
        public ILineProcessor processor;

        public void process(string line)
        {
            if (transform.shouldKeep(line))
                processor.process(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }
    }
}