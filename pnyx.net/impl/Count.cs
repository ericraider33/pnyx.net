using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class Count : IRowBuffering, ILineBuffering
    {
        public bool checkData;
        
        private int lineCount;
        private List<int> rowCounts = new List<int>();
        
        public List<string> rowHeader(List<string> header)
        {
            return header;    // no-op
        }

        public List<List<string>> bufferingRow(List<string> row)
        {
            for (int i = rowCounts.Count; i < row.Count; i++)
                rowCounts.Add(0);

            for (int i = 0; i < row.Count; i++)
            {
                int rowCount = rowCounts[i];
                if (!checkData || !String.IsNullOrEmpty(row[i]))
                    rowCount++;
                rowCounts[i] = rowCount;
            }

            return null;
        }

        public List<string> bufferingLine(string line)
        {
            if (!checkData || !String.IsNullOrEmpty(line))
                lineCount++;
            
            return null;
        }

        List<string> ILineBuffering.endOfFile()
        {
            return new List<string> { lineCount.ToString() };
        }

        List<List<string>> IRowBuffering.endOfFile()
        {
            List<String> output = rowCounts.Select(x => x.ToString()).ToList();
            return new List<List<string>> { output };
        }
    }
}