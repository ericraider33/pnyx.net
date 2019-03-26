using System;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class PhoneUtilTest
    {
        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", null, null)]
        [InlineData("asdfa", null, null)]
        [InlineData("0123456789", null, null)]

        // Wrong lengths
        [InlineData("123456789", null, null)]
        [InlineData("1234567", null, null)]
        [InlineData("12345", null, null)]
        
        [InlineData("9995551234", null, "9995551234")]
        [InlineData("5551234", "999", "9995551234")]
        [InlineData("19995551234", null, "9995551234")]
        [InlineData("15551234", "999", null)]
        public void parsePhone(String phone, String defaultAreaCode, String expected)
        {
            String actual = PhoneUtil.parsePhone(phone, defaultAreaCode);
            Assert.Equal(expected, actual);
        }
        
    }
}