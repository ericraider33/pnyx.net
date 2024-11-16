using System;
using System.Collections.Generic;
using System.Linq;

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
    
    /// <summary>
    /// Adds an item to the list if it is not null. Passed list is modified when item is not null. Passed list is returned for chaining.
    /// </summary>      
    public static List<T> addNotNull<T>(this List<T> list, T item) where T : class
    {
        if (item != null)
            list.Add(item);
        return list;
    }
    
    /// <summary>
    /// Creates a new list with all items that are not null. 
    /// </summary>      
    public static List<T> whereNotNull<T>(this IEnumerable<T> enumerable) where T : class
    {
        return enumerable.Where(e => e != null).ToList();
    }
    
    /// <summary>
    /// Creates a new list with all parameters that are not null. 
    /// </summary>      
    public static List<T> whereNotNull<T>(params T[] args) where T : class
    {
        return args.Where(e => e != null).ToList();
    }
}