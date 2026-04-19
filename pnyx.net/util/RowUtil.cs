using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.impl.columns;

namespace pnyx.net.util;

public static class RowUtil
{
    public static List<String?> replaceColumn(List<String?> row, ColumnIndex columnIndex, params String[] replacement)
    {
        if (columnIndex > row.Count)
            return row;
            
        if (columnIndex >= 0 && columnIndex < row.Count)
            row.RemoveAt(columnIndex);
            
        for (int i = 0; i < replacement.Length; i++)
            row.Insert(columnIndex+i, replacement[i] ?? "");
                        
        return row;
    }
        
    public static List<String?> insertColumns(List<String?> row, ColumnIndex columnIndex, params String[] replacement)
    {
        if (columnIndex > row.Count)
            return row;
            
        for (int i = 0; i < replacement.Length; i++)
            row.Insert(columnIndex+i, replacement[i] ?? "");
                        
        return row;
    }

    public static List<string> insertBlankColumnsHeader(List<string> row, ISet<ColumnIndex> columnIndices, string pad = "")
    {
        return insertBlankColumns_(row, columnIndices, pad);
    }

    public static List<string?> insertBlankColumns(List<string?> row, ISet<ColumnIndex> columnIndices, string pad = "")
    {
        return insertBlankColumns_(row, columnIndices, pad);
    }

    private static List<T> insertBlankColumns_<T>(List<T> row, ISet<ColumnIndex> columnIndices, T pad)
    {
        int finalSize = row.Count + columnIndices.Count;
        List<T> result = new List<T>(finalSize);
        for (int source = 0; source < row.Count; source++)
        {
            if (columnIndices.Contains(source))
                result.Add(pad);

            if (result.Count < finalSize)
                result.Add(row[source]);
        }
        if (columnIndices.Contains(row.Count))
            result.Add(pad);

        return result;
    }

    public static List<String> duplicateColumnsHeader(List<String> row, ISet<ColumnIndex> columnIndices)
    {
        return duplicateColumns_(row, columnIndices);
    }

    public static List<String?> duplicateColumns(List<String?> row, ISet<ColumnIndex> columnIndices)
    {
        return duplicateColumns_(row, columnIndices);
    }
    
    private static List<T> duplicateColumns_<T>(List<T> row, ISet<ColumnIndex> columnIndices)
    {
        int finalSize = row.Count + columnIndices.Count;
        List<T> result = new List<T>(finalSize);
        for (int source = 0; source < row.Count; source++)
        {
            if (columnIndices.Contains(source))
                result.Add(row[source]);

            if (result.Count < finalSize)
                result.Add(row[source]);
        }

        return result;
    }

    public static List<String?> removeColumns(List<String?> row, ISet<ColumnIndex> columnIndices)
    {
        List<String?> result = new List<String?>(row.Count);
        for (int i = 0; i < row.Count; i++)
        {
            if (columnIndices.Contains(new ColumnIndex(i)))
                continue;

            result.Add(row[i]);
        }

        return result;
    }

    public static List<String?> fixWidth(List<String?> row, int columns, String pad = "")
    {
        List<String?> result = new List<String?>(columns);

        for (int i = 0; i < columns; i++)
        {
            if (i < row.Count)
                result.Add(row[i]);
            else
                result.Add(pad);
        }
                        
        return result;
    }

    public static bool isEqual(List<String?>? rowA, List<String?>? rowB)
    {
        if (rowA == null && rowB == null)
            return true;
        else if (rowA == null || rowB == null)
            return false;
        else
            return rowA.SequenceEqual(rowB);
    }

    public static List<String> setDefaultHeaderNames(List<String> header)
    {
        for (int i = 0; i < header.Count; i++)
        {
            if (String.IsNullOrEmpty(header[i]))
                header[i] = "Header" + (i + 1);
        }

        return header;
    }

    public static List<String>? asRow(this String[]? source)
    {
        if (source == null)
            return null;

        return new List<String>(source);
    }
    
    public static List<String?> toRow(this List<String> source)
    {
        return source.Cast<string?>().ToList();
    }
    
    public static List<String> toHeader(this List<String?> source)
    {
        return source.Select(x => x ?? "").ToList();
    }
}