using System;
using System.Collections.Generic;
using pnyx.net.processors;

namespace pnyx.net.impl.columns
{
    public class ColumnToLine : IRowProcessor, ILinePart
    {
        public int index;                                        // zero based
        public ILineProcessor processor;

        public void rowHeader(List<String> header)
        {
            String line = index < header.Count ? header[index] : "";
            processor.processLine(line);
        }

        public void processRow(List<String> row)
        {
            String line = index < row.Count ? row[index] : "";
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
