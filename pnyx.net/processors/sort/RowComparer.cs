using System;
using System.Collections.Generic;
using pnyx.net.impl.columns;

namespace pnyx.net.processors.sort;

public class RowComparer : IComparer<List<String?>>
{
    public class ColumnDefinition
    {
        public ColumnIndex columnIndex { get; }
        public IComparer<String?> comparer { get; }

        public ColumnDefinition(ColumnIndex columnIndex, IComparer<string?> comparer)
        {
            this.columnIndex = columnIndex;
            this.comparer = comparer;
        }
    }

    private readonly List<ColumnDefinition> definitions;
        
    public RowComparer(IEnumerable<ColumnDefinition> definitions)
    {
        this.definitions = new List<ColumnDefinition>(definitions);
    }
        
    public int Compare(List<String?>? x, List<string?>? y)
    {
        foreach (ColumnDefinition cd in definitions)
        {
            String? colX = x != null && cd.columnIndex < x.Count ? x[cd.columnIndex] : null;
            String? colY = y != null && cd.columnIndex < y.Count ? y[cd.columnIndex] : null;

            int result = cd.comparer.Compare(colX, colY);
            if (result != 0)
                return result;
        }

        return 0;
    }
}