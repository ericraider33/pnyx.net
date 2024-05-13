using System;

namespace pnyx.net.util.dates;

public class LocalRange
{
    public LocalDay start { get; }             // inclusive
    public LocalDay end { get;  }               // exclusive
    public LocalRangeEnum type { get; }        // source type

    public LocalRange(LocalDay start, LocalDay end, LocalRangeEnum type = LocalRangeEnum.CustomRange)
    {
        this.start = start;
        this.end = end;
        this.type = type;
    }

    /// <summary>
    /// Creates a range from passed type and the current day. If a custom type is passed, then start/end values may be required.
    /// </summary>
    /// <param name="startDate">StartDate is inclusive</param>
    /// <param name="endDate">EndDate is inclusive from UI, but resultant LocalRange uses an exclusive End value by adding a calendar day to the result</param>
    /// <exception cref="ArgumentException"></exception>
    public static LocalRange build(LocalRangeEnum type, LocalDay today, LocalDay? startDate = null, LocalDay? endDate = null)
    {
        switch (type)
        {
            case LocalRangeEnum.Today: return new LocalRange(today, today.addDays(1), type);
            case LocalRangeEnum.Yesterday: return new LocalRange(today.addDays(-1), today, type);
            
            case LocalRangeEnum.Last7Days: return new LocalRange(today.addDays(-6), today.addDays(1), type);
            case LocalRangeEnum.Last14Days: return new LocalRange(today.addDays(-13), today.addDays(1), type);
            case LocalRangeEnum.Last30Days: return new LocalRange(today.addDays(-29), today.addDays(1), type);
            case LocalRangeEnum.Last60Days: return new LocalRange(today.addDays(-59), today.addDays(1), type);
            case LocalRangeEnum.Last90Days: return new LocalRange(today.addDays(-89), today.addDays(1), type);
            case LocalRangeEnum.Last180Days: return new LocalRange(today.addDays(-179), today.addDays(1), type);
            case LocalRangeEnum.Last365Days: return new LocalRange(today.addDays(-364), today.addDays(1), type);

            case LocalRangeEnum.Last2Years: return new LocalRange(today.addYears(-2), today.addDays(1), type);
            case LocalRangeEnum.Last3Years: return new LocalRange(today.addYears(-3), today.addDays(1), type);
            case LocalRangeEnum.Last5Years: return new LocalRange(today.addYears(-5), today.addDays(1), type);
            case LocalRangeEnum.Last10Years: return new LocalRange(today.addYears(-10), today.addDays(1), type);
            case LocalRangeEnum.Last20Years: return new LocalRange(today.addYears(-20), today.addDays(1), type);
            case LocalRangeEnum.Last30Years: return new LocalRange(today.addYears(-30), today.addDays(1), type);
            case LocalRangeEnum.Last50Years: return new LocalRange(today.addYears(-50), today.addDays(1), type);
            case LocalRangeEnum.Last100Years: return new LocalRange(today.addYears(-100), today.addDays(1), type);
            case LocalRangeEnum.Last200Years: return new LocalRange(today.addYears(-200), today.addDays(1), type);
            
            case LocalRangeEnum.ThisCalendarWeek: return new LocalRange(today.mostRecentSunday(), today.addDays(7).mostRecentSunday(), type);
            case LocalRangeEnum.LastCalendarWeek: return new LocalRange(today.addDays(-7).mostRecentSunday(), today.mostRecentSunday(), type);
            case LocalRangeEnum.ThisCalendarMonth: return new LocalRange(today.startOfMonth(), today.startOfMonth().addMonth(1), type);
            case LocalRangeEnum.LastCalendarMonth: return new LocalRange(today.startOfMonth().addMonth(-1), today.startOfMonth(), type);
            case LocalRangeEnum.ThisCalendarYear: return new LocalRange(today.startOfYear(), today.startOfYear().addYears(1), type);
            case LocalRangeEnum.LastCalendarYear: return new LocalRange(today.startOfYear().addYears(-1), today.startOfYear(), type);
            case LocalRangeEnum.ThisCalendarDecade: return new LocalRange(today.startOfDecade(), today.startOfDecade().addYears(10), type);
            case LocalRangeEnum.LastCalendarDecade: return new LocalRange(today.startOfDecade().addYears(-10), today.startOfDecade(), type);
            case LocalRangeEnum.ThisCalendarCentury: return new LocalRange(today.startOfCentury(), today.startOfCentury().addYears(100), type);
            case LocalRangeEnum.LastCalendarCentury: return new LocalRange(today.startOfCentury().addYears(-100), today.startOfCentury(), type);

            case LocalRangeEnum.Since1900:
            case LocalRangeEnum.All1900: return new LocalRange(new LocalDay(today.timeZone, new DateTime(1900,1,1)), today.addDays(1), type);
            case LocalRangeEnum.Since1970:
            case LocalRangeEnum.All1970: return new LocalRange(new LocalDay(today.timeZone, new DateTime(1970,1,1)), today.addDays(1), type);
            case LocalRangeEnum.Since2000:
            case LocalRangeEnum.All2000: return new LocalRange(new LocalDay(today.timeZone, new DateTime(200,1,1)), today.addDays(1), type);
        }
            
        // Checks if custom range can be associated with a more specific type
        if (type == LocalRangeEnum.CustomRange && startDate != null && endDate != null)
        {
            if (startDate == endDate)
                type = LocalRangeEnum.CustomDay;
            else if (startDate.Value.isStartOfMonth() && endDate == startDate.Value.addMonth(1).addDays(-1))
                type = LocalRangeEnum.CustomMonth;
            else if (startDate.Value.isStartOfYear() && endDate == startDate.Value.addYears(1).addDays(-1))
                type = LocalRangeEnum.CustomYear;
        }
            
        switch (type)
        {
            case LocalRangeEnum.CustomDay: return new LocalRange(requireStartDate(), requireStartDate().addDays(1), type);
            case LocalRangeEnum.CustomMonth: return new LocalRange(requireStartDate().startOfMonth(), requireStartDate().startOfMonth().addMonth(1), type);
            case LocalRangeEnum.CustomYear: return new LocalRange(requireStartDate().startOfYear(), requireStartDate().startOfYear().addYears(1), type);
                
            case LocalRangeEnum.CustomRange:
            {
                if (startDate == null)
                    throw new ArgumentException("End date is required field for custom date ranges");

                return new LocalRange(requireStartDate(), requireEndDate().addDays(1));
            }

            default:
                throw new ArgumentException($"Unknown algorithm for date range={type.getLabel()}");
        }
            
        LocalDay requireStartDate()
        {
            if (startDate == null)
                throw new ArgumentException($"Start date is required field for {type.getLabel()}");
                
            return startDate.Value;
        }
            
        LocalDay requireEndDate()
        {
            if (endDate == null)
                throw new ArgumentException($"End date is required field for {type.getLabel()}");
                
            return endDate.Value;
        }
    }

