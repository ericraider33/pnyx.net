using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pnyx.net.util
{
    public static class TextUtil
    {
        public static String concate(String delimiter, params String[] x)
        {
            if (x == null)
                return "";

            StringBuilder results = new StringBuilder();
            foreach (String toAdd in x)
            {
                String clean = toAdd == null ? "" : toAdd.Trim();
                if (clean.Length <= 0) 
                    continue;

                if (results.Length > 0)
                    results.Append(delimiter);

                results.Append(clean);
            }
            return results.ToString();
        }

        public static String emptyAsNull(this String value)
        {
            return String.IsNullOrEmpty(value) ? null : value;
        }

        public static String trimEmptyAsNull(this String value)
        {
            return String.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public static List<long> parseLongs(String value)
        {
            String[] values = value.Split(',');
            List<long> results = new List<long>(values.Length);
            results.AddRange(values.Select(long.Parse));
            return results;
        }

        public static List<int> parseInts(String value, char[] delimiters = null)
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

        public static List<decimal> parseDecimals(String value)
        {
            String[] values = value.Split(',');
            List<decimal> results = new List<decimal>(values.Length);
            results.AddRange(values.Select(decimal.Parse));
            return results;
        }

        public static List<double> parseDouble(String value)
        {
            String[] values = value.Split(',');
            List<double> results = new List<double>(values.Length);
            results.AddRange(values.Select(double.Parse));
            return results;
        }

        private static readonly Regex BOOLEAN_EXPRESSION_TRUE = new Regex("^(yes)|(true)$", RegexOptions.IgnoreCase);
        private static readonly Regex BOOLEAN_EXPRESSION_FALSE = new Regex("^(no)|(false)$", RegexOptions.IgnoreCase);
        
        public static bool? parseBoolNullable(String value)
        {
            if (BOOLEAN_EXPRESSION_TRUE.IsMatch(value)) return true;
            if (BOOLEAN_EXPRESSION_FALSE.IsMatch(value)) return false;
            return null;            
        }

        public static bool parseBool(String value, bool? defaultValue = null)
        {
            bool? result = parseBoolNullable(value);
            if (result.HasValue)
                return result.Value;
                   
            if (!defaultValue.HasValue)
                throw new ArgumentException(String.Format("String '{0}' can not be converted to boolean", value));

            return defaultValue.Value;
        }

        public static List<bool> parseBools(String value, bool? defaultValue = null)
        {
            String[] values = value.Split(',');
            List<bool> results = new List<bool>(values.Length);
            results.AddRange(values.Select(x => parseBool(x, defaultValue)));
            return results;
        }

        public static String padRight(String text, int length, char pad = ' ')
        {
            if (text.Length >= length)
                return text;

            StringBuilder result = new StringBuilder(length);
            result.Append(text);

            while (result.Length < length)
                result.Append(pad);

            return result.ToString();
        }

        public static String padLeft(String text, int length, char pad = ' ')
        {
            if (text.Length >= length)
                return text;

            StringBuilder result = new StringBuilder(length);
            result.Append(text);

            while (result.Length < length)
                result.Insert(0, pad);

            return result.ToString();
        }

        public static String multiple(String text, int count, String delimiter = " ")
        {
            delimiter = delimiter ?? "";

            StringBuilder result = new StringBuilder((text.Length + delimiter.Length) * count);
            for (int i = 0; i < count; i++)
                result.Append(text).Append(delimiter);
            result.Length -= delimiter.Length;
            return result.ToString();
        }

        public static bool isEqualsIgnoreCase(String a, String b)
        {
            if (a == null)
                return b == null;

            if (b == null)
                return false;
                
            return a.Equals(b, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool startsWithAny(String text, IEnumerable<String> toFind, bool ignoreCase = true)
        {
            if (ignoreCase)
                return toFind.Any(x => startsWithIgnoreCase(text, x));

            return toFind.Any(text.StartsWith);
        }

        private static readonly Regex NUMBER_ONLY_EXPRESSION = new Regex("^[\\d]+$");
        public static bool isAllNumbers(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return NUMBER_ONLY_EXPRESSION.IsMatch(text);
        }

        private static readonly Regex INTEGER_ONLY_EXPRESSION = new Regex("^[-+]?[\\d]+$");
        public static bool isInteger(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return INTEGER_ONLY_EXPRESSION.IsMatch(text);
        }
        
        private static readonly Regex DECIMAL_ONLY_EXPRESSION = new Regex("^[-+]?[\\d]+([.][\\d]*)?$");
        public static bool isDecimal(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return DECIMAL_ONLY_EXPRESSION.IsMatch(text);
        }

        private static readonly Regex CHARACTER_ONLY_EXPRESSION = new Regex("^[a-zA-Z]+$");
        public static bool isAllCharacters(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return CHARACTER_ONLY_EXPRESSION.IsMatch(text);
        }
        
        private static readonly Regex BOOLEAN_EXPRESSION = new Regex("^(yes)|(no)|(true)|(false)$", RegexOptions.IgnoreCase);
        public static bool isBoolean(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return BOOLEAN_EXPRESSION.IsMatch(text);            
        }

        private static readonly Regex CHARACTER_ANY_EXPRESSION = new Regex("[a-zA-Z]");
        public static bool hasAnyCharacters(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return CHARACTER_ANY_EXPRESSION.IsMatch(text);            
        }

        private static readonly Regex NUMBER_ANY_EXPRESSION = new Regex("[\\d]");
        public static bool hasAnyNumbers(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return NUMBER_ANY_EXPRESSION.IsMatch(text);
        }

        private static readonly Regex WHITE_SPACE_ANY_EXPRESSION = new Regex("[\\s]");
        public static bool hasAnyWhiteSpace(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return WHITE_SPACE_ANY_EXPRESSION.IsMatch(text);
        }

        public static int indexOfIgnoreCase(this String value, String toFind)
        {
            if (value == null || toFind == null)
                return -1;

            return value.IndexOf(toFind, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool containsIgnoreCase(this String value, String toFind)
        {
            if (value == null || toFind == null)
                return false;

            return value.IndexOf(toFind, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        public static bool startsWithIgnoreCase(this String value, String toFind)
        {
            if (value == null || toFind == null)
                return false;

            return value.IndexOf(toFind, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        public static bool endsWithIgnoreCase(this String value, String toFind)
        {
            if (value == null || toFind == null)
                return false;

            int index = value.LastIndexOf(toFind, StringComparison.CurrentCultureIgnoreCase);
            return index >= 0 && index == (value.Length - toFind.Length);
        }

        public static String findIgnoreCase(IEnumerable<String> values, String toMatch)
        {
            if (values == null || toMatch == null)
                return null;

            return values.FirstOrDefault(toCheck => toCheck.Equals(toMatch, StringComparison.CurrentCultureIgnoreCase));
        }

        public static int findIndexIgnoreCase(List<String> values, String toMatch)
        {
            if (toMatch == null)
                return -1;

            return values.FindIndex(toCheck => toCheck.Equals(toMatch, StringComparison.CurrentCultureIgnoreCase));
        }
        
        public static String toUpper(String x)
        {
            return x == null ? null : x.ToUpper();
        }

        public static String trim(String x)
        {
            return x == null ? null : x.Trim();
        }

        public static int length(String x)
        {
            return x == null ? 0 : x.Length;
        }

        public static bool isMixedCase(this String x)
        {
            if (x == null) return false;
            int upper = 0;
            int lower = 0;
            foreach (char c in x)
            {
                if (Char.IsLower(c)) lower++;
                else if (Char.IsUpper(c)) upper++;
            }
            return lower > 0 && upper > 0;
        }

        public static bool isUpperCase(this String x)
        {
            if (x == null) return false;
            int upper = 0;
            int lower = 0;
            foreach (char c in x)
            {
                if (Char.IsLower(c)) lower++;
                else if (Char.IsUpper(c)) upper++;
            }
            return lower == 0 && upper > 0;
        }

        public static String trunc(this String text, int maxLength)
        {
            if (text == null || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength);
        }

        public static String truncRight(this String text, int maxLength)
        {
            if (text == null || text.Length <= maxLength)
                return text;

            return text.Substring(text.Length-maxLength);
        }

        public static String truncAtWhitespace(String text, int maxLength)
        {
            if (text.Length <= maxLength)
                return text;

            StringBuilder result = new StringBuilder(text);
            for (int i = maxLength; i >= 0; i--)
            {
                char c = text[i];
                if (Char.IsWhiteSpace(c))
                {
                    result.Length = i;                          // truncates String
                    return result.ToString();
                }
            }

            result.Length = maxLength;                          // no white spaces present, just perform plain truncation
            return result.ToString();
        }

        public static String underscoreToCamel(String tableName)
        {
            StringBuilder result = new StringBuilder(tableName.Length);
            bool cap = true;
            for (int i = 0; i < tableName.Length; i++)
            {
                char c = tableName[i];
                if (c == '_')
                    cap = true;
                else
                {
                    result.Append(cap ? Char.ToUpper(c) : c);
                    cap = false;
                }
            }

            return result.ToString();
        }
       
        public static String camelToDash(Object val)
        {
            if (val == null)
                return null;

            String text = val.ToString();

            StringBuilder result = new StringBuilder(text.Length + 10);
            CamelCharType lastType = CamelCharType.Other;
            foreach (char c in text)
            {
                CamelCharType currentType = retrieveCamelCharType(c);
                if (lastType != CamelCharType.Other &&
                    currentType != lastType &&
                    (currentType == CamelCharType.UpperChar || currentType == CamelCharType.Number))
                {
                    result.Append('-');
                }

                result.Append(Char.ToLower(c));
                lastType = currentType;
            }

            return result.ToString();
        }

        public static String removeBeginning(String source, String beginning)
        {
            if (source == null || beginning == null || !source.StartsWith(beginning))
                return source;

            return source.Substring(beginning.Length);
        }

        public static String removeEnding(String source, String ending)
        {
            if (source == null || ending == null || !source.EndsWith(ending))
                return source;

            return source.Substring(0, source.Length - ending.Length);
        }

        public static String replaceEnding(String source, String oldEnding, String newEnding)
        {
            if (source == null || oldEnding == null || !source.EndsWith(oldEnding))
                return source;

            return source.Substring(0, source.Length - oldEnding.Length) + newEnding;
        }

        public static String camelToSpace(Object val)
        {
            if (val == null)
                return null;

            String text = val.ToString();
            
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

        private enum CamelCharType { LowerChar, UpperChar, Number, Other }
        private static CamelCharType retrieveCamelCharType(char c)
        {
            if (Char.IsDigit(c))
                return CamelCharType.Number;
            if (Char.IsLetter(c))
                return Char.IsUpper(c) ? CamelCharType.UpperChar : CamelCharType.LowerChar;
            return CamelCharType.Other;
        }

        public static String spaceToCamel(String text)
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

        public static List<String> textToList(String source, char delimiter = ',', bool trim = false)
        {
            if (source == null)
                return new List<String>(0);

            return source.Split(delimiter).Select(x => trim ? x.Trim() : x).ToList();
        }

        public static String textFromList(IEnumerable<String> source, char delimiter = ',')
        {
            if (source == null)
                return null;

            return emptyAsNull(String.Join(delimiter.ToString(), source));
        }

        public static String extractAlphaNumeric(String source)
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

        public static String extractAlpha(String source, bool preserveSpaces = false)
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

        public static String extractNumeric(String source)
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

        public static String extractNotWhitespace(String source)
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

        public static String extractNonGarbage(String source)
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

        private static readonly char[] SPLIT_SPACE_CHARS = new char[] {' ', '\t', '\n'};
        public static String[] splitSpace(String input)
        {
            if (String.IsNullOrEmpty(input))
                return new String[0];

            String[] words = input.Split(SPLIT_SPACE_CHARS, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        public static Tuple<String, String> splitAt(this String input, String token)
        {
            if (String.IsNullOrEmpty(input))
                return null;

            int index = input.IndexOf(token, StringComparison.Ordinal);
            if (index < 0)
                return new Tuple<String, String>(input, "");

            return new Tuple<String, String>(input.Substring(0, index), input.Substring(index+token.Length));
        }

        public static Tuple<String, String> splitAtIndex(this String input, int index)
        {
            if (String.IsNullOrEmpty(input) || index < 0)
                return null;

            if (input.Length < index)
                return new Tuple<String, String>(input, "");

            return new Tuple<String, String>(input.Substring(0, index), input.Substring(index));
        }

        public static String replaceFirst(this String input, String token, String replacement = null)
        {
            if (String.IsNullOrEmpty(input))
                return null;

            int index = input.IndexOf(token, StringComparison.Ordinal);
            if (index < 0)
                return input;
                        
            return String.Concat(input.Substring(0, index), replacement ?? "", input.Substring(index + token.Length));
        }

        public static List<String> asList(this String source)
        {
            List<String> result = new List<String>();
            if (source != null)
                result.Add(source);
            return result;
        }

        public static int? parseIntNullable(String source)
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

        public static double? parseDoubleNullable(String source)
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

        public static String trimQuotes(this String source)
        {
            if (String.IsNullOrEmpty(source))
                return source;

            int start = source[0] == '"' ? 1 : 0;
            int end = source[source.Length - 1] == '"' ? 1 : 0;
            return source.Substring(start, Math.Max(0, source.Length-start-end));
        }

        private static readonly Regex DATE_LIKE_A = new Regex("^[\\d]{1,2}[-][\\d]{1,2}$");
        // Kludge to avoid excel from destroying date like text
        public static String protectedFromExcel(String value)
        {
            if (value == null)
                return null;
            if (DATE_LIKE_A.IsMatch(value))
                return "'" + value;
            return value;
        }

        public static void removeIgnoreCase(this List<String> source, String toRemove)
        {
            int index = findIndexIgnoreCase(source, toRemove);
            if (index >= 0)
                source.RemoveAt(index);
        }

        public static void removeIgnoreCase(this List<String> source, IEnumerable<String> toRemoveEnum)
        {
            foreach (String toRemove in toRemoveEnum)
            {
                int index = findIndexIgnoreCase(source, toRemove);
                if (index >= 0)
                    source.RemoveAt(index);
            }
        }

        public static String removeParentheses(String input)
        {
            if (input == null)
                return input;

            int aIndex = input.IndexOf("(");
            while (aIndex >= 0)
            {
                int bIndex = input.IndexOf(")", aIndex);
                if (bIndex < 0)
                    return input;

                input = removeParts(input, aIndex, bIndex);
                aIndex = input.IndexOf("(");
            }

            return input;
        }

        public static String removeParts(String input, int startIndex, int endIndex)        // both indexes get removed
        {
            if (String.IsNullOrEmpty(input))
                return input;
            if (startIndex < 0 || startIndex >= input.Length)
                throw new ArgumentException("Invalid index " + startIndex);
            if (endIndex < startIndex || endIndex >= input.Length)
                throw new ArgumentException("Invalid index " + endIndex);

            StringBuilder result = new StringBuilder(input);
            result.Remove(startIndex, endIndex - startIndex + 1);

            return result.ToString();
        }

        public static Dictionary<char, int> countCharacters(String source)
        {
            Dictionary<char, int> counts = new Dictionary<char, int>();
            if (source == null)
                return counts;

            foreach (char c in source)
            {
                if (counts.ContainsKey(c))
                    counts[c]++;
                else
                    counts.Add(c, 1);
            }
            return counts;
        }

        public static String enocdeSqlValue(String source, bool quote = true, bool inculdePatternMatching = false, bool encodeNull = true)
        {
            if (encodeNull && source == null)
                return "null";

            source = source ?? "";

            StringBuilder result = new StringBuilder(source.Length * 2);
            if (quote)
                result.Append('\'');

            foreach (Char c in source)
            {
                switch (c)
                {
                    case '\0':  result.Append("\\0");   break;
                    case '\'':  result.Append("\\'");   break;
                    case '"':  result.Append("\\\"");   break;
                    case '\b':  result.Append("\\b");   break;
                    case '\n':  result.Append("\\n");   break;
                    case '\r':  result.Append("\\r");   break;
                    case '\t':  result.Append("\\t");   break;
                    case '\u001A': result.Append("\\z"); break;
                    case '\\':  result.Append("\\\\");   break;
                    case '%':   result.Append(inculdePatternMatching ? "\\%" : "%");   break;
                    case '_':  result.Append(inculdePatternMatching ? "\\_": "_");   break;
                    default:    result.Append(c);       break;
                }
            }
            if (quote)
                result.Append('\'');
            
            return result.ToString();
        }

    }
}