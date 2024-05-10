using System;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class LocalDayTest
{
    [Fact]
    public void toString()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        LocalTimestamp lt = LocalTimestamp.fromUtc(tz, new DateTime(2024, 5, 29, 7, 8, 9));
        Assert.Equal("2024-05-29", lt.day.ToString());
        
        lt = LocalTimestamp.fromUtc(tz, new DateTime(2024, 5, 29, 3, 8, 9));
        Assert.Equal("2024-05-28", lt.day.ToString());
    }

    [Fact]
    public void parse()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        string text = "2024-05-29";
        LocalDay ld = LocalDay.parse(text, tz);
        Assert.Equal("2024-05-29", ld.ToString());
    }

    [Fact]
    public void toUtc()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        DateTime source = new DateTime(2024, 5, 29);

        LocalDay ld = LocalDay.fromLocal(tz, source);
        DateTime expected = source + TimeSpan.FromHours(4);
        Assert.Equal(expected, ld.utc);
    }    
}