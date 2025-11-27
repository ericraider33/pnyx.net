using System;

namespace pnyx.net.util.dates;

public class DateRange
{
    public LocalRangeEnum type { get; set; }       
    public DateTime? startDate { get; set; }
    public DateTime? endDate { get; set; }
    
    public LocalRange toLocalRange(LocalDay today)
    {
        LocalDay? startLocal = today.withTimeZone(startDate);
        LocalDay? endLocal = today.withTimeZone(endDate);
        return LocalRange.build(type, today, startLocal, endLocal);
    }
    
    public static implicit operator DateRange(LocalRangeEnum type)
    {
        return new DateRange { type = type }; 
    }
}