using System;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class UsaStateUtilTest
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("no clue", null)]

    [InlineData("ga", "GA")]
    [InlineData("GA", "GA")]
    [InlineData("Georgia", "GA")]
    [InlineData("georgia", "GA")]

    [InlineData(" ga", "GA")]
    [InlineData("GA ", "GA")]
    [InlineData("  Georgia", "GA")]
    [InlineData("georgia  ", "GA")]
        
    [InlineData("New York", "NY")]
    [InlineData("NewYork", "NY")]
    [InlineData("New   York", "NY")]
    [InlineData("'New  York'", "NY")]
    public void parseState(String input, String expected)
    {
        String actual = UsaStateUtil.parseState(input);
        Assert.Equal(expected, actual);
    }
        
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("no clue", null)]

    [InlineData("ga US", "GA")]
    [InlineData("GA USA", "GA")]
    [InlineData("Georgia us", "GA")]
    [InlineData("georgia usa", "GA")]

    [InlineData(" us", null)]
    [InlineData(" usa", null)]
    public void testUsa(String input, String expected)
    {
        String actual = UsaStateUtil.parseState(input);
        Assert.Equal(expected, actual);
    }
}