using System.Collections.Generic;
using pnyx.net.util;

namespace pnyx.net.processors.sources
{
    public class LineProcessorSequence : ILinePart, IProcessor
    {
        public readonly List<IProcessor> processors = new List<IProcessor>();
        public ILineProcessor next { get; private set; }

        private int index;
        private StreamInformation streamInformation;

        public LineProcessorSequence(StreamInformation streamInformation)
        {
            this.streamInformation = streamInformation;
        }

        public void setNext(ILineProcessor next)
        {
            this.next = next;
        }

        public void process()
        {
            LineProcessorCollector collector = new LineProcessorCollector(next);
            foreach (ILinePart part in processors)
                part.setNext(collector);
            
            while (index < processors.Count && streamInformation.active)
            {
                IProcessor current = processors[index];                
                current.process();                
                index++;
            }
            
            next.endOfFile();
        }

        private class LineProcessorCollector : ILineProcessor
        {
            private readonly ILineProcessor next;

            public LineProcessorCollector(ILineProcessor next)
            {
                this.next = next;
            }

            public void processLine(string line)
            {
                next.processLine(line);
            }

            public void endOfFile()
            {
            }
        }
    }
}