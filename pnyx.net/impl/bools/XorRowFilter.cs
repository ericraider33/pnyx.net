using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.bools
{
    public class XorRowFilter : IRowFilter
    {
        public readonly List<IRowFilter> filters = new List<IRowFilter>();

        public XorRowFilter()
        {            
        }
        
        public XorRowFilter(IEnumerable<IRowFilter> source)
        {
            filters.AddRange(source);
        }
        
        public bool shouldKeepRow(String[] row)
        {
            bool keep = false;
            foreach (IRowFilter filter in filters)
                keep ^= filter.shouldKeepRow(row);
            
            return keep;
        }
    }
}