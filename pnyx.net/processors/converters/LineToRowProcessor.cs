using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.converters
{
    public class LineToRowProcessor : ILineProcessor, IRowPart
    {
        public bool hasHeader;        
        public IRowConverter rowConverter;
        public IRowProcessor processor;
        private int lineNumber;
        
        public void processLine(String line)
        {
            lineNumber++;
            
            List<String> row = rowConverter.lineToRow(line);
            if (lineNumber == 1 && hasHeader)
                processor.rowHeader(row);
            else
                processor.processRow(row);
        }

        public void processRow(List<String> row)
        {
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