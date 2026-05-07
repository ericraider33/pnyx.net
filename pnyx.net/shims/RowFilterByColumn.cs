using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.impl.columns;

namespace pnyx.net.shims;

/// <summary>
/// Filters an entire row based on a column, using a line filter. Use this in place of WithColumns, which
/// is more complicated and requires a RowFilter.
///
/// If row is missing specified column, row is filtered out.
/// </summary>
public class RowFilterByColumn : IRowFilter
{
    public ColumnIndex columnIndex { get; }
    public ILineFilter lineFilter { get; }

    public RowFilterByColumn(ColumnIndex columnIndex, ILineFilter lineFilter)
    {
        this.columnIndex = columnIndex;
        this.lineFilter = lineFilter;
    }

    public bool shouldKeepRow(List<string?> row)
    {
        if (columnIndex >= row.Count)
            return false;
        
        return lineFilter.shouldKeepLine(row[columnIndex] ?? "");
    }
}