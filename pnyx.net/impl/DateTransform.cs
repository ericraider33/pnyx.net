using System;
using System.Globalization;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class DateTransform : ILineTransformer
    {
        public String formatSource;
        public String formatDestination;
        public bool strict;

        public String transformLine(String line)
        {
            if (line.Length == 0)
                return line;

            DateTime date;
            try
            {
                date = DateTime.ParseExact(line, formatSource, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                if (strict)
                    throw new FormatException(String.Format("String '{0}' was not recognized as a valid DateTime format: {1}", line, formatSource));
                else
                    return line;            // return line as-is
            }

            return date.ToString(formatDestination);
        }        
    }
}