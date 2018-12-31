using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.impl.groups;

namespace pnyx.net.impl.bools
{
    public class NotLineFilter : LineFilterGroup
    {
        public NotLineFilter()
        {
        }

        public NotLineFilter(IEnumerable<ILineFilter> source)
        {
            filters.AddRange(source);
        }        
        
        public override bool shouldKeepLine(String line)
        {
            return !base.shouldKeepLine(line);
        }
    }
}