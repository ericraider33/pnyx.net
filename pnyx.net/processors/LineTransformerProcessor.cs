using pnyx.net.api;

namespace pnyx.net.processors
{
    public class LineTransformerProcessor : ILinePart, ILineProcessor
    {
        public ILineTransformer transform;
        public ILineProcessor processor;

        public void processLine(string line)
        {
            line = transform.transformLine(line);
            if (line != null)
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