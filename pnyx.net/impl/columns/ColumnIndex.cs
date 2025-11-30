using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.impl.columns;

public class ColumnIndex
{
    private int index_;
        
    public int Index
    {
        get => index_;
        set
        {
            if (value < 0)
                throw new ArgumentException($"Invalid negative index={value}");
            index_ = value;
        }
    }

    public ColumnIndex()
    {
    }
        
    public ColumnIndex(int index)
    {
        Index = index;
    }

    public static implicit operator ColumnIndex(String text)
    {
        return new ColumnIndex(textAsIndex(text));
    }

    public static implicit operator ColumnIndex(char c)
    {
        return new ColumnIndex(charAsIndex(c));
    }

    public static implicit operator ColumnIndex(int index)
    {
        return new ColumnIndex(index);
    }

    public static implicit operator int(ColumnIndex index)
    {
        return index.Index;
    }

    public static int textAsIndex(String index)
    {
        if (!TextUtil.isAllLetters(index))
            throw new ArgumentException($"Invalid character index: [{index ?? "NULL"}]");

        index = index.ToLowerInvariant();
        int asNumber = 0;
        foreach (char current in index)
            asNumber = asNumber * 26 + current - 'a' + 1;

        return asNumber - 1;            // native indexing is zero based
    }

    public static int charAsIndex(char index)
    {
        if (!Char.IsLetter(index))
            throw new ArgumentException($"Invalid character index: [{index}]");

        index = Char.ToLowerInvariant(index);
        return index - 'a';            // native indexing is zero based
    }

    public override string ToString()
    {
        StringBuilder buffer = new();
        int val = index_;
        while (val > 0)
        {
            int c = 'A' + val % 26;
            buffer.Insert(0, (char)c);

            val = val / 26;
        }
        return base.ToString();
    }
    
    /// <summary>
    /// Converts 1-based integers into ColumnIndex objects 
    /// </summary>
    public static ColumnIndex[] convertColumnNumbersToIndex(params int[] columnNumbers)
    {
        ColumnIndex[] indexes = new ColumnIndex[columnNumbers.Length];
        for (int i = 0; i < columnNumbers.Length; i++)
        {
            int columnNumber = columnNumbers[i];
            if (columnNumber <= 0)
                throw new InvalidArgumentException("Invalid ColumnNumber {0}, ColumnNumbers start at 1", columnNumber);

            indexes[i] = columnNumber - 1;
        }

        return indexes;
    }

    public static HashSet<ColumnIndex> convertColumnNumbersToIndex(IEnumerable<int> columnNumbers)
    {
        HashSet<ColumnIndex> columnIndices = columnNumbers.Select(x => new ColumnIndex(x-1)).ToHashSet();
        return columnIndices;
    }

    protected bool Equals(ColumnIndex other)
    {
        return index_ == other.index_;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ColumnIndex)obj);
    }

    public override int GetHashCode()
    {
        return index_;
    }
}