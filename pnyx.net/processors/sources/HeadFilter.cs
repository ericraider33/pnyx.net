using System;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.processors.sources
{
    public class HeadFilter : ILineFilter, IRowFilter
    {
        public int limit;

        private StreamInformation streamInformation;
        private int lineNumber;

        public HeadFilter(StreamInformation streamInformation, int limit)
        {
            this.streamInformation = streamInformation;
            this.limit = limit;
        }
        
        public bool shouldKeepLine(String line)
        {
            lineNumber++;
            if (!streamInformation.active)
                return false;

            streamInformation.active = lineNumber <= limit;
            return streamInformation.active;
        }

        public bool shouldKeepRow(String[] row)
        {
            lineNumber++;
            if (!streamInformation.active)
                return false;

            streamInformation.active = lineNumber <= limit;
            return streamInformation.active;
        }
    }
}