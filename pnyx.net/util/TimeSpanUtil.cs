using System;
using System.Xml;

namespace pnyx.net.util;

public static class TimeSpanUtil
{
    public static string toHumanReadable(this TimeSpan span)
    {
        if (span.TotalMinutes < 1)
            return $"{span.Seconds} second{(span.Seconds == 1 ? "" : "s")}";

        if (span.TotalHours < 1)
            return $"{span.Minutes} minute{(span.Minutes == 1 ? "" : "s")} {span.Seconds} seconds";

        if (span.TotalDays < 1)
            return $"{span.Hours} hour{(span.Hours == 1 ? "" : "s")} {span.Minutes} minutes";

        return $"{span.Days} day{(span.Days == 1 ? "" : "s")} {span.Hours} hours";
    }

    /// <summary>
    /// Parses an ISO 8601 formatted string into a TimeSpan. Examples include:
    /// PT1H30M       -> 1 hour 30 minutes
    /// P1DT2H        -> 1 day 2 hours
    /// PT45S         -> 45 seconds
    /// P1DT1H1M1S    -> 1 day 1 hour 1 minute 1 second
    /// PT6.679753S   -> 6.679753 seconds
    /// P2Y2M3DT4H    -> 793.04 days 4 hours
    ///
    /// If input is invalid, a FormatException is thrown.
    /// </summary>
    public static TimeSpan parseIso8601(string text)
    {
        return XmlConvert.ToTimeSpan(text);
    }

    /// <summary>
    /// Parses ISO-8601 timespans but allows for invalid input, which returns null
    /// instead of throwing an exception.
    /// </summary>
    public static TimeSpan? parseIso8601Nullable(string? text)
    {
        if (text == null || string.IsNullOrWhiteSpace(text))
            return null;

        try
        {
            return parseIso8601(text);
        }
        catch (Exception)
        {
            return null;
        }
    }
}