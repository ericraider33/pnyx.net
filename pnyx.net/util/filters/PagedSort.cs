using System;

namespace pnyx.net.util.filters;

public class PagedSort<TType> where TType : struct, Enum 
{
    public TType column { get; set; } = default;
    public bool reverse { get; set; }
}