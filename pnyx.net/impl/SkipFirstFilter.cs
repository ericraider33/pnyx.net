using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class SkipFirstLinesFilter : ILineFilter, IRowFilter
    {
        private int lineNumber;
        
        public int linesToSkip;

        public SkipFirstLinesFilter(int linesToSkip)
        {
            this.linesToSkip = linesToSkip;
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
            return lineNumber > linesToSkip;
        }
    }
}