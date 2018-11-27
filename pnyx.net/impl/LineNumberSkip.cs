using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class LineNumberSkip : ILineFilter, IRowFilter
    {                
        private readonly List<int> linesToSkip = new List<int>();
        private int lineNumber;

        public LineNumberSkip(params int[] skip)
        {
            linesToSkip.AddRange(skip);    
        }
        
        public LineNumberSkip(IEnumerable<int> lines)
        {
            linesToSkip.AddRange(lines);
        }        
        
        public bool shouldKeepLine(string line)
        {
            return shouldKeep();
        }

        public bool shouldKeepRow(string[] values)
        {
            return shouldKeep();
        }
        
        private bool shouldKeep()
        {
            lineNumber++;
            if (linesToSkip.Contains(lineNumber))
            {
                linesToSkip.Remove(lineNumber);
                return false;                
            }

            return true;
        }
    }
}