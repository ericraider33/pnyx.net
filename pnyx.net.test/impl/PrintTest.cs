using System;
using pnyx.net.impl;
using Xunit;

namespace pnyx.net.test.impl
{
    public class PrintTest
    {
        [Theory]
        [InlineData("$0", "AA,BB,CC,DD,EE,FF,GG,HH,II,JJ,KK,LL,MM")]
        [InlineData("$0 $0", "AA,BB,CC,DD,EE,FF,GG,HH,II,JJ,KK,LL,MM AA,BB,CC,DD,EE,FF,GG,HH,II,JJ,KK,LL,MM")]
        [InlineData("$1", "AA")]
        [InlineData("$5", "EE")]
        [InlineData("$10", "JJ")]
        [InlineData("$13", "MM")]
        [InlineData("$14", "")]
        [InlineData("$1$5$10", "AAEEJJ")]
        [InlineData("lord$", "lord$")]
        [InlineData("lord\\", "lord\\")]
        [InlineData("lord$tree", "lord$tree")]
        [InlineData("lord\\ditch", "lord\\ditch")]
        [InlineData("\\n", "\n")]
        [InlineData("\\r", "\r")]
        [InlineData("\\t", "\t")]
        [InlineData("\\anythingelse", "\\anythingelse")]
        public void printRow(String format, String expected)
        {
            String[] row = new[] { "AA", "BB", "CC", "DD", "EE", "FF", "GG", "HH", "II", "JJ", "KK", "LL", "MM" };
            Print p = new Print { format = format, rowConverter = new CsvRowConverter() };
            String actual = p.print(null, row);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("$0", "Adam Smith")]
        [InlineData("$0 $0", "Adam Smith Adam Smith")]
        [InlineData("$1", "")]
        [InlineData("x$0x", "xAdam Smithx")]
        public void printLine(String format, String expected)
        {
            String line = "Adam Smith";
            Print p = new Print { format = format };
            String actual = p.print(line, new string[0]);
            Assert.Equal(expected, actual);
        }
    }
}