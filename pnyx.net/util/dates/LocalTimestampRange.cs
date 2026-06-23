namespace pnyx.net.util.dates;

public class LocalTimestampRange : IUtcRange
{
    public LocalDay start { get; }             // inclusive
    public LocalDay end { get;  }              // exclusive
    
    public UtcDate startUtcDate => start.utc;
    public UtcDate endUtcDate => end.utc;
}