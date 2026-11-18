using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace pnyx.net.util;

public static class CollectionUtil
{
    public static bool isEmpty<T>([NotNullWhen(false)] this ICollection<T>? source)
    {
        return source == null || source.Count == 0;
    }

    public static bool hasAny<T>(this ICollection<T>? source)
    {
        return source != null && source.Count > 0;
    }
    
    public static void addRange<T>(this ICollection<T> target, ICollection<T>? source)
    {
        if (source == null)
            return;

        foreach (T item in source)
            target.Add(item);
    }
}