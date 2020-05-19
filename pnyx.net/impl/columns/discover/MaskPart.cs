using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.util;

namespace pnyx.net.impl.columns.discover
{
    public class MaskPart
    {
        public int dataLengthMin { get; set; } = Int32.MaxValue;
        public int dataLengthMax { get; set; } 
        public bool hasNumbers { get; set; }
        public bool hasLetters { get; set; }

        private readonly Dictionary<String, int> tokenMap = new Dictionary<String, int>();
        
        public void addPart(String data, String token = null)
        {
            dataLengthMax = Math.Max(dataLengthMax, data.Length);
            dataLengthMin = Math.Min(dataLengthMin, data.Length);

            if (!hasNumbers || !hasLetters)
            {
                foreach (Char c in data)
                {
                    if (Char.IsLetter(c))
                        hasLetters = true;
                    if (Char.IsDigit(c))
                        hasNumbers = true;
                }
            }

            if (!String.IsNullOrEmpty(token))
                tokenMap.increaseCount(token);
        }

        public bool isTokenConsistent()
        {
            return tokenMap.Count < 2;
        }

        public String getToken()
        {
            if (tokenMap.Count == 1)
                return tokenMap.Keys.First();

            return null;
        }

        public int getTokenCount()
        {
            if (tokenMap.Count == 1)
                return tokenMap.Values.First();

            return 0;
        }
    }
}