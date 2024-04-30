using System;
using System.Text;
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
}