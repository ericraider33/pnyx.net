using System;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class AddressHelperTest
{
    [Theory]
    [InlineData(null, null)]
    [InlineData(" ", null)]
    [InlineData("139 River Valley Trl, Georgia USA", null)]
    [InlineData("139 River Valley Trl, Georgia USA, 31047", "139 River Valley Trl\nGA 31047")]
    [InlineData("139 River Valley Trl, Kathleen, Georgia USA, 31047", "139 River Valley Trl\nKathleen, GA 31047")]
    [InlineData("139 River Valley Trl, Apartment #2, Kathleen, Georgia USA, 31047", "139 River Valley Trl\nApartment #2\nKathleen, GA 31047")]
    [InlineData("xxx, 139 River Valley Trl, Apartment #2, Kathleen, Georgia USA, 31047", null)]
    public void testParse(String input, String expected)
    {
        Address address = AddressUtil.parse(input);
        String actual = address?.ToString();
        Assert.Equal(expected, actual);
    }
        
}
