using System;
using pnyx.net.processors;

namespace pnyx.net.impl.columns
{
    public class ColumnToLine : IRowProcessor, ILinePart
    {
        public int index;                                        // zero based
        public ILineProcessor processor;
        
        public void processRow(string[] row)
        {
            String line = index < row.Length ? row[index] : "";
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
