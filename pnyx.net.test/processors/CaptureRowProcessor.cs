using System;
using System.Collections.Generic;
using pnyx.net.processors;

namespace pnyx.net.test.processors
{
    public class CaptureRowProcessor : IRowProcessor
    {
        public List<String[]> rows { get; }
        public bool eof { get; private set; }

        public CaptureRowProcessor()
        {
            rows = new List<String[]>();        
        }        
        
        public void processRow(String[] row)
        {
            rows.Add(row);
        }

        public void endOfFile()
        {
            eof = true;
        }
    }
}