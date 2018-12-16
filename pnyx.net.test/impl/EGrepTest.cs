using System;
using pnyx.net.impl;
using Xunit;

namespace pnyx.net.test.impl
{
    public class EGrepTest
    {
        [Theory]
        [InlineData("John Emerich Edward Dalberg-Acton", "acton", true, false)]
        [InlineData("John Emerich Edward Dalberg-Acton", "acton", false, true)]
        [InlineData("John Emerich Edward Dalberg-Acton", "John", false, true)]
        [InlineData("John Emerich Edward Dalberg-Acton", "John", true, true)]
        [InlineData("John Emerich Edward Dalberg-Acton", ".*", false, true)]
        [InlineData("John Emerich Edward Dalberg-Acton", "Edward.*", false, true)]
        public void grep(String source, String textToFind, bool caseSensitive, bool expected)
        {
            EGrep grep = new EGrep(textToFind, caseSensitive);
        }
        
    }
}