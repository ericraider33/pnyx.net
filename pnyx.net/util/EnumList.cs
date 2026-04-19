using System;
using System.Collections.Generic;
using System.Text;

namespace pnyx.net.util;

public static class EnumList
{
    public static List<TType> fromString<TType>(string? text) where TType : struct, Enum
    {
        text = text.trimEmptyAsNull();
        if (text == null)
            return new List<TType>();

        string[] parts = text.Split(',');
        List<TType> result = new List<TType>(parts.Length);
        foreach (string part in parts)
        {
            TType? enumVal = EnumUtil.stringToEnumNullable<TType>(part);
            if (enumVal == null)
                continue;

            result.Add(enumVal.Value);
        }

        return result;
    }

    public static string? toString<TType>(List<TType>? source) where TType : struct
    {
        if (source == null || source.Count == 0)
            return null;

        StringBuilder builder = new();
        foreach (TType item in source)
        {
            builder.Append(item.ToString());
            builder.Append(',');                // adds a comma after each item so that searching in DB is easier
        }

        return builder.ToString();
    }
}