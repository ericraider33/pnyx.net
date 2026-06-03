using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class HeightUtilTest
{
    [Fact]
    public void convertInchesToFeet_ValidInches_ReturnsCorrectFormat()
    {
        Assert.Equal("5' 9\"", HeightUtil.convertInchesToFeet(69));
        Assert.Equal("6' 2\"", HeightUtil.convertInchesToFeet(74));
        Assert.Equal("4' 11\"", HeightUtil.convertInchesToFeet(59));
    }

    [Fact]
    public void convertInchesToFeet_ZeroInches_ReturnsZeroFeet()
    {
        Assert.Equal("0' 0\"", HeightUtil.convertInchesToFeet(0));
    }

    [Fact]
    public void convertInchesToFeet_NegativeInches_ReturnsNegativeFormat()
    {
        Assert.Equal("-1' 0\"", HeightUtil.convertInchesToFeet(-12));
        Assert.Equal("-5' 9\"", HeightUtil.convertInchesToFeet(-69));
    }

    [Fact]
    public void convertInchesToFeet_ExactFeet_ReturnsZeroInches()
    {
        Assert.Equal("1' 0\"", HeightUtil.convertInchesToFeet(12));
        Assert.Equal("2' 0\"", HeightUtil.convertInchesToFeet(24));
        Assert.Equal("6' 0\"", HeightUtil.convertInchesToFeet(72));
    }

    [Fact]
    public void convertInchesToFeet_WithRemainingInches_ReturnsCorrectFormat()
    {
        Assert.Equal("1' 1\"", HeightUtil.convertInchesToFeet(13));
        Assert.Equal("2' 1\"", HeightUtil.convertInchesToFeet(25));
        Assert.Equal("3' 1\"", HeightUtil.convertInchesToFeet(37));
    }
}