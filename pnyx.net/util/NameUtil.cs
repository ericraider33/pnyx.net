using System;
using System.Collections.Generic;
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
            "dvm",  "edd",  "esq",  "ii",   "iii",  "iv", "vi", "vii", "viii", "ix", "inc",  "jd",   "jr",   "lld",  "ltd",
            "md",   "od",   "osb",  "pc",   "pe",   "phd",  "rev", "ret",  "rgs",  "rn",   "rnc",  "shcj",
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

        // Removes special characters from String.    Allows letters, spaces, hyphens and apostrophes
        public static String extractNameString(this String str)
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

            // Processes String up to last char
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

        public static String toTitleCase(this String str)
        {
            if (str == null) return null;
            TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }

        public static String asName(this String str)
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

            return new Tuple<String, String, String>(firstName, middleName, lastName);
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

            return new Tuple<String, String, String>(firstName, middleName, lastName);
        }

        public static Name parseFullName(String name)
        {
            name = name.asName();
            if (String.IsNullOrWhiteSpace(name))
                return null;

            Name result = new Name();
            List<String> nameParts = name.Split(' ').Select(x => x.asName()).Where(x => x != null).ToList();

            // Parses known suffix
            if (nameParts.Count > 2)
            {
                result.suffix = nameParts.FirstOrDefault(x => TextUtil.findIgnoreCase(SUFFIX, x) != null);
                if (result.suffix != null)
                    nameParts.RemoveAll(x => x == result.suffix);
            }
            
            // Looks for a middle initial
            if (nameParts.Count >= 4 && nameParts[2].Length == 1)
            {
                // Sets first name using first 2 parts
                result.firstName = String.Concat(nameParts[0], " ", nameParts[1]);
                nameParts.RemoveAt(0);                
                nameParts.RemoveAt(0);                
            }
            else
            {
                // Sets first name
                result.firstName = nameParts[0];
                nameParts.RemoveAt(0);
            }

            // Sets middle name
            if (nameParts.Count > 1)
            {
                result.middleName = nameParts[0];
                nameParts.RemoveAt(0);                
            }

            result.lastName = String.Join(" ", nameParts);
            
            return result;
        }

        public static Tuple<String,String> parseSuffix(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return new Tuple<String, String>(name, null);

            name = name.asName();
            
            List<String> nameParts = new List<String>();
            String suffix = null;
            foreach (String part in name.Split(' '))
            {
                if (suffix == null)
                {
                    suffix = TextUtil.findIgnoreCase(SUFFIX, part);
                    if (suffix == null)
                        nameParts.Add(part);
                }
                else
                    nameParts.Add(part);                    
            }

            name = String.Join(" ", nameParts);
            return new Tuple<String, String>(name, suffix);
        }
    }
}