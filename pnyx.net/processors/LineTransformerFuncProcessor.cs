using System;

namespace pnyx.net.processors
{
    public class LineTransformerFuncProcessor : ILinePart, ILineProcessor
    {
        public Func<String,String> transform;
        public ILineProcessor processor;

        public void processLine(string line)
        {
            line = transform(line);
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