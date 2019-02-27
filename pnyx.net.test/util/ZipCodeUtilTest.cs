using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class ZipCodeUtilTest
    {
        [Fact]
        public void parseZipCode()
        {
            Assert.Null(ZipCodeUtil.parseZipCode(null));
            Assert.Null(ZipCodeUtil.parseZipCode(""));
            Assert.Null(ZipCodeUtil.parseZipCode(" "));
            Assert.Null(ZipCodeUtil.parseZipCode("a30068 "));
            Assert.Null(ZipCodeUtil.parseZipCode("3006"));
            Assert.Null(ZipCodeUtil.parseZipCode("30068-123"));
            Assert.Null(ZipCodeUtil.parseZipCode("30068-12345"));

            Assert.Equal("30068", ZipCodeUtil.parseZipCode("30068"));
            Assert.Equal("30068", ZipCodeUtil.parseZipCode(" 30068 "));
            Assert.Equal("30068", ZipCodeUtil.parseZipCode(" 3 0 0 6 8- "));

            Assert.Equal("300681234", ZipCodeUtil.parseZipCode(" 300681234"));
            Assert.Equal("300681234", ZipCodeUtil.parseZipCode(" 30068-1234"));
            Assert.Equal("300681234", ZipCodeUtil.parseZipCode(" 30068 - 1234"));
        }
        
        [Fact]
        public void parseZipCodeZeroPad()
        {
            Assert.Equal("00300", ZipCodeUtil.parseZipCode("300", zeroPad: true));
            Assert.Equal("03006", ZipCodeUtil.parseZipCode("3006", zeroPad: true));
            Assert.Null(ZipCodeUtil.parseZipCode("30068-123", zeroPad: true));
            Assert.Null(ZipCodeUtil.parseZipCode("30068-12345", zeroPad: true));
            Assert.Equal("030061234", ZipCodeUtil.parseZipCode("3006-1234", zeroPad: true));
            Assert.Equal("003001234", ZipCodeUtil.parseZipCode("300-1234", zeroPad: true));

            Assert.Equal("03006", ZipCodeUtil.parseZipCode("3006", zeroPad: true));
            Assert.Equal("00300", ZipCodeUtil.parseZipCode(" 300 ", zeroPad: true));
            Assert.Equal("03008", ZipCodeUtil.parseZipCode(" 3 0 0 8- ", zeroPad: true));

            Assert.Equal("030061234", ZipCodeUtil.parseZipCode(" 30061234", zeroPad: true));
            Assert.Equal("003001234", ZipCodeUtil.parseZipCode(" 300-1234", zeroPad: true));
            Assert.Equal("003001234", ZipCodeUtil.parseZipCode(" 300 - 1234", zeroPad: true));
        }
        
        [Fact]
        public void isZipCode()
        {            
            Assert.False(ZipCodeUtil.isZipCode(null));
            Assert.False(ZipCodeUtil.isZipCode(""));
            Assert.False(ZipCodeUtil.isZipCode(" "));
            Assert.False(ZipCodeUtil.isZipCode("a30068 "));

            Assert.True(ZipCodeUtil.isZipCode("30068"));
            Assert.True(ZipCodeUtil.isZipCode("30068-"));
            Assert.True(ZipCodeUtil.isZipCode("30068-1234"));
            Assert.True(ZipCodeUtil.isZipCode("300681234"));

            Assert.True(ZipCodeUtil.isZipCode(" 30068 "));
            Assert.True(ZipCodeUtil.isZipCode(" 30068- "));
            Assert.True(ZipCodeUtil.isZipCode(" 30068 - 1234 "));
            Assert.True(ZipCodeUtil.isZipCode("30068 1234"));
        }
    }
}