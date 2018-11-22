using System.Collections.Generic;

namespace pnyx.net.processors
{
    public class RowProcessorSequence : IRowPart, IProcessor
    {
        public readonly List<IProcessor> processors = new List<IProcessor>();
        public IRowProcessor next { get; private set; }

        private int index;
        
        public void setNext(IRowProcessor next)
        {
            this.next = next;
        }

        public void process()
        {
            RowProcessorCollector collector = new RowProcessorCollector(next);
            foreach (IRowPart part in processors)
                part.setNext(collector);
            
            while (index < processors.Count)
            {
                IProcessor current = processors[index];                
                current.process();                
                index++;
            }
            
            next.endOfFile();
        }

        private class RowProcessorCollector : IRowProcessor
        {
            private readonly IRowProcessor next;

            public RowProcessorCollector(IRowProcessor next)
            {
                this.next = next;
            }

            public void processRow(string[] row)
            {
                next.processRow(row);
            }

            public void endOfFile()
            {
            }
        }
    }
}