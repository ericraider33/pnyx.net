using System;

namespace pnyx.net.processors.converters
{
    public class RowPassProcessor : IRowProcessor, IRowPart
    {
        public IRowProcessor processor;

        public void rowHeader(String[] header)
        {
            processor.rowHeader(header);
        }

        public void processRow(String[] row)
        {
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