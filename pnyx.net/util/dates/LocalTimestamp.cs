using System;

namespace pnyx.net.util.dates;

public readonly struct LocalTimestamp : IComparable<LocalTimestamp>, IFormattable, IUtcCapable
{
    public TimeZoneInfo timeZone { get; }
    public DateTime local { get; }
    
    public LocalTimestamp(TimeZoneInfo timeZone, DateTime localTimestamp)
    {
        this.timeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
        this.local = localTimestamp;
    }

    public static LocalTimestamp fromLocal(TimeZoneInfo timeZone, DateTime localTimestamp)
    {
        if (timeZone == null)
            throw new ArgumentException(nameof(timeZone));

        return new LocalTimestamp(timeZone, localTimestamp);
    }

    public static LocalTimestamp? fromLocal(TimeZoneInfo timeZone, DateTime? localTimestamp)
    {
        if (timeZone == null || localTimestamp == null)
            return null;
            
        return new LocalTimestamp(timeZone, localTimestamp.Value);
    }

    /// <summary>
    /// Converts UTC Timestamps into a timestamp for the passed local timezone 
    /// </summary>
    public static DateTime toLocal(IUtcCapable utcDate, TimeZoneInfo timeZone)
    {
        DateTime x = new DateTime(utcDate.utc.Ticks, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTime(x, timeZone);
    }
    
    public static LocalTimestamp fromUtc(TimeZoneInfo timeZone, DateTime utcTime)
    {
        if (timeZone == null)
            throw new ArgumentException(nameof(timeZone));
        
        return new LocalTimestamp(timeZone, toLocal((UtcDate)utcTime, timeZone));
    }
    
    public static LocalTimestamp? fromUtc(TimeZoneInfo timeZone, DateTime? utcTime)
    {
        if (utcTime == null || timeZone == null)
            return null;
            
        return new LocalTimestamp(timeZone, toLocal((UtcDate)utcTime, timeZone));
    }
    
    public DateTime utc 
    {
        get
        {
            DateTime x = new DateTime(local.Ticks, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(x, timeZone);
        }
    }

    public LocalDay day => new LocalDay(timeZone, local.Date);
    
    public int CompareTo(LocalTimestamp other)
    {
        return utc.CompareTo(other.utc);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return local.ToString(format, formatProvider);
    }

    public override string ToString()
    {
        return local.toIso8601Timestamp();
    }

    public bool Equals(LocalTimestamp other)
    {
        return timeZone.Equals(other.timeZone) && local.Equals(other.local);
    }

    public override bool Equals(object obj)
    {
        return obj is LocalTimestamp other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(timeZone, local);
    }
    
    public static bool operator ==(LocalTimestamp a, LocalTimestamp b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(LocalTimestamp a, LocalTimestamp b)
    {
        return !Equals(a, b);
    }
        
    public static bool operator >(LocalTimestamp a, LocalTimestamp b)
    {
        return a.CompareTo(b) > 0;
    }

    public static bool operator <(LocalTimestamp a, LocalTimestamp b)
    {
        return a.CompareTo(b) < 0;
    }
        
    public static bool operator >=(LocalTimestamp a, LocalTimestamp b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(LocalTimestamp a, LocalTimestamp b)
    {
        return a.CompareTo(b) <= 0;
    }
        
    public static LocalTimestamp parse(String text, TimeZoneInfo timeZone = null)
    {
        DateTime asDate = DateUtil.parseIso8601Timestamp(text);
        return fromLocal(timeZone ?? TimeZoneInfo.Local, asDate);
    }
    
    public LocalTimestamp add(TimeSpan duration)
    {
        return new LocalTimestamp(timeZone, local + duration);
    }
}