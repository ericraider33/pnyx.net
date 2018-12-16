using System;
using pnyx.net.impl;
using Xunit;

namespace pnyx.net.test.impl
{
    public class GrepTest
    {
        [Theory]
        [InlineData("John Emerich Edward Dalberg-Acton", "acton", true, false)]
        [InlineData("John Emerich Edward Dalberg-Acton", "acton", false, true)]
        [InlineData("John Emerich Edward Dalberg-Acton", "John", false, true)]
        [InlineData("John Emerich Edward Dalberg-Acton", "John", true, true)]
        [InlineData("John Emerich Edward Dalberg-Acton", ".*", false, false)]
        [InlineData("John Emerich Edward Dalberg-Acton", "Edward.*", false, false)]
        public void grep(String source, String textToFind, bool caseSensitive, bool expected)
        {
            Grep grep = new Grep();
            grep.textToFind = textToFind;
            grep.caseSensitive = caseSensitive;
            
            Assert.Equal(expected, grep.shouldKeepLine(source));
        }
    }
}