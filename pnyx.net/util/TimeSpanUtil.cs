using System;

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
}