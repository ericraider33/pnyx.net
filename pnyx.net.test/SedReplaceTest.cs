using pnyx.net.transforms.sed;
using Xunit;

namespace pnyx.net.test
{
    public class SedReplaceTest
    {
        [Fact]
        public void basic()
        {
            SedReplace replace = new SedReplace("eoe", "XXX", "g");
            Assert.Equal("my text XXX is here", replace.transformLine("my text eoe is here"));
            Assert.Equal("my XXX XXX XXX", replace.transformLine("my eoe eoe eoe"));
            Assert.Equal("XXXXXXXXX", replace.transformLine("eoeeoeeoe"));
            
            replace = new SedReplace("eoe", "XXX", null);            // same as "1"
            Assert.Equal("my text XXX is here", replace.transformLine("my text eoe is here"));
            Assert.Equal("my XXX eoe eoe", replace.transformLine("my eoe eoe eoe"));
            Assert.Equal("XXXeoeeoe", replace.transformLine("eoeeoeeoe"));
            
            replace = new SedReplace("eoe", "XXX", "2");
            Assert.Equal("my text eoe is here", replace.transformLine("my text eoe is here"));
            Assert.Equal("my eoe XXX eoe", replace.transformLine("my eoe eoe eoe"));
            Assert.Equal("eoeXXXeoe", replace.transformLine("eoeeoeeoe"));
            
            replace = new SedReplace("eoe", "XXX", "g2");
            Assert.Equal("my text eoe is here", replace.transformLine("my text eoe is here"));
            Assert.Equal("my eoe XXX XXX", replace.transformLine("my eoe eoe eoe"));
            Assert.Equal("eoeXXXXXX", replace.transformLine("eoeeoeeoe"));
        }
    }
}