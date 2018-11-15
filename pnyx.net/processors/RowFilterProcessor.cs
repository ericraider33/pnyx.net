using pnyx.net.api;

namespace pnyx.net.processors
{
    public class RowFilterProcessor : IRowPart, IRowProcessor
    {
        public IRowFilter transform;
        public IRowProcessor processor;

        public void processRow(string[] row)
        {
            if (transform.shouldKeepRow(row))
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