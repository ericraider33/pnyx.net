using System;
using System.Collections.Generic;
using System.Linq;

namespace pnyx.net.util
{
    public class EnumUtil
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

        public static IDictionary<string, T> toDictionary<T>(IEqualityComparer<String> comparer = null) where T : struct, IConvertible
        {
            comparer = comparer ?? StringComparer.OrdinalIgnoreCase;            
            Dictionary<string, T> enumValues = new Dictionary<string, T>(comparer);
            
            Type enumType = typeof(T);
            foreach (var value in Enum.GetValues(enumType))
                enumValues.Add(Enum.GetName(enumType, value), (T) value);

            return enumValues;
        }        
    }
}