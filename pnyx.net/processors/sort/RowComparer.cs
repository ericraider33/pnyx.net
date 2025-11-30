using System;
using System.Collections.Generic;
using pnyx.net.impl.columns;

namespace pnyx.net.processors.sort;

public class RowComparer : IComparer<List<String>>
{
    public class ColumnDefinition
    {
        public ColumnIndex columnIndex;
        public IComparer<String> comparer;
    }

    private readonly List<ColumnDefinition> definitions;
        
    public RowComparer(IEnumerable<ColumnDefinition> definitions)
    {
        this.definitions = new List<ColumnDefinition>(definitions);
    }
        
    public int Compare(List<String> x, List<String> y)
    {
        foreach (ColumnDefinition cd in definitions)
        {
            String colX = cd.columnIndex < x.Count ? x[cd.columnIndex] : "";
            String colY = cd.columnIndex < y.Count ? y[cd.columnIndex] : "";

            int result = cd.comparer.Compare(colX, colY);
            if (result != 0)
                return result;
        }

        return 0;
    }
}