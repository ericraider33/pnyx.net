using System.Collections.Generic;

namespace pnyx.net.util.dates;

public class UtcDateUtil
{
    public static UtcDate min(UtcDate a, UtcDate b)
    {
        return a < b ? a : b;
    }

    public static UtcDate max(UtcDate a, UtcDate b)
    {
        return a > b ? a : b;
    }

    public static UtcDate? max(UtcDate? a, UtcDate? b)
    {
        if (a == null)
            return b;
        if (b == null)
            return a;
        
        return a.Value > b.Value ? a.Value : b.Value;
    }

    public static UtcDate max(IEnumerable<UtcDate> source)
    {
        UtcDate result = new UtcDate();        // really old date
        foreach (UtcDate x in source)
            result = max(result, x);

        return result;
    }

    public static UtcDate max(params UtcDate[] source)
    {
        UtcDate result = new UtcDate();        // really old date
        foreach (UtcDate x in source)
            result = max(result, x);

        return result;
    }
}