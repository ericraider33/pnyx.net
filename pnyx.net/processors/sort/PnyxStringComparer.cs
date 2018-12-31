using System;
using System.Collections.Generic;

namespace pnyx.net.processors.sort
{
    public class PnyxStringComparer : IComparer<String>
    {
        private readonly Func<char,char,int> charFunc;

        public PnyxStringComparer(bool descending, bool caseSensitive)
        {
            if (caseSensitive)
            {
                if (descending)
                    charFunc = caseSensitiveDescending;
                else
                    charFunc = caseSensitiveAscending;
            }
            else
            {
                if (descending)
                    charFunc = caseInsensitiveDescending;
                else
                    charFunc = caseInsensitiveAscending;
            }
        }
        
        public int Compare(String x, String y)
        {
            int max = Math.Min(x.Length, y.Length);
            for (int i = 0; i < max; i++)
            {
                char cX = x[i];
                char cY = y[i];

                int result = charFunc(cX, cY);
                if (result != 0)
                    return result;
            }

            if (x.Length == y.Length)
                return 0;
            else if (x.Length < y.Length)
                return charFunc('a', 'b');
            else
                return charFunc('b', 'a');
        }

        private int caseSensitiveAscending(char x, char y)
        {
            return x.CompareTo(y);
        }

        private int caseSensitiveDescending(char x, char y)
        {
            return y.CompareTo(x);
        }

        private int caseInsensitiveAscending(char x, char y)
        {            
            return Char.ToLowerInvariant(x).CompareTo(Char.ToLowerInvariant(y));
        }

        private int caseInsensitiveDescending(char x, char y)
        {
            return Char.ToLowerInvariant(y).CompareTo(Char.ToLowerInvariant(x));
        }
    }
}