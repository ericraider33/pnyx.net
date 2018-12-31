using System;
using pnyx.net.api;

namespace pnyx.net.processors.converters
{
    public class RowToLineProcessor : IRowProcessor, ILinePart
    {
        public IRowConverter rowConverter;
        public ILineProcessor processor;

        public void rowHeader(String[] header)
        {
            String line = rowConverter.rowToLine(header);
            processor.processLine(line);
        }

        public void processRow(String[] row)
        {
            String line = rowConverter.rowToLine(row);
            processor.processLine(line);
        }

        public void processLine(String line)
        {
            processor.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }

        public void setNextLineProcessor(ILineProcessor next)
        {
            processor = next;
        }
    }
}