using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowFilterFuncShim : IRowFilter
    {
        public Func<String,bool> lineFilter;
        
        public bool shouldKeepRow(string[] row)
        {
            foreach (String column in row)
                if (lineFilter(column))
                    return true;

            return false;
        }
    }
}