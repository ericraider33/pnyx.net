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