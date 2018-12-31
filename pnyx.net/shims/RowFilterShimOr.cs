using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowFilterShimOr : IRowFilter
    {
        public ILineFilter lineFilter;
        
        public bool shouldKeepRow(List<String> row)
        {
            foreach (String column in row)
                if (lineFilter.shouldKeepLine(column))
                    return true;

            return false;
        }
    }
}