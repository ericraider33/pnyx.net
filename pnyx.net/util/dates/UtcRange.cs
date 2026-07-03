namespace pnyx.net.util.dates;

public class UtcRange : IUtcRange
{
    public UtcDate startUtcDate { get; }
    public UtcDate endUtcDate { get; }
    
    public UtcRange(UtcDate startUtcDate, UtcDate endUtcDate)
    {
        this.startUtcDate = startUtcDate;
        this.endUtcDate = endUtcDate;
    }
}

public static class UtcRangeExtensions
{
    
    /// <summary>
    /// Method returns the intersection of two ranges, or null if they do not overlap.
    /// </summary>
    public static UtcRange? intersection(this IUtcRange range1, IUtcRange range2)
    {
        UtcDate startUtcDate = DateUtil.max(range1.startUtcDate, range2.startUtcDate);
        UtcDate endUtcDate = DateUtil.min(range1.endUtcDate, range2.endUtcDate);

        if (startUtcDate < endUtcDate)
            return new UtcRange(startUtcDate, endUtcDate);

        return null;
    }
    
    /// <summary>
    /// Method returns the union of two ranges, or null if they do not overlap.
    /// </summary>
    public static UtcRange? union(this IUtcRange range1, IUtcRange range2)
    {
        if (range1.startUtcDate > range2.endUtcDate || range2.startUtcDate > range1.endUtcDate)
            return null;
        
        UtcDate startUtcDate = DateUtil.min(range1.startUtcDate, range2.startUtcDate);
        UtcDate endUtcDate = DateUtil.max(range1.endUtcDate, range2.endUtcDate);
        
        return new UtcRange(startUtcDate, endUtcDate);
    }
    
}