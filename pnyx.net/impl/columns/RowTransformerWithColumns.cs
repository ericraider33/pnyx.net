using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.columns;

public class RowTransformerWithColumns : IRowTransformer
{
    public ColumnIndex[] indexes { get; }   
    public IRowTransformer rowTransformer  { get; }

    public RowTransformerWithColumns(ColumnIndex[] indexes, IRowTransformer rowTransformer)
    {
        this.indexes = indexes;
        this.rowTransformer = rowTransformer;            
    }

    public List<String>? transformHeader(List<String> header)
    {
        List<String> subColumns = subColumnsIn(header, "");
        List<String>? transformed = rowTransformer.transformHeader(subColumns);
        if (transformed == null)
            return null;
        
        return subColumnsOut(transformed, header, "");
    }

    public List<String?>? transformRow(List<String?> row)
    {
        List<String?> subColumns = subColumnsIn(row, "");
        List<String?>? transformed = rowTransformer.transformRow(subColumns);
        if (transformed == null)
            return null;
        
        return subColumnsOut(transformed, row, "");
    }

    private List<T> subColumnsIn<T>(List<T> row, T pad)
    {
        List<T> subColumns = new List<T>(indexes.Length);
        foreach (int columnIndex in indexes)
        {
            T column = columnIndex < row.Count ? row[columnIndex] : pad;
            subColumns.Add(column);
        }
        return subColumns;
    }

    private List<T> subColumnsOut<T>(List<T> transformed, List<T> row, T pad)
    {
        for (int i = 0; i < indexes.Length; i++)
        {
            int columnIndex = indexes[i];
            if (columnIndex >= row.Count)
                continue;

            if (transformed == null)
                row[columnIndex] = pad;
            else
                row[columnIndex] = transformed[i];
        }

        return row;
    }
}