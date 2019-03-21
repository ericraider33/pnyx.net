using System;
using pnyx.net.errors;
using pnyx.net.impl.sed;
using Xunit;

namespace pnyx.net.test.impl
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
        // https://github.com/ericraider33/pnyx.net/issues/1
        public void bug1()
        {
            SedReplace replace = new SedReplace("set[ ]*", "_", "");
            Assert.Equal("y _x", replace.transformLine("y set x"));                        
            Assert.Equal("_x", replace.transformLine("set x"));                        

            replace = new SedReplace("set[ ]*", "_", "i");
            Assert.Equal("y _x", replace.transformLine("y set x"));                        
            Assert.Equal("_x", replace.transformLine("set x"));                        
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
        public void replaceLength()
        {
            verify("eoe", "", "g",      "eoe eoe eoe", "  ");            
            verify("eoe", "X", "g",     "eoe eoe eoe", "X X X");            
            verify("eoe", "XX", "g",    "eoe eoe eoe", "XX XX XX");            
            verify("eoe", "XXX", "g",   "eoe eoe eoe", "XXX XXX XXX");            
            verify("eoe", "XXXX", "g",  "eoe eoe eoe", "XXXX XXXX XXXX");            
            verify("eoe", "XXXXX", "g", "eoe eoe eoe", "XXXXX XXXXX XXXXX");            
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
            
            verify(".*", "\\\\0", null, "first", "\\0");            
            verify(".*", "\\\\\\0", null, "first", "\\first");            
            verify(".*", "\\", null, "first", "\\");            
            verify(".*", "\\0\\", null, "first", "first\\");
            verify(".*", "\\n", null, "first", "\n");
            verify(".*", "\\t", null, "first", "\t");
        }
        
    }
}