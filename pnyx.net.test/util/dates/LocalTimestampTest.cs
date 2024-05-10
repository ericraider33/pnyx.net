using System;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class LocalTimestampTest
{
    [Fact]
    public void toString()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        LocalTimestamp lt = LocalTimestamp.fromUtc(tz, new DateTime(2024, 5, 29, 7, 8, 9));
        Assert.Equal("2024-05-29T03:08:09.000Z", lt.ToString());

        lt = lt.add(TimeSpan.FromMilliseconds(11));
        Assert.Equal("2024-05-29T03:08:09.011Z", lt.ToString());
    }

    [Fact]
    public void parse()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        string text = "2024-05-29T07:08:09.000Z";
        LocalTimestamp lt = LocalTimestamp.parse(text, tz);
        Assert.Equal("2024-05-29T07:08:09.000Z", lt.ToString());

        text = "2024-05-29T07:08:09.011Z";
        lt = LocalTimestamp.parse(text, tz);
        Assert.Equal(text, lt.ToString());
    }

    [Fact]
    public void parse_to_second()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        string input = "2024-05-29T07:08:09";
        string output = "2024-05-29T07:08:09.000Z";
        LocalTimestamp lt = LocalTimestamp.parse(input, tz);
        Assert.Equal(output, lt.ToString());
    }

    [Fact]
    public void parse_to_date()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        string input = "2024-05-29";
        string output = "2024-05-29T00:00:00.000Z";
        LocalTimestamp lt = LocalTimestamp.parse(input, tz);
        Assert.Equal(output, lt.ToString());
    }

    [Fact]
    public void toUtc()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        DateTime source = new DateTime(2024, 5, 29, 7, 8, 9);
        source = source + TimeSpan.FromMilliseconds(11);

        LocalTimestamp lt = LocalTimestamp.fromLocal(tz, source);
        DateTime expected = source + TimeSpan.FromHours(4);
        Assert.Equal(expected, lt.utc);
    }    
}