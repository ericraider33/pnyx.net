using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.bools
{
    public class OrRowFilter : IRowFilter
    {
        public readonly List<IRowFilter> filters = new List<IRowFilter>();

        public OrRowFilter()
        {            
        }
        
        public OrRowFilter(IEnumerable<IRowFilter> source)
        {
            filters.AddRange(source);
        }
        
        public bool shouldKeepRow(String[] row)
        {
            bool keep = false;
            foreach (IRowFilter filter in filters)
                keep |= filter.shouldKeepRow(row);
            
            return keep;
        }
    }
}