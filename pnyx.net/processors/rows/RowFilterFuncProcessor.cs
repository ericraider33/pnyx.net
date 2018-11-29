using System;

namespace pnyx.net.processors.rows
{
    public class RowFilterFuncProcessor : IRowPart, IRowProcessor
    {
        public Func<String[], bool> filter;
        public IRowProcessor processor;

        public void processRow(string[] row)
        {
            if (filter(row))
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