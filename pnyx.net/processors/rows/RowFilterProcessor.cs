using System;
using pnyx.net.api;

namespace pnyx.net.processors.rows
{
    public class RowFilterProcessor : IRowPart, IRowProcessor
    {
        public IRowFilter filter;
        public IRowProcessor processor;

        public void rowHeader(String[] header)
        {
            processor.rowHeader(header);
        }

        public void processRow(String[] row)
        {
            if (filter.shouldKeepRow(row))
                processor.processRow(row);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }

        public void setNextRowProcessor(IRowProcessor next)
        {
            processor = next;
        }        
    }
}