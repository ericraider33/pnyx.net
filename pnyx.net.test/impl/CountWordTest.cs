using System;
using pnyx.net.impl;
using Xunit;

namespace pnyx.net.test.impl
{
    public class CountWordTest
    {
        [Theory]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData(" ", 0)]
        [InlineData("a", 1)]
        [InlineData(" a", 1)]
        [InlineData("a ", 1)]
        [InlineData("a1a", 1)]
        [InlineData("a!a", 1)]
        [InlineData("!aa", 1)]
        [InlineData("a b", 2)]
        [InlineData("a b c", 3)]
        [InlineData("a @ ! 1", 4)]
        public void countWords(String text, int expected)
        {
            Assert.Equal(expected, CountWords.countWords(text));
        }
    }
}