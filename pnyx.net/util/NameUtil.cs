using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace pnyx.net.util
{
    public static class NameUtil
    {
        private static readonly String[] SUFFIX =
        {
            "bvm",  "cfre", "clu",  "cpa",  "csc",  "csj",  "dc",   "dd",   "dds",  "dmd",  "do",
            "dvm",  "edd",  "esq",  "ii",   "iii",  "iv",   "inc",  "jd",   "jr",   "lld",  "ltd",
            "md",   "od",   "osb",  "pc",   "pe",   "phd",  "ret",  "rgs",  "rn",   "rnc",  "shcj",
            "sj",   "snjm", "sr",   "ssmo", "usa",  "usaf", "usafr",    "usar", "uscg", "usmc", "usmcr",
            "usn",  "usnr"
        };

        private static readonly Regex NAME_EXPRESSION = new Regex("^[a-zA-Z'-]+(\\s+[a-zA-Z'-]+)*$");
        // NOTE: only checks that value looks like a name, but still good to run through method "extractNameString"
        public static bool isName(String text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            return NAME_EXPRESSION.IsMatch(text);
        }

        // Removes special characters from string.    Allows letters, spaces, hyphens and apostrophes
        public static string extractNameString(this string str)
        {
            if (str == null) return null;

            StringBuilder sb = new StringBuilder();

            // Finds last good character, ignoring trailing format
            int max = str.Length-1;
            while (max >= 0)
            {
                char c = str[max];
                if (isNameChar(c) && !isNameFormatChar(c))
                    break;
                max--;
            }

            // Processes string up to last char
            bool previousFormat = false;
            for (int i = 0; i <= max; i++)
            {
                char c = str[i];
                if (isNameFormatChar(c))
                {
                    if (!previousFormat && sb.Length > 0)
                        sb.Append(c);
                    previousFormat = true;
                }
                else if (isNameChar(c))
                {
                    previousFormat = false;
                    sb.Append(c);
                }
            }

            return sb.Length == 0 ? null : sb.ToString();
        }

        private static bool isNameChar(char c)
        {
            return c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || isNameFormatChar(c);
        }

        private static bool isNameFormatChar(char c)
        {
            return c == ' ' || c == '-' || c == '\'';
        }

        public static String toTitleCase(this string str)
        {
            if (str == null) return null;
            TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }

        public static String asName(this string str)
        {
            str = extractNameString(str);           // removes any invalid characters

            if (!TextUtil.isMixedCase(str))
                str = toTitleCase(str);

            return str;
        }

        public static Tuple<String, String, String> lastFirstMiddleName(String name)
        {
            if (name == null || !name.Contains(","))
                return null;

            String[] nameParts = name.Split(',').Select(x => x.asName()).Where(x => x != null).ToArray();
            if (nameParts.Length != 2)
                return null;

            String lastName = nameParts[0];
            nameParts = nameParts[1].Split(' ');
            
            // Parses known suffix
            String suffix = nameParts.FirstOrDefault(x => TextUtil.findIgnoreCase(SUFFIX, x) != null);
            if (suffix != null && nameParts.Length > 1)
            {
                nameParts = nameParts.Where(x => x != suffix).ToArray();
                lastName = String.Concat(lastName, " ", suffix);
            }

            if (nameParts.Length == 0)
                return null;

            String firstName = String.Join(" ", nameParts.Take(Math.Max(1, nameParts.Length - 1)));
            String middleName = nameParts.Length == 1 ? null : nameParts.Last().Replace(".", "");

            return new Tuple<string, string, string>(firstName, middleName, lastName);
        }

        public static Tuple<String, String, String> lastMiddleFirstName(String name)
        {
            if (name == null || !name.Contains(","))
                return null;

            String[] nameParts = name.Split(',').Select(x => x.asName()).Where(x => x != null).ToArray();
            if (nameParts.Length != 2)
                return null;

            String lastName = nameParts[0];
            nameParts = nameParts[1].Split(' ');

            // Parses known suffix
            String suffix = nameParts.FirstOrDefault(x => TextUtil.findIgnoreCase(SUFFIX, x) != null);
            if (suffix != null && nameParts.Length > 1)
            {
                nameParts = nameParts.Where(x => x != suffix).ToArray();
                lastName = String.Concat(lastName, " ", suffix);
            }

            if (nameParts.Length == 0)
                return null;

            String middleName = nameParts.Length >= 2 ? nameParts[0] : null;
            String firstName = String.Join(" ", nameParts.Skip(middleName != null ? 1 : 0));

            return new Tuple<string, string, string>(firstName, middleName, lastName);
        }

        public static Tuple<String,String,String> parseFullName(String name)
        {
            name = name.asName();
            if (String.IsNullOrWhiteSpace(name))
                return null;

            String[] nameParts = name.Split(' ').Select(x => x.asName()).Where(x => x != null).ToArray();

            // Parses known suffix
            String suffix = null;
            if (nameParts.Length > 2)
            {
                suffix = nameParts.FirstOrDefault(x => TextUtil.findIgnoreCase(SUFFIX, x) != null);
                if (suffix != null)
                    nameParts = nameParts.Where(x => x != suffix).ToArray();
            }

            if (nameParts.Length == 1)
                return new Tuple<string, string, string>(nameParts[0], null, suffix);

            String firstName = nameParts[0];
            String middleName = nameParts.Length == 2 ? null : nameParts[1];
            String lastName = nameParts.Length == 2 ? nameParts[1] : String.Join(" ", nameParts.Skip(2));

            if (suffix != null)
                lastName = String.Concat(lastName, " ", suffix);

            return new Tuple<string, string, string>(firstName, middleName, lastName);
        }

        public static String parseSuffix(String text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return null;

            text = TextUtil.extractAlpha(text);
            return TextUtil.findIgnoreCase(SUFFIX, text) != null ? text : null;
        }
    }
}