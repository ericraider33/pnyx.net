using System;

namespace pnyx.net.util.dropdown;

public class GuidDropdownItem : DropdownItem<Guid>
{
    public GuidDropdownItem(string text, Guid value) : base(text, value)
    {
    }

    public GuidDropdownItem()
    {
    }
}