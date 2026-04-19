using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.columns;

public class SelectColumns : IRowTransformer
{
    public ColumnIndex[] columnIndices { get; set; }

    public SelectColumns(ColumnIndex[] columnIndices)
    {
        this.columnIndices = columnIndices;
    }

    public List<String> transformHeader(List<String> header)
    {
        return transformRow(header);
    }

    public List<String> transformRow(List<String> row)
    {
        List<String> result = new List<String>(columnIndices.Length);
        foreach (int columnIndex in columnIndices)
        {
            String column = columnIndex < row.Count ? row[columnIndex] : "";
            result.Add(column);
        }

        return result;
    }
}