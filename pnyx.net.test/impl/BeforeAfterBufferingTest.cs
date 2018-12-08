using System;
using System.Collections.Generic;
using pnyx.net.impl;
using pnyx.net.test.util;
using Xunit;

namespace pnyx.net.test.impl
{
    public class BeforeAfterBufferingTest
    {
        [Theory]
        [InlineData( 1, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,2,3,4,5,6,7 })]
        [InlineData( 2, 2, new[] { "1","2","3","4","5","6","7" }, new String[0],                         new int[0])]
        [InlineData( 3, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1" },                         new[] { 1 })]
        [InlineData( 4, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2" },                     new[] { 1,2 })]
        [InlineData( 5, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3" },                 new[] { 1,2,3 })]
        [InlineData( 6, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "5","6","7" },                 new[] { 7 })]
        [InlineData( 7, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "4", "5","6","7" },            new[] { 6,7 })]
        [InlineData( 7, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "3","4", "5","6","7" },        new[] { 5,6,7 })]
        [InlineData( 8, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 3,6,7 })]
        [InlineData( 9, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 2,5,7 })]
        [InlineData(10, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,4,7 })]
        public void before(int num, int before, String[] input, String[] expected, int[] lines)
        {
            Assert.NotEqual(-1, num);
            verify(before, 0, input, expected, lines);
        }
        
        [Theory]
        [InlineData( 1, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,2,3,4,5,6,7 })]
        [InlineData( 2, 2, new[] { "1","2","3","4","5","6","7" }, new String[0],                         new int[0])]
        [InlineData( 3, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3" },                 new[] { 1 })]
        [InlineData( 4, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4" },             new[] { 1,2 })]
        [InlineData( 5, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5" },         new[] { 1,2,3 })]
        [InlineData( 6, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "7" },                         new[] { 7 })]
        [InlineData( 7, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "6","7" },                     new[] { 6,7 })]
        [InlineData( 7, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "5","6","7" },                 new[] { 5,6,7 })]
        [InlineData( 8, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,4,7 })]
        [InlineData( 9, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,3,6 })]
        [InlineData(10, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,2,5 })]
        public void after(int num, int after, String[] input, String[] expected, int[] lines)
        {
            Assert.NotEqual(-1, num);
            verify(0, after, input, expected, lines);
        }        
        
        [Theory]
        [InlineData( 1, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,2,3,4,5,6,7 })]
        [InlineData( 2, 2, new[] { "1","2","3","4","5","6","7" }, new String[0],                         new int[0])]
        [InlineData( 3, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3" },                 new[] { 1 })]
        [InlineData( 4, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4" },             new[] { 1,2 })]
        [InlineData( 5, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5" },         new[] { 1,2,3 })]
        [InlineData( 6, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "5","6","7" },                 new[] { 7 })]
        [InlineData( 7, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "4", "5","6","7" },            new[] { 6,7 })]
        [InlineData( 7, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "3","4", "5","6","7" },        new[] { 5,6,7 })]
        [InlineData( 8, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,4,7 })]
        [InlineData( 9, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,3,6 })]
        [InlineData(10, 2, new[] { "1","2","3","4","5","6","7" }, new[] { "1","2","3","4","5","6","7" }, new[] { 1,2,5 })]
        public void both(int num, int x, String[] input, String[] expected, int[] lines)
        {
            Assert.NotEqual(-1, num);
            verify(x, x, input, expected, lines);
        }        
        
        private void verify(int before, int after, String[] input, String[] expected, int[] lines)
        {
            LineNumberFilter filter = new LineNumberFilter(lines);
            BeforeAfterLineBuffering buf = new BeforeAfterLineBuffering(before, after, filter);

            String[] bufOut;
            List<String> output = new List<String>();
            foreach (String x in input)
            {
                bufOut = buf.bufferingLine(x);
                if (bufOut != null)
                    output.AddRange(bufOut);
            }

            bufOut = buf.endOfFile();
            if (bufOut != null)
                output.AddRange(bufOut);

            String[] actual = output.ToArray();
            TestUtil.assertArrayEquals(expected, actual);
        }                
    }
}