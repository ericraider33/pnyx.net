using System;

namespace pnyx.net.processors.rows
{
    public class RowTransformerFuncProcessor : IRowPart, IRowProcessor
    {
        public Func<String[],String[]> transform;
        public IRowProcessor processor;

        public void processRow(string[] row)
        {
            row = transform(row);
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