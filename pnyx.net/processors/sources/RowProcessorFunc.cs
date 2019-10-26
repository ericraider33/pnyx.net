using System;
using System.Collections.Generic;

namespace pnyx.net.processors.sources
{
    public class RowProcessorFunc : IRowPart, IProcessor
    {
        public Func<List<String>> header  { get; private set; }        
        public Func<IEnumerable<List<String>>> source { get; private set; }
        public IRowProcessor next { get; private set; }

        public RowProcessorFunc(Func<List<string>> header, Func<IEnumerable<List<string>>> source)
        {
            this.header = header;
            this.source = source;
        }

        public void setNextRowProcessor(IRowProcessor next)
        {
            this.next = next;
        }

        public void process()
        {
            if (header != null)
            {
                List<String> headerData = header();
                if (headerData != null)
                    next.rowHeader(headerData);
            }

            IEnumerable<List<String>> data = source();
            foreach (List<String> row in data)
                next.processRow(row);
            
            next.endOfFile();
        }
    }
}