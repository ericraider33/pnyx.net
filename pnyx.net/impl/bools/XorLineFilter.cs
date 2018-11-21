using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.bools
{
    public class XorLineFilter : ILineFilter
    {
        public readonly List<ILineFilter> filters = new List<ILineFilter>();

        public XorLineFilter()
        {            
        }
        
        public XorLineFilter(IEnumerable<ILineFilter> source)
        {
            filters.AddRange(source);
        }        
        
        public bool shouldKeepLine(string line)
        {
            bool keep = false;
            foreach (ILineFilter filter in filters)
                keep ^= filter.shouldKeepLine(line);
            
            return keep;
        }
    }
}