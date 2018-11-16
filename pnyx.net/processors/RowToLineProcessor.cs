using System;
using pnyx.net.api;

namespace pnyx.net.processors
{
    public class RowToLineProcessor : IRowProcessor, ILinePart
    {
        public IRowConverter rowConverter;
        public ILineProcessor processor;
        
        public void processRow(string[] row)
        {
            String line = rowConverter.rowToLine(row);
            processor.processLine(line);
        }

        public void processLine(string line)
        {
            processor.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }

        public void setNext(ILineProcessor next)
        {
            processor = next;
        }
    }
}