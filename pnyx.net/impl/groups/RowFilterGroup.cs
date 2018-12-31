using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.groups
{
    public class RowFilterGroup : IRowFilter
    {
        public readonly List<IRowFilter> filters = new List<IRowFilter>();

        public virtual bool shouldKeepRow(List<String> rows)
        {
            foreach (IRowFilter filter in filters)
                if (!filter.shouldKeepRow(rows))
                    return false;
            
            return true;
        }
    }
}