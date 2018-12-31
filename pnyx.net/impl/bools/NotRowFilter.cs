using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.impl.groups;

namespace pnyx.net.impl.bools
{
    public class NotRowFilter : RowFilterGroup
    {
        public NotRowFilter()
        {            
        }
        
        public NotRowFilter(IEnumerable<IRowFilter> source)
        {
            filters.AddRange(source);
        }
        
        public override bool shouldKeepRow(List<String> row)
        {
            return !base.shouldKeepRow(row);
        }        
    }
}