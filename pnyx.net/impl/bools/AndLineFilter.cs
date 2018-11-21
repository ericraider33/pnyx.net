using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.impl.groups;

namespace pnyx.net.impl.bools
{
    // Sugar for group
    public class AndLineFilter : LineFilterGroup
    {        
        public AndLineFilter()
        {
        }

        public AndLineFilter(IEnumerable<ILineFilter> source)
        {
            filters.AddRange(source);
        }        
    }
}