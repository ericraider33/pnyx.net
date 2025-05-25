using System;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class TimeZoneNameTest
{
    [Fact]
    public void testNamesMapToRealTimezones_daylight()
    {
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.Hawaii, "2025-05-29T06:50:09.000-10:00");
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.Alaska, "2025-05-29T08:50:09.000-08:00");
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.Pacific, "2025-05-29T09:50:09.000-07:00");
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.Arizona, "2025-05-29T09:50:09.000-07:00");
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.Mountain, "2025-05-29T10:50:09.000-06:00");
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.Central, "2025-05-29T11:50:09.000-05:00");
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.Eastern, "2025-05-29T12:50:09.000-04:00");
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.GlaceBay, "2025-05-29T13:50:09.000-03:00");
        verifyNamesMapToRealTimezones_daylight(TimeZoneName.UTC, "2025-05-29T16:50:09.000+00:00");
    }

    private void verifyNamesMapToRealTimezones_daylight(TimeZoneName tzn, string expected)
    {
        TimeZoneInfo tz = tzn.getTimeZoneInfo();
        LocalTimestamp lt = LocalTimestamp.parse("2025-05-29T16:50:09.000", tz);
        Assert.Equal(expected, lt.ToString());
    }
    
    [Fact]
    public void testNamesMapToRealTimezones_winter()
    {
        verifyNamesMapToRealTimezones_winter(TimeZoneName.Hawaii, "2025-02-16T06:50:09.000-10:00");
        verifyNamesMapToRealTimezones_winter(TimeZoneName.Alaska, "2025-02-16T07:50:09.000-09:00"); // diff
        verifyNamesMapToRealTimezones_winter(TimeZoneName.Pacific, "2025-02-16T08:50:09.000-08:00"); // diff
        verifyNamesMapToRealTimezones_winter(TimeZoneName.Arizona, "2025-02-16T09:50:09.000-07:00");
        verifyNamesMapToRealTimezones_winter(TimeZoneName.Mountain, "2025-02-16T09:50:09.000-07:00"); // diff
        verifyNamesMapToRealTimezones_winter(TimeZoneName.Central, "2025-02-16T10:50:09.000-06:00"); // diff
        verifyNamesMapToRealTimezones_winter(TimeZoneName.Eastern, "2025-02-16T11:50:09.000-05:00"); // diff
        verifyNamesMapToRealTimezones_winter(TimeZoneName.GlaceBay, "2025-02-16T12:50:09.000-04:00"); // diff
        verifyNamesMapToRealTimezones_winter(TimeZoneName.UTC, "2025-02-16T16:50:09.000+00:00");
    }

    private void verifyNamesMapToRealTimezones_winter(TimeZoneName tzn, string expected)
    {
        TimeZoneInfo tz = tzn.getTimeZoneInfo();
        LocalTimestamp lt = LocalTimestamp.parse("2025-02-16T16:50:09.000", tz);
        Assert.Equal(expected, lt.ToString());
    }
}