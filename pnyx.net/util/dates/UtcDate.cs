using System;

namespace pnyx.net.util.dates;

public readonly struct UtcDate : IComparable<UtcDate>, IFormattable, IUtcCapable
{
    public DateTime utc { get; }

    public UtcDate(DateTime utc)
    {
        this.utc = utc;
    }

    public static implicit operator UtcDate(DateTime raw)
    {
        return new UtcDate(raw);
    }

    public static implicit operator UtcDate?(DateTime? raw)
    {
        if (raw == null)
            return null;
        
        return new UtcDate(raw.Value);
    }

    public static implicit operator DateTime(UtcDate source)
    {
        return source.utc;
    }
    
    public int CompareTo(UtcDate other)
    {
        return utc.CompareTo(other.utc);
    }

    public string ToString(String? format, IFormatProvider? formatProvider)
    {
        return utc.ToString(format, formatProvider);
    }

    public override string ToString()
    {
        return utc.toIso8601Timestamp();
    }

    public static UtcDate parse(String text)
    {
        return DateUtil.parseIso8601Timestamp(text);
    }    

    public static bool operator ==(UtcDate a, UtcDate b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(UtcDate a, UtcDate b)
    {
        return !Equals(a, b);
    }
    
    public static bool operator >(UtcDate a, UtcDate b)
    {
        return a.CompareTo(b) > 0;
    }

    public static bool operator <(UtcDate a, UtcDate b)
    {
        return a.CompareTo(b) < 0;
    }
    
    public static bool operator >=(UtcDate a, UtcDate b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(UtcDate a, UtcDate b)
    {
        return a.CompareTo(b) <= 0;
    }

    public override int GetHashCode()
    {
        return utc.GetHashCode();
    }
    
    public UtcDate add(TimeSpan duration)
    {
        return new UtcDate(utc + duration);
    }

    public UtcDate addSeconds(int seconds)
    {
        return new UtcDate(utc.AddSeconds(seconds));
    }

    public UtcDate addMinutes(int minutes, int seconds = 0)
    {
        DateTime toSet = utc.AddMinutes(minutes);
        if (seconds > 0) toSet = toSet.AddSeconds(seconds);
        return new UtcDate(toSet);
    }

    public UtcDate addHours(int hours, int minutes = 0, int seconds = 0)
    {
        DateTime toSet = utc.AddHours(hours);
        if (minutes > 0) toSet = toSet.AddMinutes(minutes);
        if (seconds > 0) toSet = toSet.AddSeconds(seconds);
        return new UtcDate(toSet);
    }
    
    public UtcDate addDays(int days, int hours = 0, int minutes = 0, int seconds = 0)
    {
        DateTime toSet = utc.AddDays(days);
        if (hours > 0) toSet = toSet.AddHours(hours);
        if (minutes > 0) toSet = toSet.AddMinutes(minutes);
        if (seconds > 0) toSet = toSet.AddSeconds(seconds);
        return new UtcDate(toSet);
    }

    public UtcDate addWeeks(int x)
    {
        return new UtcDate(utc.AddDays(x * 7));
    }
    
    public UtcDate addMonths(int months)
    {
        return new UtcDate(utc.AddMonths(months));
    }

    public bool Equals(UtcDate other)
    {
        return utc.Equals(other.utc);
    }

    public override bool Equals(object? obj)
    {
        return obj is UtcDate other && Equals(other);
    }
}
