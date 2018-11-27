using System;
using pnyx.net.api;

namespace pnyx.net.processors.rows
{
    public class RowBufferingProcessor : IRowPart, IRowProcessor
    {
        public IRowBuffering transform;
        public IRowProcessor processor;

        public void processRow(string[] row)
        {
            forward(transform.bufferingRow(row));
        }

        public void endOfFile()
        {
            forward(transform.endOfFile());
            processor.endOfFile();
        }

        private void forward(String[][] rows)
        {
            if (rows == null)
                return;
            
            foreach (String[] row in rows)
                processor.processRow(row);
        }

        public void setNext(IRowProcessor next)
        {
            processor = next;
        }        
    }
}