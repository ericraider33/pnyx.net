using System.Collections.Generic;

namespace pnyx.net.processors
{
    public class LineProcessorSequence : ILinePart, IProcessor
    {
        public readonly List<IProcessor> sequence = new List<IProcessor>();
        public ILineProcessor lineProcessor { get; private set; }

        private int index;
        
        public void setNext(ILineProcessor next)
        {
            lineProcessor = next;
        }

        public void process()
        {
            LineProcessorCollector collector = new LineProcessorCollector(lineProcessor);
            foreach (ILinePart part in sequence)
                part.setNext(collector);
            
            while (index < sequence.Count)
            {
                IProcessor next = sequence[index];                
                next.process();                
                index++;
            }
            
            lineProcessor.endOfFile();
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