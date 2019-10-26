using System;
using System.Collections.Generic;

namespace pnyx.net.processors.sources
{
    public class LineProcessorFunc : ILinePart, IProcessor
    {
        public Func<IEnumerable<String>> source { get; private set; }
        public ILineProcessor next { get; private set; }

        public LineProcessorFunc(Func<IEnumerable<string>> source)
        {
            this.source = source;
        }

        public void setNextLineProcessor(ILineProcessor next)
        {
            this.next = next;
        }

        public void process()
        {
            IEnumerable<String> data = source();
            foreach (String line in data)
                next.processLine(line);
            
            next.endOfFile();
        }
    }
}