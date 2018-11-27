using System;
using pnyx.net.api;

namespace pnyx.net.processors.converters
{
    public class LineToRowProcessor : ILineProcessor, IRowPart
    {
        public IRowConverter rowConverter;
        public IRowProcessor processor;
        
        public void processLine(string line)
        {
            String[] row = rowConverter.lineToRow(line);
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