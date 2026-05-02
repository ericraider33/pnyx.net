using System;
using System.Collections.Generic;
using System.Linq;

namespace pnyx.net.util;

public static class ListUtil
{
    public static bool isEqualIgnoreOrder<T>(this List<T> list1, List<T> list2)
    {
        if (list1.Count != list2.Count)
            return false;

        return list1.All(list2.Contains) && list2.All(list1.Contains);
    }
    
    /// <summary>
    /// Adds an item to the list if it is not null. Passed list is modified when item is not null. Passed list is returned for chaining.
    /// </summary>      
    public static List<T> addNotNull<T>(this List<T> list, T? item) where T : class
    {
        if (item == null)
            return list;
        
        list.Add(item);
        return list;
    }

    public static T pop<T>(this List<T> list)
    {
        if (list.Count == 0)
            throw new InvalidOperationException("Cannot pop from an empty list.");

        T item = list[^1];
        list.RemoveAt(list.Count - 1);
        
        return item;
    }

    /// <summary>
    /// Returns object as a set
    /// </summary>
    public static HashSet<T> objectAsSet<T>(T value)
    {
        return new HashSet<T> { value };
    }
    
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
    /// Creates a new list with all items that are not null. 
    /// </summary>      
    public static List<T> whereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
    {
        return enumerable.Where(e => e != null).Cast<T>().ToList();
    }
    
    /// <summary>
    /// Creates a new list with all parameters that are not null. 
    /// </summary>      
    public static List<T> whereNotNull<T>(params T?[] args) where T : class
    {
        return args.Where(e => e != null).Cast<T>().ToList();
    }
}