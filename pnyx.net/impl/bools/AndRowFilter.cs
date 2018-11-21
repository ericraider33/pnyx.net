using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.impl.groups;

namespace pnyx.net.impl.bools
{
    // Sugar for group
    public class AndRowFilter : RowFilterGroup
    {
        public AndRowFilter()
        {
        }

        public AndRowFilter(IEnumerable<IRowFilter> source)
        {
            filters.AddRange(source);
        }
    }
}