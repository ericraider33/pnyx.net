using System;
using System.Text.RegularExpressions;

namespace pnyx.net.util
{
    // https://www.zip-codes.com/learn-about/why-are-there-3-and-4-digit-zip-codes-in-the-united-states.asp
    public static class ZipCodeUtil
    {
        private static readonly Regex ZERO_PAD_EXPRESSION = new Regex("^[\\d]{3,5}[-]?([\\d]{4})?$");
        private static readonly Regex ZIP_CODE_EXPRESSION = new Regex("^[\\d]{5}[-]?([\\d]{4})?$");

        public static String parseZipCode(String source, bool zeroPad = false)
        {
            source = ParseExtensions.extractNotWhitespace(source);
            if (ZIP_CODE_EXPRESSION.IsMatch(source))
                return ParseExtensions.extractNumeric(source);    

            if (!zeroPad || !ZERO_PAD_EXPRESSION.IsMatch(source))            
                return null;
            
            source = ParseExtensions.extractNumeric(source);
            if (source.Length == (5 - 2) || source.Length == (9 - 2))
                return "00" + source;

            return "0" + source;
        }

        public static bool isZipCode(String source)
        {
            return parseZipCode(source) != null;
        }
    }
}