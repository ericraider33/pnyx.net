using System;

namespace pnyx.net.util.dates;

public static class TimeZoneFinder
{
    public static TimeZoneInfo getTimeZoneInfo(this TimeZoneName name)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(Environment.OSVersion.Platform == PlatformID.Unix ? name.ianaId : name.windowsId);
    }
}