using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class LineNumberFilter : ILineFilter, IRowFilter
    {                
        private readonly List<int> linesToKeep = new List<int>();
        private int lineNumber;

        public LineNumberFilter(IEnumerable<int> lines)
        {
            linesToKeep.AddRange(lines);
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
            if (!linesToKeep.Contains(lineNumber))
                return false;

            linesToKeep.Remove(lineNumber);
            return true;
        }
    }
}