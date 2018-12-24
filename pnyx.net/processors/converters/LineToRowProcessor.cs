using System;
using pnyx.net.api;

namespace pnyx.net.processors.converters
{
    public class LineToRowProcessor : ILineProcessor, IRowPart
    {
        public bool hasHeader;        
        public IRowConverter rowConverter;
        public IRowProcessor processor;
        private int lineNumber;
        
        public void processLine(string line)
        {
            lineNumber++;
            
            String[] row = rowConverter.lineToRow(line);
            if (lineNumber == 1 && hasHeader)
                processor.rowHeader(row);
            else
                processor.processRow(row);
        }

        public void processRow(string[] row)
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