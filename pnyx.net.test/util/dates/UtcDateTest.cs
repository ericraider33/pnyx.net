using System;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class UtcDateTest
{
    [Fact]
    public void toString()
    {
        UtcDate x = new DateTime(2024, 5, 29, 7, 8, 9);
        Assert.Equal("2024-05-29T07:08:09.000Z", x.ToString());

        x = x.utc.AddMilliseconds(11);
        Assert.Equal("2024-05-29T07:08:09.011Z", x.ToString());
    }

    [Fact]
    public void parse()
    {
        string text = "2024-05-29T07:08:09.000Z";
        UtcDate x = UtcDate.parse(text);
        Assert.Equal(text, x.ToString());

        text = "2024-05-29T07:08:09.011Z";
        x = UtcDate.parse(text);
        Assert.Equal(text, x.ToString());
    }

    [Fact]
    public void parse_to_second()
    {
        string input = "2024-05-29T07:08:09";
        string output = "2024-05-29T07:08:09.000Z";
        UtcDate x = UtcDate.parse(input);
        Assert.Equal(output, x.ToString());
    }

    [Fact]
    public void parse_to_date()
    {
        string input = "2024-05-29";
        string output = "2024-05-29T00:00:00.000Z";
        UtcDate x = UtcDate.parse(input);
        Assert.Equal(output, x.ToString());
    }

    [Fact]
    public void toUtc()
    {
        DateTime source = new DateTime(2024, 5, 29, 7, 8, 9);
        source = source + TimeSpan.FromMilliseconds(11);

        UtcDate x = source;
        Assert.Equal(source, x.utc);
    }
}