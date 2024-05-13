using System;
using System.Collections.Generic;
using System.Linq;

namespace pnyx.net.util.dates;

public static class TimeZoneFinder
{
    private static readonly List<TimeZoneName> tzList = new()
    {
        TimeZoneName.Hawaii,
        TimeZoneName.Alaska,
        TimeZoneName.Pacific,
        TimeZoneName.Arizona,
        TimeZoneName.Mountain,
        TimeZoneName.Central,
        TimeZoneName.Eastern,
        TimeZoneName.Indiana,
        TimeZoneName.GlaceBay
    };
    private static readonly Dictionary<String, TimeZoneName> ianaidMap = tzList.ToDictionary(tzn => tzn.ianaId, tzn => tzn);
    
    public static TimeZoneInfo getTimeZoneInfo(this TimeZoneName name)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(Environment.OSVersion.Platform == PlatformID.Unix ? name.ianaId : name.windowsId);
    }
    
    /// <summary>
    /// Uses the IANA-ID to find TimeZoneInfo in a platform independent way. If null is passed, then null is returned. For all other values, if
    /// the IANA-ID could not find a matching timezone, then an exception is thrown.
    /// </summary>
    /// <exception cref="InvalidTimeZoneException"></exception>
    public static TimeZoneInfo findTimeZoneInfoByIanaId(String ianaId)
    {
        if (ianaId == null)
            return null;
    
        TimeZoneName tzn = ianaidMap.GetValueOrDefault(ianaId);
        if (tzn == null)
            throw new InvalidTimeZoneException($"Could not find a match for tz iana-ID={ianaId}");
        
        return TimeZoneInfo.FindSystemTimeZoneById(Environment.OSVersion.Platform == PlatformID.Unix ? tzn.ianaId : tzn.windowsId);
    }    
}