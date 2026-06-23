using System;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class TimeSpanUtilTest
{
    [Fact]
    public void parseIso8601_SimpleHoursAndMinutes_ParsesCorrectly()
    {
        // Arrange
        string input = "PT1H30M";

        // Act
        TimeSpan result = TimeSpanUtil.parseIso8601(input);

        // Assert
        Assert.Equal(1, result.Hours);
        Assert.Equal(30, result.Minutes);
        Assert.Equal(0, result.Seconds);
    }

    [Fact]
    public void parseIso8601_DurationWithDays_ParsesCorrectly()
    {
        // Arrange
        string input = "P1DT2H";

        // Act
        TimeSpan result = TimeSpanUtil.parseIso8601(input);

        // Assert
        Assert.Equal(1, result.Days);
        Assert.Equal(2, result.Hours);
    }

    [Fact]
    public void parseIso8601_SecondsOnly_ParsesCorrectly()
    {
        // Arrange
        string input = "PT45S";

        // Act
        TimeSpan result = TimeSpanUtil.parseIso8601(input);

        // Assert
        Assert.Equal(45, result.Seconds);
    }

    [Fact]
    public void parseIso8601_ComplexDuration_ParsesCorrectly()
    {
        // Arrange
        string input = "P1DT1H1M1S";

        // Act
        TimeSpan result = TimeSpanUtil.parseIso8601(input);

        // Assert
        Assert.Equal(1, result.Days);
        Assert.Equal(1, result.Hours);
        Assert.Equal(1, result.Minutes);
        Assert.Equal(1, result.Seconds);
    }

    [Theory]
    [InlineData("PT6.679753S", "00:00:06.6797530")]
    [InlineData("P2Y2M3DT4H", "793.04:00:00")]
    [InlineData("-PT6S", "-00:00:06")]
    public void parseIso8601_example(string isoText, string expectedText)
    {
        TimeSpan actual = TimeSpanUtil.parseIso8601(isoText);
        TimeSpan expected = TimeSpan.Parse(expectedText);
        
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("A78")]
    [InlineData("p787")]
    [InlineData("e")]
    [InlineData("PT-6S")]
    public void parseIso8601_InvalidFormat_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(() => TimeSpanUtil.parseIso8601(input));
    }
    
}