    public override String ToString()
    {
        return $"start={start:yyyy-M-d},end={end:yyyy-M-d}";
    }

    public bool isInRange(IUtcCapable toCheck)
    {
        DateTime raw = toCheck.utc;
        return raw >= start.utc && raw < end.utc;
    }

    public bool isInRange(DateTime toCheckUtc)
    {
        return toCheckUtc >= start.utc && toCheckUtc < end.utc;
    }
        
    public String getDescription()
    {
        switch (type)
        {
            case LocalRangeEnum.All1900:
            case LocalRangeEnum.All1970:
            case LocalRangeEnum.All2000:
                return "All";
                
            case LocalRangeEnum.CustomRange: return $"{start:M/dd/yyyy} - {end.addDays(-1):M/dd/yyyy}";
            case LocalRangeEnum.CustomDay: return $"{start:M/dd/yyyy}";
            case LocalRangeEnum.CustomMonth: return $"{start:M/yyyy}";
            case LocalRangeEnum.CustomYear: return $"{start:yyyy}";
            default:
                return type.getLabel();
        }
    }
        
    public String getFileName(String baseName)
    {
        switch (type)
        {
            // One day
            case LocalRangeEnum.Today: 
            case LocalRangeEnum.Yesterday:
            case LocalRangeEnum.CustomDay:
                return $"{baseName}_{start:yyyy-MM-dd}";                    

            // One month
            case LocalRangeEnum.ThisCalendarMonth: 
            case LocalRangeEnum.LastCalendarMonth: 
            case LocalRangeEnum.CustomMonth:
                return $"{baseName}_{start:yyyy-MM}";                    

            // One year
            case LocalRangeEnum.ThisCalendarYear:
            case LocalRangeEnum.LastCalendarYear:
            case LocalRangeEnum.CustomYear:
                return $"{baseName}_{start:yyyy}";                    
                
            // All
            case LocalRangeEnum.All1900: 
            case LocalRangeEnum.All1970: 
            case LocalRangeEnum.All2000: 
                return $"{baseName}_all";

            // All others show a date range
            default:
                return $"{baseName}_{start:yyyy-MM-dd}_to_{end.addDays(-1):yyyy-MM-dd}";                    
        }
    }
}