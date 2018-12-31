using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowFilterFunc : IRowFilter
    {
        public Func<List<String>,bool> rowFilterFunc;
        
        public bool shouldKeepRow(List<String> row)
        {
            return rowFilterFunc(row);
        }        
    }
}