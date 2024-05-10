using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace pnyx.net.util;

public static class DateUtil
{
    public const String FORMAT_MDYYYY = "M/d/yyyy";
    public const String FORMAT_ISO_8601_TIMESTAMP = "yyyy-MM-ddTHH:mm:ss.fff'Z'";
    public const String FORMAT_ISO_8601_DATE = "yyyy-MM-dd";
    public static readonly TimeSpan SPAN_1970;
    private static readonly DateTime UTCUnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);                 // UTC 1970-1-1 00:00:00
    
    public static String toMDYYYY(this DateTime x)
    {
        return x.ToString(FORMAT_MDYYYY);
    }

    static DateUtil()
    {
        SPAN_1970 = new TimeSpan(new DateTime(1970,1,1).Ticks);
    }

    public static DateTime addWeeks(this DateTime date, int weeks)
    {
        return date.AddDays(weeks * 7);
    }
    
    public static DateTime? parseNullable(String format, String text)
    {
        if (String.IsNullOrEmpty(text))
            return null;

        try
        {
            return DateTime.ParseExact(text, format, CultureInfo.CurrentCulture);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static DateTime parseExact(String format, String text, DateTimeStyles style = DateTimeStyles.None, int? lineNumber = null)
    {
        try
        {
            return DateTime.ParseExact(text, format, CultureInfo.CurrentCulture, style);
        }
        catch (Exception)
        {
            throw new FormatException(String.Format("String '{0}' was not recognized as a valid DateTime format: {1}{2}", text, format, lineNumber == null ? "" : ", lineNumber="+lineNumber));
        }
    }
    
    public static DateTime convert(String text, int? lineNumber = null)
    {
        try
        {
            return Convert.ToDateTime(text);
        }
        catch (Exception)
        {
            throw new FormatException(String.Format("String '{0}' was not recognized as a valid DateTime{1}", text, lineNumber == null ? "" : ", lineNumber="+lineNumber));
        }
    }


    public static DateTime? convertNullable(String text, int? lineNumber = null)
    {
        if (String.IsNullOrEmpty(text))
            return null;
        
        try
        {
            return Convert.ToDateTime(text);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public static DateTime parseWithYear(String format, String text, int year)
    {
        DateTime result = parseExact(format, text);
        return result.AddYears(year - result.Year);
    }

    public static DateTime parseIso8601Date(String source)
    {
        return parseExact(FORMAT_ISO_8601_DATE, source);
    }

    public static DateTime? parseIso8601DateNullable(String source)
    {
        return parseNullable(FORMAT_ISO_8601_DATE, source);
    }

    private static string getIso8601Format(String source)
    {
        String format = null;
        if (source != null)
        {
            (int, String)[] available = new[]
            {
                (29, "yyyy-MM-ddTHH:mm:ss.fffK"),                           // example: 2024-05-29T07:08:09.000-01:30
                (24, FORMAT_ISO_8601_TIMESTAMP),                           // example: 2024-05-29T07:08:09.000-01:30
                (23, "yyyy-MM-ddTHH:mm:ss.fff"),
                (19, "yyyy-MM-ddTHH:mm:ss"),
                (10, "yyyy-MM-dd")
            };
            format = available.Where(f => f.Item1 == source.Length).Select(f => f.Item2).FirstOrDefault();
        }

        return format ?? FORMAT_ISO_8601_TIMESTAMP;
    }
    
    /// <summary>
    /// Parse an ISO-8601 date. If no timezone is specified, date is assumed in UTC. If a TZ is specified, then date is
    /// automatically converted to UTC time. Supported ISO-8601 formats:
    /// 
    /// yyyy-MM-ddTHH:mm:ss.fffK - example: 2024-05-29T07:08:09.000-01:30
    /// yyyy-MM-ddTHH:mm:ss.fffZ - example: 2024-05-29T07:08:09.000Z
    /// yyyy-MM-ddTHH:mm:ss.fff  - example: 2024-05-29T07:08:09.000
    /// yyyy-MM-ddTHH:mm:ss      - example: 2024-05-29T07:08:09
    /// yyyy-MM-dd               - example: 2024-05-29
    ///
    /// See https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings for definitions
    /// </summary>
    /// <returns>DateTime in UTC</returns>
    public static DateTime parseIso8601Timestamp(String source)
    {
        return parseExact(getIso8601Format(source), source, style: DateTimeStyles.AdjustToUniversal);
    }

    public static DateTime? parseIso8601TimestampNullable(String source)
    {
        return parseNullable(getIso8601Format(source), source);
    }

    public static String toIso8601Date(this DateTime? source)
    {
        if (source == null)
            return null;
        
        return source.Value.ToString(FORMAT_ISO_8601_DATE);
    }

    public static String toIso8601Date(this DateTime source)
    {
        return source.ToString(FORMAT_ISO_8601_DATE);
    }

    public static String toIso8601Timestamp(this DateTime source)
    {
        return source.ToString(FORMAT_ISO_8601_TIMESTAMP);
    }

    public static bool betweenAnd(DateTime date, DateTime startDate, DateTime endDate)
    {
        return DateTime.Compare(date, startDate) >= 0 && DateTime.Compare(date, endDate) <= 0;
    }

    public static int calculateAge(DateTime dateOfBirth, DateTime today)
    {
        int age = today.Year - dateOfBirth.Year - 1;
        if (today.Month > dateOfBirth.Month ||
            today.Month == dateOfBirth.Month && today.Day >= dateOfBirth.Day)
            age++;

        return age;
    }

    public static int? calculateAge(DateTime? dateOfBirth, DateTime today)
    {
        return dateOfBirth == null ? null : new int?(calculateAge(dateOfBirth.Value, today));
    }

    public static int diffDays(this DateTime a, DateTime b)
    {
        return (int)Math.Round(a.Subtract(b).TotalDays, 0);
    }
    
    public static int diffMonths(this DateTime source, DateTime rValue)
    {
        return (source.Month - rValue.Month) + 12 * (source.Year - rValue.Year);
    }        

    public static int diffYears(this DateTime source, DateTime rValue)
    {
        return source.Year - rValue.Year;
    }        

    public static TimeSpan timeOfDayByMinute(DateTime x, int? modInterval = null)
    {
        int minute = x.Minute;
        if (modInterval.HasValue)
            minute -= minute % modInterval.Value;

        TimeSpan timeOfDay = new TimeSpan(0, x.Hour, minute, 0);
        return timeOfDay;
    }

    public static DateTime mostRecentSunday(this DateTime today)
    {
        return startOfWeek(today);
    }

    public static DateTime startOfWeek(this DateTime date, DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
    {
        int startOfWeekOffset = firstDayOfWeek - date.DayOfWeek;

        // if offset would create a date in the "future", then adjust the offset by 1 week (7 days)
        if (startOfWeekOffset > 0)
            startOfWeekOffset -= 7;

        return date.AddDays(startOfWeekOffset).Date;
    }

    public static DateTime findNextDayOfWeek(this DateTime today, DayOfWeek toFind)
    {
        if (today.DayOfWeek == toFind)
            return today;

        int dowInt = (int) today.DayOfWeek;
        int findInt = (int) toFind;
        return findInt > dowInt ? today.AddDays(findInt - dowInt) : today.AddDays(7 + findInt - dowInt);
    }

    public static DateTime findPreviousDayOfWeek(this DateTime today, DayOfWeek toFind)
    {
        if (today.DayOfWeek == toFind)
            return today;

        int dowInt = (int) today.DayOfWeek;
        int findInt = (int) toFind;
        return findInt > dowInt ? today.AddDays(findInt - dowInt - 7) : today.AddDays(findInt - dowInt);
    }

    public static DateTime previousWeekDay(this DateTime today)
    {
        DateTime result = today.AddDays(-1);
        while (result.isWeekEnd())
            result = result.AddDays(-1);

        return result;
    }

    public static DateTime lastWeekDayOfMonth(this DateTime today)
    {
        DateTime startOfThisMonth = new DateTime(today.Year, today.Month, 1);
        DateTime startOfNextMonth = startOfThisMonth.AddMonths(1);
        return previousWeekDay(startOfNextMonth);
    }

    /// <summary>Returns the last day of the month that this date is in</summary>
    public static DateTime lastDayOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

    public static bool isWeekEnd(this DateTime today)
    {
        return today.DayOfWeek == DayOfWeek.Sunday || today.DayOfWeek == DayOfWeek.Saturday;
    }

    public static bool isWeekDay(this DateTime today)
    {
        return today.DayOfWeek != DayOfWeek.Sunday && today.DayOfWeek != DayOfWeek.Saturday;
    }
    
    public static bool isSameMonth(this DateTime source, DateTime other)
    {
        return source.startOfMonth() == other.startOfMonth();
    }

    public static DateTime firstSundayOfYear(this DateTime today)
    {
        DateTime firstOfYear = new DateTime(today.Year, 1, 1);
        if (firstOfYear.DayOfWeek == DayOfWeek.Sunday)
            return firstOfYear;

        DateTime previousSunday = mostRecentSunday(firstOfYear);
        return previousSunday.AddDays(7);
    }

    public static int weekOfYear(this DateTime today)
    {
        DateTime firstSunday = firstSundayOfYear(today);
        int days = today.DayOfYear - firstSunday.DayOfYear;
        if (days < 0)
            return 0;
        if (firstSunday.Day == 1)
            return days / 7;
        return (days / 7) + 1;        
    }

    // Returns 1,2,3,4,5 based on whether date is the 1st,2nd,3rd,4th or 5th DayOfWeek (like Monday) of that month
    public static int dowInMonth(this DateTime today)
    {
        return (today.Day - 1) / 7 + 1;
    }

    public static DateTime startOfDay(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
    }

    public static DateTime startOfMonth(this DateTime today)
    {
        return new DateTime(today.Year, today.Month, 1);
    }

    public static DateTime endOfMonth(this DateTime today)
    {
        return new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
    }

    public static DateTime min(DateTime a, DateTime b)
    {
        return a < b ? a : b;
    }

    public static DateTime max(DateTime a, DateTime b)
    {
        return a > b ? a : b;
    }

    public static DateTime? max(DateTime? a, DateTime? b)
    {
        if (a == null)
            return b;
        if (b == null)
            return a;
        
        return a.Value > b.Value ? a.Value : b.Value;
    }

    public static DateTime max(IEnumerable<DateTime> source)
    {
        DateTime result = new DateTime();        // really old date
        foreach (DateTime x in source)
            result = max(result, x);
        return result;
    }

    public static DateTime limitToSecond(this DateTime value)
    {
        return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
    }
    
    public static DateTime limitToMinute(this DateTime value)
    {
        return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
    }
    
    // Converts a DateTime to unix time. Unix time is the number of seconds between 1970-1-1 0:0:0.0 (unix epoch) and the time (UTC).
    // Returns the number of seconds between Unix epoch and the input time
    public static long toUnixTime(DateTime time)
    {
        return (long)(time - UTCUnixEpoch).TotalSeconds;
    }
    
    // Converts a long representation of a unix time into a DateTime. Unix time is the number of seconds between 1970-1-1 0:0:0.0 (unix epoch) and the time (UTC).
    // Returns a UTC DateTime object representing the unix time
    public static DateTime fromUnixTime(long unixTime)
    {
        return UTCUnixEpoch.AddSeconds(unixTime);
    }
    
    public static DateTime fromUnixTimeWithMilliseconds(long unixTime)
    {
        // Test whether timestamp is millis or second
        const long threshold = 10000000000;
        if (Math.Abs(unixTime) > threshold)
            return UTCUnixEpoch.AddMilliseconds(unixTime);
        
        return UTCUnixEpoch.AddSeconds(unixTime);
    }
}