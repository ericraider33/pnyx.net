using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class LineFilterFunc : ILineFilter
    {
        public Func<String, bool> lineFilterFunc;
        
        public bool shouldKeepLine(string line)
        {
            return lineFilterFunc(line);
        }
    }
}