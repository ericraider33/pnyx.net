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

    public UtcDate asUtcDate()
    {
        return new UtcDate(utc);
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
        String withZ = local.toIso8601Timestamp();
        TimeSpan offset = timeZone.GetUtcOffset(local);
        String offsetAsText = $"{offset.Hours:00}:{offset.Minutes:00}";
        if (!offsetAsText.StartsWith("-"))
            offsetAsText = "+" + offsetAsText;
        String result = withZ.Replace("Z", offsetAsText);
        return result;
    }

    public bool Equals(LocalTimestamp other)
    {
        return Object.Equals(timeZone, other.timeZone)
            && local.Equals(other.local);
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
    /// <returns>LocalTimestamp as specified by passed timezone</returns>
    public static LocalTimestamp parse(String text, TimeZoneInfo timeZone = null)
    {
        DateTime asDate = DateUtil.parseIso8601Timestamp(text);
        return fromUtc(timeZone ?? TimeZoneInfo.Local, asDate);
    }
    
    public LocalTimestamp add(TimeSpan duration)
    {
        return new LocalTimestamp(timeZone, local + duration);
    }
}