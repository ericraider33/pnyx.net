using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace pnyx.net.util;

public static class EnumUtil
{
    public static T stringToEnum<T>(String text, T defaultValue = default(T)) where T : struct
    {
        if (String.IsNullOrEmpty(text))
            return defaultValue;

        return (T)Enum.Parse(typeof(T), text, true);
    }

    public static T? stringToEnumNullable<T>(String text) where T : struct
    {
        if (String.IsNullOrEmpty(text) || TextUtil.isEqualsIgnoreCase(text, "null"))
            return null;

        return (T)Enum.Parse(typeof(T), text, true);
    }

    public static List<T> valuesAsList<T>() where T : struct
    {
        T[] raw = (T[])Enum.GetValues(typeof(T));
        return raw.ToList();
    }

    public static List<T> valuesAsListIgnore<T>(params T[] toIgnore) where T : struct
    {
        T[] raw = (T[])Enum.GetValues(typeof(T));
        List<T> result = raw.ToList();

        foreach (T ignore in toIgnore)
            result.Remove(ignore);
            
        return result;
    }

    public static Dictionary<String, T> toDictionary<T>(IEqualityComparer<String> comparer = null) where T : struct, IConvertible
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
}