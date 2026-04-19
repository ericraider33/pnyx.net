using System;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class DateOnlyUtilTest
{
    [Fact]
    public void toString()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        LocalDay ld = LocalDay.fromLocal(tz, new DateTime(2024, 5, 29));
        Assert.Equal("2024-05-29", ld.ToString());
    }
    
    [Fact]
    public void toMDYYYY()
    {
        DateOnly x = new DateOnly(2024, 5, 29);
        Assert.Equal("5/29/2024", x.toMDYYYY());
    }
}