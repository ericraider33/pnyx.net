using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace pnyx.net.util;

public static class EnumUtil
{
    public static T stringToEnum<T>(String text, T defaultValue = default) where T : Enum
    {
        if (String.IsNullOrEmpty(text))
            return defaultValue;

        return (T)Enum.Parse(typeof(T), text, true);
    }

    public static T? stringToEnumNullable<T>(String text) where T : struct, Enum
    {
        if (String.IsNullOrEmpty(text) || TextUtil.isEqualsIgnoreCase(text, "null"))
            return default;

        try
        {
            return (T)Enum.Parse(typeof(T), text, true);
        }
        catch (ArgumentException)
        {
            return default;
        }
    }

    public static List<T> valuesAsList<T>() where T : Enum
    {
        T[] raw = (T[])Enum.GetValues(typeof(T));
        return raw.ToList();
    }

    public static List<T> valuesAsListIgnore<T>(params T[] toIgnore) where T : Enum
    {
        T[] raw = (T[])Enum.GetValues(typeof(T));
        List<T> result = raw.ToList();

        foreach (T ignore in toIgnore)
            result.Remove(ignore);
            
        return result;
    }

    public static Dictionary<String, T> toDictionary<T>(IEqualityComparer<String> comparer = null) where T : Enum
    {
        comparer = comparer ?? StringComparer.OrdinalIgnoreCase;            
        Dictionary<String, T> enumValues = new Dictionary<String, T>(comparer);
            
        Type enumType = typeof(T);
        foreach (var value in Enum.GetValues(enumType))
            enumValues.Add(Enum.GetName(enumType, value), (T) value);

        return enumValues;
    }

    public static String getLabel(this Enum val)
    {
        if (val == null)
            return null;
        
        Type type = val.GetType();
        string valAsName = type.GetEnumName(val);
        if (valAsName == null)
            return null;
            
        MemberInfo mi = type.GetMember(valAsName).FirstOrDefault();
        if (mi == null)
            return null;

        DescriptionAttribute da = mi.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
        if (da != null)
            return da.Description;

        return valAsName.camelToSpace();
    }

    public static T max<T>(params T[] source) where T : Enum
    {
        return max((IEnumerable<T>)source);
    }

    public static T min<T>(params T[] source) where T : Enum
    {
        return min((IEnumerable<T>) source);
    }
    
    public static T max<T>(IEnumerable<T> source) where T : Enum
    {
        if (source == null)
            return default(T);

        bool first = true;
        int m = 0;
        foreach (T a in source)
        {
            int aI = (int) Convert.ChangeType(a, TypeCode.Int32);

            if (first)
                m = aI;
            else
                m = Math.Max(aI, m);

            first = false;
        }

        if (first)
            return default(T);

        return (T) Enum.ToObject(typeof(T), m);
    }

    public static T min<T>(IEnumerable<T> source) where T : Enum
    {
        if (source == null)
            return default;

        bool first = true;
        int m = 0;
        foreach (T a in source)
        {
            int aI = (int) Convert.ChangeType(a, TypeCode.Int32);

            if (first)
                m = aI;
            else
                m = Math.Min(aI, m);

            first = false;
        }

        if (first)
            return default;

        return (T) Enum.ToObject(typeof(T), m);
    }
}