using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;

namespace pnyx.net.impl.columns
{
    public class SwapColumnsAndRows : IRowBuffering
    {
        private readonly List<List<String>> buffer = new List<List<String>>();
        private int lineNumber = 0;
        
        public string[][] bufferingRow(string[] row)
        {
            lineNumber++;

            for (int i = buffer.Count; i < row.Length; i++)
            {
                List<String> outRow = new List<String>();
                buffer.Add(outRow);

                for (int j = outRow.Count; j < lineNumber - 1; j++)
                    outRow.Add("");
            }

            for (int i = 0; i < buffer.Count; i++)
            {
                String column = i < row.Length ? row[i] : "";
                List<String> outRow = buffer[i];
                outRow.Add(column);
            }

            return null;
        }

        public string[][] endOfFile()
        {
            string[][] result = buffer.Select(x => x.ToArray()).ToArray();
            return result;
        }
    }
}