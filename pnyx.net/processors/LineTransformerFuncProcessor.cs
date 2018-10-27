using System;

namespace pnyx.net.processors
{
    public class LineTransformerFuncProcessor : ILineProcessor
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
    }
}