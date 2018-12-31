using System;
using pnyx.net.api;

namespace pnyx.net.processors.rows
{
    public class RowTransformerProcessor : IRowPart, IRowProcessor
    {
        public IRowTransformer transform;
        public IRowProcessor processor;

        public void rowHeader(String[] header)
        {
            header = transform.transformHeader(header);
            if (header != null)
                processor.rowHeader(header);            
        }

        public void processRow(String[] row)
        {
            row = transform.transformRow(row);
            if (row != null)
                processor.processRow(row);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }

        public void setNext(IRowProcessor next)
        {
            processor = next;
        }        
    }
}