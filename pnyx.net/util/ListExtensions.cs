using System;
using System.Collections.Generic;

namespace pnyx.net.util;

public static class ListExtensions
{
    /// <summary>
    /// Returns the first element, removes it from the list, then appends to the end
    /// </summary>
    /// <returns>First element</returns>
    /// <exception cref="ArgumentException">Throws an exception if the list is empty</exception>
    public static T rotate<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List is empty");

        T result = list[0];
        list.RemoveAt(0);
        list.Add(result);
        return result;
    }
    
    public static void addSorted<T>(this List<T> list, Comparison<T> comparison, T item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (comparison(list[i], item) > 0)
            {
                list.Insert(i, item);
                return;
            }
        }
        list.Add(item);
    }    
}