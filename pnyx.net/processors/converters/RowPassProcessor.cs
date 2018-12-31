using System;
using System.Collections.Generic;

namespace pnyx.net.processors.converters
{
    public class RowPassProcessor : IRowProcessor, IRowPart
    {
        public IRowProcessor processor;

        public void rowHeader(List<String> header)
        {
            processor.rowHeader(header);
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