using System;
using System.Collections.Generic;
using System.Globalization;

namespace pnyx.net.util.dates;

public static class DateOnlyUtil
{
    public static DateOnly? parseNullable(String format, String? text)
    {
        if (String.IsNullOrEmpty(text))
            return null;

        try
        {
            return DateOnly.ParseExact(text, format, CultureInfo.CurrentCulture);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static DateOnly parseExact(String format, String? text, DateTimeStyles style = DateTimeStyles.None, int? lineNumber = null)
    {
        if (text == null)
            throw new ArgumentException("Value cannot be null", nameof(text));
        
        try
        {
            return DateOnly.ParseExact(text, format, CultureInfo.CurrentCulture, style);
        }
        catch (Exception)
        {
            throw new FormatException($"String '{text}' was not recognized as a valid Date format: {format}{(lineNumber == null ? "" : ", lineNumber=" + lineNumber)}");
        }
    }
    
    public static String? toMDYYYY(this DateOnly? x)
    {
        if (x == null)
            return null;
        
        return x.Value.ToString(DateUtil.FORMAT_MDYYYY);
    }
    
    public static String toMDYYYY(this DateOnly x)
    {
        return x.ToString(DateUtil.FORMAT_MDYYYY);
    }

    public static String? toIso8601Date(this DateOnly? source)
    {
        if (source == null)
            return null;
        
        return source.Value.ToString(DateUtil.FORMAT_ISO_8601_DATE);
    }

    public static String toIso8601Date(this DateOnly source)
    {
        return source.ToString(DateUtil.FORMAT_ISO_8601_DATE);
    }
    
    public static DateOnly parseIso8601Date(String source)
    {
        return parseExact(DateUtil.FORMAT_ISO_8601_DATE, source);
    }

    public static DateOnly? parseIso8601DateNullable(String? source)
    {
        return parseNullable(DateUtil.FORMAT_ISO_8601_DATE, source);
    }
    
    public static int calculateAge(DateOnly dateOfBirth, DateOnly today)
    {
        int age = today.Year - dateOfBirth.Year - 1;
        if (today.Month > dateOfBirth.Month ||
            today.Month == dateOfBirth.Month && today.Day >= dateOfBirth.Day)
            age++;

        return age;
    }
    
    public static int? calculateAge(DateOnly? dateOfBirth, DateOnly today)
    {
        return dateOfBirth == null ? null : new int?(calculateAge(dateOfBirth.Value, today));
    }

    public static int diffDays(this DateOnly a, DateOnly b)
    {
        return a.DayNumber - b.DayNumber;
    }
    
    public static int diffMonths(this DateOnly source, DateOnly rValue)
    {
        return (source.Month - rValue.Month) + 12 * (source.Year - rValue.Year);
    }        

    public static int diffYears(this DateOnly source, DateOnly rValue)
    {
        return source.Year - rValue.Year;
    }        

    public static DateOnly mostRecentSunday(this DateOnly today)
    {
        return startOfWeek(today);
    }

    public static DateOnly startOfWeek(this DateOnly date, DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
    {
        int startOfWeekOffset = firstDayOfWeek - date.DayOfWeek;

        // if offset would create a date in the "future", then adjust the offset by 1 week (7 days)
        if (startOfWeekOffset > 0)
            startOfWeekOffset -= 7;

        return date.AddDays(startOfWeekOffset);
    }

    public static DateOnly findNextDayOfWeek(this DateOnly today, DayOfWeek toFind)
    {
        if (today.DayOfWeek == toFind)
            return today;

        int dowInt = (int) today.DayOfWeek;
        int findInt = (int) toFind;
        return findInt > dowInt ? today.AddDays(findInt - dowInt) : today.AddDays(7 + findInt - dowInt);
    }

    public static DateOnly findPreviousDayOfWeek(this DateOnly today, DayOfWeek toFind)
    {
        if (today.DayOfWeek == toFind)
            return today;

        int dowInt = (int) today.DayOfWeek;
        int findInt = (int) toFind;
        return findInt > dowInt ? today.AddDays(findInt - dowInt - 7) : today.AddDays(findInt - dowInt);
    }

    public static DateOnly previousWeekDay(this DateOnly today)
    {
        DateOnly result = today.AddDays(-1);
        while (result.isWeekEnd())
            result = result.AddDays(-1);

        return result;
    }

    public static DateOnly lastWeekDayOfMonth(this DateOnly today)
    {
        DateOnly startOfThisMonth = new DateOnly(today.Year, today.Month, 1);
        DateOnly startOfNextMonth = startOfThisMonth.AddMonths(1);
        return previousWeekDay(startOfNextMonth);
    }

    /// <summary>Returns the last day of the month that this date is in</summary>
    public static DateOnly lastDayOfMonth(this DateOnly date)
    {
        return new DateOnly(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

    public static bool isWeekEnd(this DateOnly today)
    {
        return today.DayOfWeek == DayOfWeek.Sunday || today.DayOfWeek == DayOfWeek.Saturday;
    }

    public static bool isWeekDay(this DateOnly today)
    {
        return today.DayOfWeek != DayOfWeek.Sunday && today.DayOfWeek != DayOfWeek.Saturday;
    }
    
    public static bool isSameMonth(this DateOnly source, DateOnly other)
    {
        return source.startOfMonth() == other.startOfMonth();
    }

    public static DateOnly firstSundayOfYear(this DateOnly today)
    {
        DateOnly firstOfYear = new DateOnly(today.Year, 1, 1);
        if (firstOfYear.DayOfWeek == DayOfWeek.Sunday)
            return firstOfYear;

        DateOnly previousSunday = mostRecentSunday(firstOfYear);
        return previousSunday.AddDays(7);
    }

    public static int weekOfYear(this DateOnly today)
    {
        DateOnly firstSunday = firstSundayOfYear(today);
        int days = today.DayOfYear - firstSunday.DayOfYear;
        if (days < 0)
            return 0;
        if (firstSunday.Day == 1)
            return days / 7;
        return (days / 7) + 1;        
    }

    // Returns 1,2,3,4,5 based on whether date is the 1st,2nd,3rd,4th or 5th DayOfWeek (like Monday) of that month
    public static int dowInMonth(this DateOnly today)
    {
        return (today.Day - 1) / 7 + 1;
    }

    public static DateOnly startOfDay(this DateOnly date)
    {
        return new DateOnly(date.Year, date.Month, date.Day);
    }

    public static DateOnly startOfMonth(this DateOnly today)
    {
        return new DateOnly(today.Year, today.Month, 1);
    }

    public static DateOnly endOfMonth(this DateOnly today)
    {
        return new DateOnly(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
    }

    public static DateOnly min(DateOnly a, DateOnly b)
    {
        return a < b ? a : b;
    }

    public static DateOnly max(DateOnly a, DateOnly b)
    {
        return a > b ? a : b;
    }

    public static DateOnly? max(DateOnly? a, DateOnly? b)
    {
        if (a == null)
            return b;
        if (b == null)
            return a;
        
        return a.Value > b.Value ? a.Value : b.Value;
    }

    public static DateOnly max(IEnumerable<DateOnly> source)
    {
        DateOnly result = new DateOnly();        // really old date
        foreach (DateOnly x in source)
            result = max(result, x);
        return result;
    }

    public static DateOnly limitToWeek(this DateOnly value)
    {
        return value.startOfWeek();
    }
    
    public static DateOnly limitToMonth(this DateOnly value)
    {
        return new DateOnly(value.Year, value.Month, 1);
    }
}