using System;
using System.Collections.Generic;
using System.Linq;

namespace pnyx.net.util.dropdown;

public class IntDropdownItem : DropdownItem<int>
{
    public IntDropdownItem(string text, int value) : base(text, value)
    {
    }

    public IntDropdownItem()
    {
    }
    
    public static IntDropdownItem? fromEnumNullable(Enum? value)
    {
        if (value == null)
            return null;
        
        return new IntDropdownItem(value.getLabel(), Convert.ToInt32(value));
    }

    public static IntDropdownItem fromEnum(Enum value)
    {
        return new IntDropdownItem(value.getLabel(), Convert.ToInt32(value));
    }
    
    public static List<IntDropdownItem> fromEnumList<TType>(IEnumerable<TType> value) where TType : Enum
    {
        return value.Select(v => fromEnum(v)).ToList();
    }    
}