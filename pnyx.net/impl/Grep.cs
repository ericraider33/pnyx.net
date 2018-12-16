using System;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl
{
    public class Grep : ILineFilter
    {
        public String textToFind;
        public bool caseSensitive;
                
        public bool shouldKeepLine(string line)
        {
            bool match;
            if (caseSensitive)
                match = line.Contains(textToFind);
            else
                match = line.containsIgnoreCase(textToFind);

            return match;
        }
    }
}