using System;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.transforms
{
    public class Grep : ILineFilter, IRowFilter
    {
        public String textToFind;
        public bool caseSensitive;
        public bool invert;               
                
        public bool shouldKeepLine(string line)
        {
            bool match;
            if (caseSensitive)
                match = line.containsIgnoreCase(textToFind);
            else
                match = line.Contains(textToFind);

            return match ^ invert;
        }

        public bool shouldKeepRow(string[] values)
        {
            bool match = false;
            for (int i = 0; i < values.Length && !match; i++)
            {
                if (caseSensitive)
                    match = values[i].containsIgnoreCase(textToFind);
                else
                    match = values[i].Contains(textToFind);                
            }

            return match ^ invert;
        }
    }
}