using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.groups
{
    public class LineFilterGroup : ILineFilter
    {
        public readonly List<ILineFilter> filters = new List<ILineFilter>();
        
        public virtual bool shouldKeepLine(string line)
        {
            foreach (ILineFilter filter in filters)
                if (!filter.shouldKeepLine(line))
                    return false;
            
            return true;
        }
    }
}