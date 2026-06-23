namespace pnyx.net.util.dates;

public class LocalTimestampRange : IUtcRange
{
    public LocalTimestamp start { get; }             // inclusive
    public LocalTimestamp end { get; }              // exclusive
    
    public UtcDate startUtcDate => start.utc;
    public UtcDate endUtcDate => end.utc;
    
    public override string ToString()
    {
        return $"{start} - {end}";
    }
    
    public LocalTimestampRange(LocalTimestamp start, LocalTimestamp end)
    {
        this.start = start;
        this.end = end;
    }
}