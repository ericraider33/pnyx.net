using System;

namespace pnyx.net.util.dates;

public readonly struct LocalDay : IComparable<LocalDay>, IFormattable, IUtcCapable
{
    public TimeZoneInfo timeZone { get; }
    public DateTime local { get; }

    public LocalDay(TimeZoneInfo timeZone, DateTime raw)
    {
        if (timeZone == null || raw.Hour != 0 || raw.Minute != 0 || raw.Second != 0 || raw.Millisecond != 0)
            throw new ArgumentException("Date must not contain hours/minutes/seconds/millisecond");
        
        this.timeZone = timeZone;
        local = raw;
    }
    
    public static LocalDay fromLocal(TimeZoneInfo timeZone, DateTime localTimestamp)
    {
        if (timeZone == null)
            throw new ArgumentException(nameof(timeZone));
            
        return new LocalDay(timeZone, localTimestamp.Date);
    }
    
    public static LocalDay? fromLocal(TimeZoneInfo timeZone, DateTime? localTimestamp)
    {
        if (localTimestamp == null || timeZone == null)
            return null;
            
        return new LocalDay(timeZone, localTimestamp.Value.Date);
    }

    public static LocalDay fromUtc(TimeZoneInfo timeZone, DateTime utcTime)
    {
        return LocalTimestamp.fromUtc(timeZone, utcTime).day;
    }
    
    public static LocalDay? fromUtc(TimeZoneInfo timeZone, DateTime? utcTime)
    {
        if (utcTime == null || timeZone == null)
            return null;
            
        return LocalTimestamp.fromUtc(timeZone, utcTime.Value).day;
    }

    public LocalDay? withTimeZone(DateTime? localDate)
    {
        if (localDate == null)
            return null;
            
        return new LocalDay(timeZone, localDate.Value.Date);
    }
    
    public DateTime utc 
    {
        get
        {
            DateTime x = new DateTime(local.Ticks, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(x, timeZone);
        }
    }

    public int CompareTo(LocalDay other)
    {
        return utc.CompareTo(other.utc);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return local.ToString(format, formatProvider);
    }

    public override string ToString()
    {
        return local.toIso8601Date();
    }
        
    public static LocalDay parse(String text, TimeZoneInfo timeZone = null)
    {
        DateTime asDate = DateUtil.parseIso8601Date(text);
        return fromLocal(timeZone ?? TimeZoneInfo.Local, asDate);
    }    

    public bool Equals(LocalDay other)
    {
        return Equals(timeZone, other.timeZone)
            && local.Equals(other.local);
    }

    public override bool Equals(object? obj)
    {
        return obj is LocalDay other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(timeZone, local);
    }
    
    public static bool operator ==(LocalDay a, LocalDay b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(LocalDay a, LocalDay b)
    {
        return !Equals(a, b);
    }
        
    public static bool operator >(LocalDay a, LocalDay b)
    {
        return a.CompareTo(b) > 0;
    }

    public static bool operator <(LocalDay a, LocalDay b)
    {
        return a.CompareTo(b) < 0;
    }
        
    public static bool operator >=(LocalDay a, LocalDay b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(LocalDay a, LocalDay b)
    {
        return a.CompareTo(b) <= 0;
    }
    
    public LocalTimestamp asTimestamp()
    {
        return new LocalTimestamp(timeZone, local);
    }
    
    public LocalTimestamp add(TimeSpan duration)
    {
        return new LocalTimestamp(timeZone, local + duration);
    }
        
    public LocalDay addDays(int x)
    {
        return new LocalDay(timeZone, local.AddDays(x));
    }

    public LocalDay addWeeks(int x)
    {
        return new LocalDay(timeZone, local.AddDays(x * 7));
    }

    public LocalDay mostRecentSunday()
    {
        return new LocalDay(timeZone, local.mostRecentSunday());
    }

    public LocalDay startOfWeek(DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
    {
        return new LocalDay(timeZone, local.startOfWeek(firstDayOfWeek));
    }

    public LocalDay findPreviousDayOfWeek(DayOfWeek toFind)
    {
        return new LocalDay(timeZone, local.findPreviousDayOfWeek(toFind));
    }

    public bool isStartOfMonth()
    {
        return local.Day == 1;
    }

    public bool isStartOfYear()
    {
        return local.Day == 1 && local.Month == 1;
    }

    public bool isWeekEnd()
    {
        return local.isWeekEnd();
    }

    public bool isWeekDay()
    {
        return local.isWeekDay();
    }
        
    public bool isSameMonth(LocalDay other)
    {
        return local.isSameMonth(other.local); 
    }

    public LocalDay startOfMonth()
    {
        return new LocalDay(timeZone, new DateTime(local.Year, local.Month, 1));
    }

    public LocalDay endOfMonth()
    {
        return startOfMonth().addMonth(1).addDays(-1);
    }

    public LocalDay lastWeekDayOfMonth()
    {
        return new LocalDay(timeZone, local.lastWeekDayOfMonth());
    }

    public LocalDay previousWeekDay()
    {
        return new LocalDay(timeZone, local.previousWeekDay());
    }

    public LocalDay addMonth(int months)
    {
        return new LocalDay(timeZone, local.AddMonths(months));
    }

    public LocalDay startOfYear()
    {
        return new LocalDay(timeZone, new DateTime(local.Year, 1, 1));
    }

    public LocalDay addYears(int x)
    {
        return new LocalDay(timeZone, local.AddYears(x));
    }
    
    public LocalDay startOfDecade()
    {
        return new LocalDay(timeZone, new DateTime(local.Year - (local.Year  % 10), 1, 1));
    }
    
    public LocalDay startOfCentury()
    {
        return new LocalDay(timeZone, new DateTime(local.Year - (local.Year  % 100), 1, 1));
    }
}