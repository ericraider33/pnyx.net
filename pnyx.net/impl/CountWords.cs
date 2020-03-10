using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class CountWords : IRowBuffering, ILineBuffering
    {
        private int lineCount;
        private List<int> rowCounts = new List<int>();
        
        public List<string> rowHeader(List<string> header)
        {
            // Assures headers are included in output even if no data in present in remainder of input
            for (int i = rowCounts.Count; i < header.Count; i++)
                rowCounts.Add(0);                                

            return header;    // no-op
        }

        public List<List<string>> bufferingRow(List<string> row)
        {
            for (int i = rowCounts.Count; i < row.Count; i++)
                rowCounts.Add(0);

            for (int i = 0; i < row.Count; i++)
                rowCounts[i] += countWords(row[i]);

            return null;
        }

        public List<string> bufferingLine(string line)
        {
            lineCount += countWords(line);
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

        public static int countWords(String text)
        {
            if (text == null)
                return 0;

            int count = 0;
            bool inWord = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (Char.IsWhiteSpace(c))
                {
                    inWord = false;
                }
                else if (!inWord)
                {
                    count++;
                    inWord = true;
                }
            }

            return count;
        }
    }
}