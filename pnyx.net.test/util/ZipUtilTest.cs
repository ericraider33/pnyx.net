using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class ZipUtilTest
    {
        [Fact]
        public void parseZipCode()
        {
            Assert.Equal(null, ZipUtil.parseZipCode(null));
            Assert.Equal(null, ZipUtil.parseZipCode(""));
            Assert.Equal(null, ZipUtil.parseZipCode(" "));
            Assert.Equal(null, ZipUtil.parseZipCode("a30068 "));
            Assert.Equal(null, ZipUtil.parseZipCode("3006"));
            Assert.Equal(null, ZipUtil.parseZipCode("30068-123"));
            Assert.Equal(null, ZipUtil.parseZipCode("30068-12345"));

            Assert.Equal("30068", ZipUtil.parseZipCode("30068"));
            Assert.Equal("30068", ZipUtil.parseZipCode(" 30068 "));
            Assert.Equal("30068", ZipUtil.parseZipCode(" 3 0 0 6 8- "));

            Assert.Equal("300681234", ZipUtil.parseZipCode(" 300681234"));
            Assert.Equal("300681234", ZipUtil.parseZipCode(" 30068-1234"));
            Assert.Equal("300681234", ZipUtil.parseZipCode(" 30068 - 1234"));
        }
        
        [Fact]
        public void parseZipCodeZeroPad()
        {
            Assert.Equal("00300", ZipUtil.parseZipCode("300", zeroPad: true));
            Assert.Equal("03006", ZipUtil.parseZipCode("3006", zeroPad: true));
            Assert.Equal(null, ZipUtil.parseZipCode("30068-123", zeroPad: true));
            Assert.Equal(null, ZipUtil.parseZipCode("30068-12345", zeroPad: true));
            Assert.Equal("030061234", ZipUtil.parseZipCode("3006-1234", zeroPad: true));
            Assert.Equal("003001234", ZipUtil.parseZipCode("300-1234", zeroPad: true));

            Assert.Equal("03006", ZipUtil.parseZipCode("3006", zeroPad: true));
            Assert.Equal("00300", ZipUtil.parseZipCode(" 300 ", zeroPad: true));
            Assert.Equal("03008", ZipUtil.parseZipCode(" 3 0 0 8- ", zeroPad: true));

            Assert.Equal("030061234", ZipUtil.parseZipCode(" 30061234", zeroPad: true));
            Assert.Equal("003001234", ZipUtil.parseZipCode(" 300-1234", zeroPad: true));
            Assert.Equal("003001234", ZipUtil.parseZipCode(" 300 - 1234", zeroPad: true));
        }
        
        [Fact]
        public void isZipCode()
        {
            Assert.Equal(false, ZipUtil.isZipCode(null));
            Assert.Equal(false, ZipUtil.isZipCode(""));
            Assert.Equal(false, ZipUtil.isZipCode(" "));
            Assert.Equal(false, ZipUtil.isZipCode("a30068 "));

            Assert.Equal(true, ZipUtil.isZipCode("30068"));
            Assert.Equal(true, ZipUtil.isZipCode("30068-"));
            Assert.Equal(true, ZipUtil.isZipCode("30068-1234"));
            Assert.Equal(true, ZipUtil.isZipCode("300681234"));

            Assert.Equal(true, ZipUtil.isZipCode(" 30068 "));
            Assert.Equal(true, ZipUtil.isZipCode(" 30068- "));
            Assert.Equal(true, ZipUtil.isZipCode(" 30068 - 1234 "));
            Assert.Equal(true, ZipUtil.isZipCode("30068 1234"));
        }
    }
}