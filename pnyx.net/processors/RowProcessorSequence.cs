using System.Collections.Generic;

namespace pnyx.net.processors
{
    public class RowProcessorSequence : IRowPart, IProcessor
    {
        public readonly List<IProcessor> sequence = new List<IProcessor>();
        public IRowProcessor rowProcessor { get; private set; }

        private int index;
        
        public void setNext(IRowProcessor next)
        {
            rowProcessor = next;
        }

        public void process()
        {
            RowProcessorCollector collector = new RowProcessorCollector(rowProcessor);
            foreach (IRowPart part in sequence)
                part.setNext(collector);
            
            while (index < sequence.Count)
            {
                IProcessor next = sequence[index];                
                next.process();                
                index++;
            }
            
            rowProcessor.endOfFile();
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