using System;
using System.Collections.Generic;

namespace pnyx.net.processors.sort
{
    public class RowComparer : IComparer<String[]>
    {
        public class ColumnDefinition
        {
            public int columnNumber;
            public IComparer<String> comparer;
        }

        private readonly List<ColumnDefinition> definitions;
        
        public RowComparer(IEnumerable<ColumnDefinition> definitions)
        {
            this.definitions = new List<ColumnDefinition>(definitions);
        }
        
        public int Compare(String[] x, String[] y)
        {
            foreach (ColumnDefinition cd in definitions)
            {
                String colX = cd.columnNumber <= x.Length ? x[cd.columnNumber - 1] : "";
                String colY = cd.columnNumber <= y.Length ? y[cd.columnNumber - 1] : "";

                int result = cd.comparer.Compare(colX, colY);
                if (result != 0)
                    return result;
            }

            return 0;
        }
    }
}