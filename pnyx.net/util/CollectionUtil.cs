using System.Collections;

namespace pnyx.net.util;

public static class CollectionUtil
{
    public static bool isEmpty(this ICollection? source)
    {
        return source == null || source.Count == 0;
    }

    public static bool hasAny(this ICollection? source)
    {
        return source != null && source.Count > 0;
    }
}