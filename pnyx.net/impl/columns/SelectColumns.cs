using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.columns;

public class SelectColumns : IRowTransformer
{
    public ColumnIndex[] columnIndices { get; }

    public SelectColumns(ColumnIndex[] columnIndices)
    {
        this.columnIndices = columnIndices;
    }

    public List<String> transformHeader(List<String> header)
    {
        return doTransform(header, "");
    }

    public List<String?> transformRow(List<String?> row)
    {
        return doTransform(row, "");
    }

    private List<T> doTransform<T>(List<T> row, T pad)
    {
        List<T> result = new List<T>(columnIndices.Length);
        foreach (int columnIndex in columnIndices)
        {
            T column = columnIndex < row.Count ? row[columnIndex] : pad;
            result.Add(column);
        }

        return result;
    }
}