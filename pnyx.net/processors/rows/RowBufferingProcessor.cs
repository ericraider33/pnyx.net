using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.rows
{
    public class RowBufferingProcessor : IRowPart, IRowProcessor
    {
        public IRowBuffering buffering;
        public IRowProcessor processor;

        public void rowHeader(List<String> header)
        {
            header = buffering.rowHeader(header);
            if (header != null)
                processor.rowHeader(header);
        }

        public void processRow(List<String> row)
        {
            forward(buffering.bufferingRow(row));
        }

        public void endOfFile()
        {
            forward(buffering.endOfFile());
            processor.endOfFile();
        }

        private void forward(List<List<String>> rows)
        {
            if (rows == null)
                return;
            
            foreach (List<String> row in rows)
                processor.processRow(row);
        }

        public void setNextRowProcessor(IRowProcessor next)
        {
            processor = next;
        }        
    }
}