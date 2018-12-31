using System;

namespace pnyx.net.processors.dest
{
    public class RowTeeProcessor : IRowProcessor, IRowPart
    {
        public IRowProcessor processor;
        public IRowProcessor tee { get; private set; }

        public RowTeeProcessor(IRowProcessor tee)
        {
            this.tee = tee;
        }

        public void rowHeader(String[] header)
        {
            processor.processRow(header);
            tee.processRow(header);            
        }

        public void processRow(String[] row)
        {
            processor.processRow(row);
            tee.processRow(row);
        }

        public void endOfFile()
        {
            processor.endOfFile();
            tee.endOfFile();
        }

        public void setNextRowProcessor(IRowProcessor next)
        {
            processor = next;
        }
    }
}