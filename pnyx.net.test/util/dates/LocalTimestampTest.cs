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
        Assert.Equal("2024-05-29T03:08:09.000-04:00", lt.ToString());

        lt = lt.add(TimeSpan.FromMilliseconds(11));
        Assert.Equal("2024-05-29T03:08:09.011-04:00", lt.ToString());
    }
    
    [Theory]
    [InlineData("2024-05-29T07:08:09.000-04:00")]
    [InlineData("2024-05-29T08:08:09.000-03:00")]
    [InlineData("2024-05-29T06:08:09.000-05:00")]
    public void parse_TZD(String input)
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        LocalTimestamp lt = LocalTimestamp.parse(input, tz);
        Assert.Equal("2024-05-29T07:08:09.000-04:00", lt.ToString());
    }
    
    [Fact]
    public void parse_utc()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        string text = "2024-05-29T07:08:09.000Z";
        LocalTimestamp lt = LocalTimestamp.parse(text, tz);
        Assert.Equal("2024-05-29T03:08:09.000-04:00", lt.ToString());

        text = "2024-01-29T07:08:09.011Z";
        lt = LocalTimestamp.parse(text, tz);
        Assert.Equal("2024-01-29T02:08:09.011-05:00", lt.ToString());
    }

    [Fact]
    public void parse_to_second()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        string input = "2024-05-29T07:08:09";
        string output = "2024-05-29T03:08:09.000-04:00";
        LocalTimestamp lt = LocalTimestamp.parse(input, tz);
        Assert.Equal(output, lt.ToString());
    }

    [Fact]
    public void parse_to_date()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        string input = "2024-05-29";
        string output = "2024-05-28T20:00:00.000-04:00";
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

    [Fact]
    public void equals_default()
    {
        TimeZoneInfo tz = TimeZoneName.Eastern.getTimeZoneInfo();
        DateTime source = new DateTime(2024, 5, 29);

        LocalTimestamp lt = LocalTimestamp.fromLocal(tz, source);
        LocalTimestamp other = default;
        Assert.False(lt == other);
        Assert.False(lt == default);
        Assert.True(other == default);
    }
}