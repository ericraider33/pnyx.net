using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class SkipSpecificFilter : ILineFilter, IRowFilter
    {                
        private readonly List<int> linesToSkip = new List<int>();
        private int lineNumber;

        public SkipSpecificFilter(params int[] skip)
        {
            linesToSkip.AddRange(skip);    
        }
        
        public SkipSpecificFilter(IEnumerable<int> lines)
        {
            linesToSkip.AddRange(lines);
        }        
        
        public bool shouldKeepLine(String line)
        {
            return shouldKeep();
        }

        public bool shouldKeepRow(List<String> values)
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