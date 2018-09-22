using System;
using System.Collections.Generic;
using pnyx.net.errors;

namespace pnyx.net.util
{
    public class IndexRange : ICloneable
    {
        public static readonly IndexRange NULL_RANGE = new IndexRange(1, 0);

        public int low;
        public int high;

        public IndexRange()
        {            
        }
        
        public IndexRange(int low, int high)
        {
            this.low = low;
            this.high = high;
        }

        public bool isSingleIndex()
        {
            return low == high;
        }
                
        public bool containsInclusive(int value)
        {
            return low <= value && value <= high;
        }

        public void narrow(IndexRange rule)
        {
            narrow(rule.low, rule.high);
        }

        public void narrow(int? newLow, int? newHight)
        {
            if (newLow.HasValue)
                low = Math.Max(low, newLow.Value);
            
            if (newHight.HasValue)
                high = Math.Min(high, newHight.Value);
        }

        public override string ToString()
        {
            if (low == high)
                return low.ToString();

            return String.Format("{0}-{1}", low, high);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static List<IndexRange> parse(String text)
        {
            List<IndexRange> result = new List<IndexRange>();
            if (string.IsNullOrEmpty(text))
                return result;
            
            String[] parts = text.Split(',');
            foreach (String part in parts)
            {
                String[] innerParts = part.Split('-');
                if (innerParts.Length == 1)
                {
                    int num = int.Parse(innerParts[0]);
                    result.Add(new IndexRange(num, num));
                }
                else if (innerParts.Length == 2 && innerParts[0].Length > 0 && innerParts[1].Length > 0)
                {
                    result.Add(new IndexRange(int.Parse(innerParts[0]), int.Parse(innerParts[1])));
                }
                else
                {
                    throw new InvalidArgumentException("Invalid range: " + text);
                }
            }
            return result;
        }
    }

}