using System;
using pnyx.net.api;

namespace pnyx.net.processors.rows
{
    public class RowBufferingProcessor : IRowPart, IRowProcessor
    {
        public IRowBuffering buffering;
        public IRowProcessor processor;

        public void rowHeader(String[] header)
        {
            header = buffering.rowHeader(header);
            if (header != null)
                processor.rowHeader(header);
        }

        public void processRow(String[] row)
        {
            forward(buffering.bufferingRow(row));
        }

        public void endOfFile()
        {
            forward(buffering.endOfFile());
            processor.endOfFile();
        }

        private void forward(String[][] rows)
        {
            if (rows == null)
                return;
            
            foreach (String[] row in rows)
                processor.processRow(row);
        }

        public void setNextRowProcessor(IRowProcessor next)
        {
            processor = next;
        }        
    }
}