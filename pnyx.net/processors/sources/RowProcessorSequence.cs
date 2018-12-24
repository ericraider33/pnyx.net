using System;
using System.Collections.Generic;
using pnyx.net.util;

namespace pnyx.net.processors.sources
{
    public class RowProcessorSequence : IRowPart, IProcessor
    {
        public readonly List<IProcessor> processors = new List<IProcessor>();
        public IRowProcessor next { get; private set; }

        private int index;
        private StreamInformation streamInformation;

        public RowProcessorSequence(StreamInformation streamInformation)
        {
            this.streamInformation = streamInformation;
        }

        public void setNext(IRowProcessor next)
        {
            this.next = next;
        }

        public void process()
        {
            RowProcessorCollector collector = new RowProcessorCollector(next);
            foreach (IRowPart part in processors)
                part.setNext(collector);
            
            while (index < processors.Count && streamInformation.active)
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
            private bool hasRowHeader;

            public RowProcessorCollector(IRowProcessor next)
            {
                this.next = next;
            }

            public void rowHeader(String[] header)
            {
                if (hasRowHeader)
                    return;

                next.rowHeader(header);
                hasRowHeader = true;
            }

            public void processRow(String[] row)
            {
                next.processRow(row);
            }

            public void endOfFile()
            {
            }
        }
    }
}