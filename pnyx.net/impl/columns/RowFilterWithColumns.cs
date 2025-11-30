using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.columns;

public class RowFilterWithColumns : IRowFilter
{
    public ColumnIndex[] indexes { get; private set; }   
    public IRowFilter rowFilter  { get; private set; }
        
    public RowFilterWithColumns(ColumnIndex[] indexes, IRowFilter rowFilter)
    {
        this.indexes = indexes;
        this.rowFilter = rowFilter;
    }

    public bool shouldKeepRow(List<String> row)
    {
        List<String> subColumns = new List<String>(indexes.Length);
        foreach (int columnIndex in indexes)
        {
            String column = columnIndex < row.Count ? row[columnIndex] : "";
            subColumns.Add(column);
        }

        return rowFilter.shouldKeepRow(subColumns);
    }
}