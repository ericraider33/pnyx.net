using System;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class HasLine : ILineFilter
    {
        public bool shouldKeepLine(string line)
        {
            return !String.IsNullOrEmpty(line);
        }
    }
}