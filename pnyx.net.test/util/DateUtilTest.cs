using System;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class DateUtilTest
{
    [Fact]
    public void parseIso8601Timestamp_TZD()
    {
        string text = "2024-05-29T07:08:09.123+00:00";
        DateTime x = DateUtil.parseIso8601Timestamp(text);
        Assert.Equal("5/29/2024 7:08:09 AM", x.ToString());
        Assert.Equal(123, x.Millisecond);
    }

    [Theory]
    [InlineData("2024-05-29T07:08:09.000+00:00", "5/29/2024 7:08:09 AM")]
    [InlineData("2024-05-29T07:08:09.000+01:00", "5/29/2024 6:08:09 AM")]
    [InlineData("2024-05-29T07:08:09.000+02:00", "5/29/2024 5:08:09 AM")]
    [InlineData("2024-05-29T07:08:09.000-01:30", "5/29/2024 8:38:09 AM")]
    public void parseIso8601Timestamp_TZD_to_utc(String input, String expected)
    {
        DateTime x = DateUtil.parseIso8601Timestamp(input);
        Assert.Equal(expected, x.ToString());
    }
        
    [Fact]
    public void parseIso8601Timestamp_z()
    {
        string text = "2024-05-29T07:08:09.123Z";
        DateTime x = DateUtil.parseIso8601Timestamp(text);
        Assert.Equal("5/29/2024 7:08:09 AM", x.ToString());
        Assert.Equal(123, x.Millisecond);
    }
    
    [Fact]
    public void parseIso8601Timestamp_no_tz()
    {
        string text = "2024-05-29T07:08:09.123Z";
        DateTime x = DateUtil.parseIso8601Timestamp(text);
        Assert.Equal("5/29/2024 7:08:09 AM", x.ToString());
        Assert.Equal(123, x.Millisecond);
    }

    [Fact]
    public void parseIso8601Timestamp_second()
    {
        string text = "2024-05-29T07:08:09";
        DateTime x = DateUtil.parseIso8601Timestamp(text);
        Assert.Equal("5/29/2024 7:08:09 AM", x.ToString());
        Assert.Equal(0, x.Millisecond);
    }

    [Fact]
    public void parseIso8601Timestamp_date()
    {
        string text = "2024-05-29";
        DateTime x = DateUtil.parseIso8601Timestamp(text);
        Assert.Equal("5/29/2024 12:00:00 AM", x.ToString());
    }
}