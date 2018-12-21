using System;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd.code
{
    public class LineGlobals : BaseGlobals
    {
        public String line;
        
        public LineGlobals(StreamInformation streamInformation, Settings settings) : 
            base(streamInformation, settings)
        {
        }
    }
}