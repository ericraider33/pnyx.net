using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowFilterFunc : IRowFilter
    {
        public Func<String[],bool> rowFilterFunc;
        
        public bool shouldKeepRow(string[] row)
        {
            return rowFilterFunc(row);
        }        
    }
}