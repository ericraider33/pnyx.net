using System;
using pnyx.net.errors;
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

        [Fact]
        public void caseSensitive()
        {
            verify("john", "XXXX", "g", "john JOHN John", "XXXX JOHN John");
            verify("JOHN", "XXXX", "g", "john JOHN John", "john XXXX John");
            verify("John", "XXXX", "g", "john JOHN John", "john JOHN XXXX");
            
            verify("john", "XXXX", "gi", "john JOHN John", "XXXX XXXX XXXX");
            verify("JOHN", "XXXX", "gi", "john JOHN John", "XXXX XXXX XXXX");
            verify("John", "XXXX", "gi", "john JOHN John", "XXXX XXXX XXXX");
        }

        private void verify(String pattern, String replacement, String flags, String source, String expected)
        {
            SedReplace replace = new SedReplace(pattern, replacement, flags);
            Assert.Equal(expected, replace.transformLine(source));
        }

        [Fact]
        public void ranges()
        {
            verify("eoe", "XXX", "g",         "eoe eoe eoe eoe eoe eoe eoe eoe eoe eoe", "XXX XXX XXX XXX XXX XXX XXX XXX XXX XXX");
            verify("eoe", "XXX", "1",         "eoe eoe eoe eoe eoe eoe eoe eoe eoe eoe", "XXX eoe eoe eoe eoe eoe eoe eoe eoe eoe");
            verify("eoe", "XXX", "1-1",       "eoe eoe eoe eoe eoe eoe eoe eoe eoe eoe", "XXX eoe eoe eoe eoe eoe eoe eoe eoe eoe");
            verify("eoe", "XXX", "10",        "eoe eoe eoe eoe eoe eoe eoe eoe eoe eoe", "eoe eoe eoe eoe eoe eoe eoe eoe eoe XXX");
            verify("eoe", "XXX", "1,10",      "eoe eoe eoe eoe eoe eoe eoe eoe eoe eoe", "XXX eoe eoe eoe eoe eoe eoe eoe eoe XXX");
            verify("eoe", "XXX", "1-10",      "eoe eoe eoe eoe eoe eoe eoe eoe eoe eoe", "XXX XXX XXX XXX XXX XXX XXX XXX XXX XXX");
            verify("eoe", "XXX", "1-2,9-10",  "eoe eoe eoe eoe eoe eoe eoe eoe eoe eoe", "XXX XXX eoe eoe eoe eoe eoe eoe XXX XXX");
            
            constructionException("eoe", "XXX", "0");
            constructionException("eoe", "XXX", "3-1");
            constructionException("eoe", "XXX", "-1-9");
            constructionException("eoe", "XXX", "-1");
        }
        
        private void constructionException(String pattern, String replacement, String flags)
        {
            Assert.Throws<InvalidArgumentException>(() => new SedReplace(pattern, replacement, flags));
        }

        [Fact]
        public void grouping()
        {
            verify(".*", "\\0", null, "first second third", "first second third");
            verify(".*", "\\0 \\0", null, "first second third", "first second third first second third");
            
            verify("(first) (second) (third)", "\\1 \\3", null, "first second third", "first third");
            verify("(first) (second) (third)", "\\2 \\1 \\3", null, "first second third", "second first third");

            verify("(first) (second) (third)", "\\1 \\3", "g", "first second third first second third", "first third first third");
            verify("(first) (second) (third)", "\\2 \\1 \\3", "g", "first second third first second third", "second first third second first third");

            constructionException(".*", "\\0\\1", null);
        }
        
    }
}