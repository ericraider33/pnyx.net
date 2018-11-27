using pnyx.net.api;

namespace pnyx.net.processors.rows
{
    public class RowFilterProcessor : IRowPart, IRowProcessor
    {
        public IRowFilter filter;
        public IRowProcessor processor;

        public void processRow(string[] row)
        {
            if (filter.shouldKeepRow(row))
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