using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pnyx.net.util;

namespace pnyx.net.impl.columns;

public class Row : IList<String>
{
    private readonly IList<String> values;

    public Row()
    {
        values = new List<string>();
    }
    
    public Row(IEnumerable<String> source)
    {
        values = source.ToList();
    }

    public Row(params String[] values)
    {
        this.values = values;
        if (values == null)
            throw new NullReferenceException("Can't wrap a NULL array");

        for (int i = 0; i < values.Length; i++)
            values[i] = values[i].trimEmptyAsNull();
    }

    public Row(List<String> values)
    {
        this.values = values;
        if (values == null)
            throw new NullReferenceException("Can't wrap a NULL list");

        for (int i = 0; i < values.Count; i++)
            values[i] = values[i].trimEmptyAsNull();
    }

    public static explicit operator Row(String[] source)
    {
        return new Row(source);
    }

    public static explicit operator Row(List<String> source)
    {
        return new Row(source);
    }

    public String this[int index]
    {
        get => values[index];
        set => values[index] = value;
    }

    public String this[String index]
    {
        get => values[ColumnIndex.textAsIndex(index)];
        set => values[ColumnIndex.textAsIndex(index)] = value;
    }

    public String this[char index]
    {
        get => values[ColumnIndex.charAsIndex(index)];
        set => values[ColumnIndex.charAsIndex(index)] = value;
    }

    public String this[ColumnIndex index]
    {
        get => values[index.Index];
        set => values[index.Index] = value;
    }

    public int Count => values.Count;

    public bool BlankLine
    {
        get { return values.All(x => x == null); }
    }

    public int BlankCount
    {
        get { return values.Count(x => x == null); }
    }

    public int DataCount
    {
        get { return values.Count(x => x != null); }
    }

    public int dataCountForIndexes(params String[] indexes)
    {
        int sum = 0;
        foreach (String index in indexes)
        {
            int indexInt = ColumnIndex.textAsIndex(index);
            if (indexInt < values.Count && values[indexInt] != null)
                sum++;
        }
        return sum;
    }

    public int dataCountForIndexes(params int[] indexes)
    {
        int sum = 0;
        foreach (int indexInt in indexes)
        {
            if (indexInt < values.Count && values[indexInt] != null)
                sum++;
        }
        return sum;
    }

    public int dataCountForIndexes(params char[] indexes)
    {
        int sum = 0;
        foreach (char index in indexes)
        {
            int indexInt = ColumnIndex.textAsIndex(index.ToString());
            if (indexInt < values.Count && values[indexInt] != null)
                sum++;
        }
        return sum;
    }

    public bool hasIndex(String index)
    {
        return dataCountForIndexes(index) == 1;
    }

    public bool hasIndex(int index)
    {
        return dataCountForIndexes(index) == 1;
    }

    public bool hasIndex(char index)
    {
        return dataCountForIndexes(index) == 1;
    }
    
    public bool verifyCountForIndexes(int count, params String[] indexes)
    {
        return DataCount == count && dataCountForIndexes(indexes) == count;
    }

    public bool verifyCountForIndexes(int count, params int[] indexes)
    {
        return DataCount == count && dataCountForIndexes(indexes) == count;
    }

    public bool verifyCountForIndexes(int count, params char[] indexes)
    {
        return DataCount == count && dataCountForIndexes(indexes) == count;
    }

    public override string ToString()
    {
        return toCsvLine(values);
    }

    public void pad(int desiredSize)
    {
        while (values.Count < desiredSize)
            Add(null);
    }

    public void pad(char desiredIndex)
    {
        pad(ColumnIndex.charAsIndex(desiredIndex) + 1);
    }

    public void pad(String desiredIndex)
    {
        pad(ColumnIndex.textAsIndex(desiredIndex) + 1);
    }
    
    public void updateCells(Func<String, String> func)
    {
        for (int i = 0; i < Count; i++)
        {
            String original = this[i];
            String modified = func(original);
            this[i] = modified;
        }
    }

    public static string toCsvLine(params String[] text)
    {
        return toCsvLine((IEnumerable<String>)text);
    }

    public static string toCsvLine(IEnumerable<String> source)
    {
        if (source == null)
            return null;
        
        StringBuilder builder = new StringBuilder();
        toCsvBuilder(builder, source);
        return builder.ToString();
    }

    public static void toCsvBuilder(StringBuilder builder, IEnumerable<String> source)
    {
        bool first = true;
        foreach (String val in source)
        {
            if (!first)
                builder.Append(",");
            first = false;

            if (val == null)
                continue;

            if (val.Contains(",") || val.Contains("\"") || val.Contains("\n") || val.Contains("\\"))
            {
                builder.Append("\"");
                foreach (char c in val)
                {
                    switch (c)
                    {
                        case '"': builder.Append('\\').Append('"'); break;
                        case '\\': builder.Append('\\').Append('\\'); break;
                        case '\n': builder.Append('\\').Append('n'); break;
                        default: builder.Append(c); break;
                    }
                }
                builder.Append("\"");
            }
            else
                builder.Append(val);
        }
    }

    //
    // IList methods
    //

    public int IndexOf(string item)
    {
        item = item.emptyAsNull();
        return values.IndexOf(item);
    }

    public void Insert(int index, string item)
    {
        item = item.emptyAsNull();
        values.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        values.RemoveAt(index);
    }

    public void Add(string item)
    {
        item = item.emptyAsNull();
        values.Add(item);
    }

    public void Clear()
    {
        values.Clear();
    }

    public bool Contains(string item)
    {
        item = item.emptyAsNull();
        return values.Contains(item);
    }

    public void CopyTo(string[] array, int arrayIndex)
    {
        values.CopyTo(array, arrayIndex);
    }

    public bool Remove(string item)
    {
        item = item.emptyAsNull();
        return values.Remove(item);
    }

    public bool IsReadOnly => values.IsReadOnly;

    public IEnumerator<string> GetEnumerator()
    {
        return values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return values.GetEnumerator();
    }
}