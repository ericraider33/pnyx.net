using System;

namespace pnyx.net.util.dropdown;

public class DropdownItem<TValue> : IComparable<DropdownItem<TValue>> where TValue : IComparable
{
    public string text { get; set; } = "";
    public TValue value { get; set; } = default!;

    public DropdownItem(string text, TValue value)
    {
        this.text = text;
        this.value = value;
    }

    public DropdownItem()
    {
    }

    public int CompareTo(DropdownItem<TValue>? other)
    {
        if (other == null)
            return 1;
                
        string text1 = text ?? "";
        string text2 = other.text ?? "";

        int diff = string.Compare(text1, text2, StringComparison.OrdinalIgnoreCase);
        if (diff != 0)
            return diff;

        return value.CompareTo(other.value);
    }
}