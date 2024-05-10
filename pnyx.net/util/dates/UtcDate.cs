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
    
    public UtcDate addDays(int x)
    {
        return new UtcDate(utc.AddDays(x));
    }

    public UtcDate addWeeks(int x)
    {
        return new UtcDate(utc.AddDays(x * 7));
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
