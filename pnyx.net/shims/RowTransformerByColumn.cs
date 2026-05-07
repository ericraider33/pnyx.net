using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;
using pnyx.net.impl.columns;
using pnyx.net.util;

namespace pnyx.net.shims;

/// <summary>
/// Transforms a specific column, using a line transform. Use this in place of WithColumns, which
/// is more complicated and requires a RowTransformer.
///
/// If row is missing specified column, row remains unchanged. If line transformer returns null, row is filtered out.
/// </summary>
public class RowTransformerByColumn : IRowTransformer
{        
    public ColumnIndex columnIndex { get; }
    public ILineTransformer lineTransformer { get; }
    public bool treatHeaderAsRow { get; }

    public RowTransformerByColumn(ColumnIndex columnIndex, ILineTransformer lineTransformer, bool treatHeaderAsRow = false)
    {
        this.treatHeaderAsRow = treatHeaderAsRow;
        this.columnIndex = columnIndex;
        this.lineTransformer = lineTransformer;
    }

    public List<String>? transformHeader(List<String> header)
    {
        if (!treatHeaderAsRow)
            return header;

        List<string?> asRow = header.Cast<string?>().ToList();
        List<string?>? result = doTransform(asRow);
        if (result == null)
            return null;

        return result.toHeader();
    }

    public List<String?>? transformRow(List<String?> row)
    {
        return doTransform(row);
    }

    private List<String?>? doTransform(List<String?> row)
    {
        if (columnIndex >= row.Count)
            return row;

        string columnAsLine = row[columnIndex] ?? "";
        string? transformedColumn = lineTransformer.transformLine(columnAsLine);
        if (transformedColumn == null)
            return null;

        List<string?> result = row.ToList();
        result[columnIndex] = transformedColumn;
        return result;
    }
}