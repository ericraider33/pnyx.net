using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.bools
{
    public class OrLineFilter : ILineFilter
    {
        public readonly List<ILineFilter> filters = new List<ILineFilter>();

        public OrLineFilter()
        {            
        }
        
        public OrLineFilter(IEnumerable<ILineFilter> source)
        {
            filters.AddRange(source);
        }        
        
        public bool shouldKeepLine(String line)
        {
            bool keep = false;
            foreach (ILineFilter filter in filters)
                keep |= filter.shouldKeepLine(line);
            
            return keep;
        }
    }
}