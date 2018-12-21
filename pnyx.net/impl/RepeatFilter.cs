using System;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl
{
    public class RepeatFilter : ILineFilter, IRowFilter
    {
        private String previousLine;
        private String[] previousRow;
        
        public bool shouldKeepLine(string line)
        {
            if (line.Equals(previousLine))
                return false;

            previousLine = line;
            return true;
        }

        public bool shouldKeepRow(string[] row)
        {
            if (RowUtil.isEqual(row, previousRow))
                return false;

            previousRow = row;
            return true;
        }
    }
}