using System;
using System.Globalization;
using pnyx.net.api;

namespace pnyx.net.impl;

public class DateTransform : ILineTransformer
{
    public String formatSource { get; }
    public String formatDestination { get; }
    public bool strict { get; }
    
    public DateTransform(String formatSource, String formatDestination, bool strict = false)
    {
        this.formatSource = formatSource;
        this.formatDestination = formatDestination;
        this.strict = strict;
    }

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
                throw new FormatException($"String '{line}' was not recognized as a valid DateTime format: {formatSource}");
            else
                return line;            // return line as-is
        }

        return date.ToString(formatDestination);
    }        
}