using System;
using System.Collections.Generic;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class IndexRangeTest
    {
        [Fact]
        public void contains()
        {
            IndexRange range = new IndexRange(2, 4);
            Assert.False(range.containsInclusive(1));
            Assert.True(range.containsInclusive(2));
            Assert.True(range.containsInclusive(3));
            Assert.True(range.containsInclusive(4));
            Assert.False(range.containsInclusive(5));
        }

        [Fact]
        public void containsNull()
        {
            IndexRange range = IndexRange.NULL_RANGE;
            Assert.False(range.containsInclusive(-1));
            Assert.False(range.containsInclusive(0));
            Assert.False(range.containsInclusive(1));
            Assert.False(range.containsInclusive(2));
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("1", "1")]
        [InlineData("1,2", "1|2")]
        [InlineData("1-2", "1-2")]
        [InlineData("1-2,15", "1-2|15")]
        public void parse(String input, String expected)
        {
            List<IndexRange> ranges = IndexRange.parse(input);
            String actual = String.Join("|", ranges);
            Assert.Equal(expected, actual);
        }
        
    }
}