using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pnyx.net.util;

public static class ParseExtensions
{
    public static Tuple<string, string>? splitAtNullable(this String? input, String token, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        return splitAt(input, token, comparisonType);
    }

    public static Tuple<string, string> splitAt(this String input, String token, StringComparison comparisonType = StringComparison.Ordinal)
    {
        int index = input.IndexOf(token, comparisonType);
        if (index < 0)
            return new Tuple<string, string>(input, "");

        return new Tuple<string, string>(input.Substring(0, index), input.Substring(index+token.Length));
    }

    public static Tuple<string, string>? splitAtIndexNullable(this String? input, int index)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        return splitAtIndex(input, index);
    }
    
    public static Tuple<string, string> splitAtIndex(this String input, int index)
    {
        if (index < 0)
            return new Tuple<string, string>("", input);

        if (input.Length < index)
            return new Tuple<string, string>(input, "");

        return new Tuple<string, string>(input.Substring(0, index), input.Substring(index));
    }

    public static Tuple<string, string>? splitAtLastNullable(this String? input, String token)
    {
        if (input == null)
            return null;

        return splitAtLast(input, token);
    }    
    
    public static Tuple<string, string> splitAtLast(this String input, String token)
    {
        int index = input.LastIndexOf(token, StringComparison.Ordinal);
        if (index < 0)
            return new Tuple<string, string>(input, "");

        return new Tuple<string, string>(input.Substring(0, index), input.Substring(index+token.Length));
    }    

    private static readonly char[] SPLIT_SPACE_CHARS = [' ', '\t', '\n'];
    public static String[] splitSpace(this String? input)
    {
        if (String.IsNullOrEmpty(input))
            return [];

        String[] words = input.Split(SPLIT_SPACE_CHARS, StringSplitOptions.RemoveEmptyEntries);
        return words;
    }

    public static int? parseIntNullable(this String? source)
    {
        if (String.IsNullOrEmpty(source))
            return null;

        try
        {
            return int.Parse(source);
        }
        catch
        {
            return null;
        }
    }

    public static double? parseDoubleNullable(this string? source)
    {
        if (String.IsNullOrEmpty(source))
            return null;

        try
        {
            return double.Parse(source);
        }
        catch
        {
            return null;
        }
    }
    
    public static List<long> parseLongs(this string? value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return [];
        
        String[] values = value.Split(',');
        List<long> results = new List<long>(values.Length);
        results.AddRange(values.Select(long.Parse));
        return results;
    }

    public static List<int> parseInts(this String? value, char[]? delimiters = null)
    {
        List<int> results = new List<int>();

        delimiters = delimiters ?? new[] { ',' };
        if (String.IsNullOrWhiteSpace(value))
            return results;

        String[] values = value.Split(delimiters);
        foreach (String val in values)
        {
            if (String.IsNullOrWhiteSpace(val))
                continue;

            results.Add(int.Parse(val));
        }
        return results;
    }

    public static List<decimal> parseDecimals(this String? value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return [];
        
        String[] values = value.Split(',');
        List<decimal> results = new List<decimal>(values.Length);
        results.AddRange(values.Select(decimal.Parse));
        return results;
    }

    public static List<double> parseDouble(this String? value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return [];
        
        String[] values = value.Split(',');
        List<double> results = new List<double>(values.Length);
        results.AddRange(values.Select(double.Parse));
        return results;
    }

    private static readonly Regex BOOLEAN_EXPRESSION_TRUE = new Regex("^(yes)|(true)$", RegexOptions.IgnoreCase);
    private static readonly Regex BOOLEAN_EXPRESSION_FALSE = new Regex("^(no)|(false)$", RegexOptions.IgnoreCase);
    
    public static bool? parseBoolNullable(this String? value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return null;
        
        if (BOOLEAN_EXPRESSION_TRUE.IsMatch(value)) return true;
        if (BOOLEAN_EXPRESSION_FALSE.IsMatch(value)) return false;
        return null;            
    }

    public static bool parseBool(this String? value, bool? defaultValue = null)
    {
        bool? result = parseBoolNullable(value);
        if (result.HasValue)
            return result.Value;
               
        if (!defaultValue.HasValue)
            throw new ArgumentException($"String '{value}' can not be converted to boolean");

        return defaultValue.Value;
    }

    public static List<bool> parseBools(this String? value, bool? defaultValue = null)
    {
        if (String.IsNullOrWhiteSpace(value))
            return [];
        
        String[] values = value.Split(',');
        List<bool> results = new List<bool>(values.Length);
        results.AddRange(values.Select(x => parseBool(x, defaultValue)));
        return results;
    }

    public static String extractAlphaNumeric(String? source)
    {
        if (source == null)
            return "";

        StringBuilder result = new StringBuilder(source.Length);
        foreach (Char c in source)
        {
            if (Char.IsLetterOrDigit(c))
                result.Append(c);
        }
        return result.ToString();
    }
    
    public static String extractAlphaNumericDash(String? source)
    {
        if (source == null)
            return "";

        StringBuilder result = new StringBuilder(source.Length);
        foreach (Char c in source)
        {
            if (Char.IsLetterOrDigit(c) || c == '-' || c == '_')
                result.Append(c);
        }
        return result.ToString();
    }

    public static String extractAlpha(String? source, bool preserveSpaces = false)
    {
        if (source == null)
            return "";

        StringBuilder result = new StringBuilder(source.Length);
        foreach (Char c in source)
        {
            if (Char.IsLetter(c))
                result.Append(c);
            else if (preserveSpaces && c == ' ')
                result.Append(c);
        }
        return result.ToString();
    }

    public static String extractNumeric(String? source)
    {
        if (source == null)
            return "";

        StringBuilder result = new StringBuilder(source.Length);
        foreach (Char c in source)
        {
            if (Char.IsNumber(c))
                result.Append(c);
        }
        return result.ToString();
    }

    public static String extractNotWhitespace(String? source)
    {
        if (source == null)
            return "";

        StringBuilder result = new StringBuilder(source.Length);
        foreach (Char c in source)
        {
            if (!Char.IsWhiteSpace(c))
                result.Append(c);
        }
        return result.ToString();
    }

    public static String extractNonGarbage(String? source)
    {
        if (source == null)
            return "";

        StringBuilder result = new StringBuilder(source.Length);
        foreach (Char c in source)
        {
            if (Char.IsLetter(c) || Char.IsNumber(c) || Char.IsPunctuation(c) || Char.IsWhiteSpace(c))
                result.Append(c);
        }
        return result.ToString();
    }
}