using System;
using System.Text;

namespace pnyx.net.util;

public static class CasingExtensions
{
    public static String kebobToCamel(this String name)
    {
        return toCamel(name, x: '-');
    }

    public static String snakeToCamel(this String name)
    {
        return toCamel(name, x: '_');
    }

    public static String toCamel(this String name, char? x = null)
    {
        if (name == null)
            return null;
        
        StringBuilder result = new StringBuilder(name.Length);
        bool cap = true;
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            bool match;
            if (x.HasValue)
                match = c == x.Value;
            else
                match = c == '_' || c == '-';
                    
            if (match)
                cap = true;
            else
            {
                result.Append(cap ? Char.ToUpper(c) : c);
                cap = false;
            }
        }

        return result.ToString();
    }

    public static String camelToKebob(this String name)
    {
        return fromCamel(name, '-');
    }

    public static String camelToSnake(this String name)
    {
        return fromCamel(name, '_');
    }
    
    public static String fromCamel(this String text, char dash = '-')
    {
        if (text == null)
            return null;

        StringBuilder result = new StringBuilder(text.Length + 10);
        CamelCharType lastType = CamelCharType.Other;
        foreach (char c in text)
        {
            CamelCharType currentType = retrieveCamelCharType(c);
            if (lastType != CamelCharType.Other &&
                currentType != lastType &&
                (currentType == CamelCharType.UpperChar || currentType == CamelCharType.Number))
            {
                result.Append(dash);
            }

            result.Append(Char.ToLower(c));
            lastType = currentType;
        }

        return result.ToString();
    }

    private enum CamelCharType { LowerChar, UpperChar, Number, Other }
    private static CamelCharType retrieveCamelCharType(char c)
    {
        if (Char.IsDigit(c))
            return CamelCharType.Number;
        if (Char.IsLetter(c))
            return Char.IsUpper(c) ? CamelCharType.UpperChar : CamelCharType.LowerChar;
        return CamelCharType.Other;
    }

    public static String camelToSpace(this String text)
    {
        if (text == null)
            return null;
            
        StringBuilder result = new StringBuilder(text.Length + 10);
        CamelCharType lastType = CamelCharType.Other;
        foreach (char c in text)
        {
            CamelCharType currentType = retrieveCamelCharType(c);
            if (lastType != CamelCharType.Other &&
                currentType != lastType &&
                (currentType == CamelCharType.UpperChar || currentType == CamelCharType.Number))
            {
                result.Append(' ');
            }

            result.Append(c);
            lastType = currentType;
        }

        result = result.Replace(" Or ", " or ");
        result = result.Replace(" And ", " and ");

        return result.ToString();
    }

    public static String spaceToCamel(this String text)
    {
        if (text == null)
            return null;

        StringBuilder result = new StringBuilder(text.Length);
        bool space = false;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == ' ')
                space = true;
            else if (space)
            {
                result.Append(Char.ToUpper(c));
                space = false;
            }
            else
                result.Append(c);
        }

        return result.ToString();
    }
}