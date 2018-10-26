using pnyx.net.api;

namespace pnyx.net.processors
{
    public class LineTransformerProcessor : ILineProcessor
    {
        public ILineTransformer transform;
        public ILineProcessor processor;

        public void processLine(string line)
        {
            line = transform.transformLine(line);
            processor.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }
    }
}