using System;
using System.Collections.Generic;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.impl.csv;
using pnyx.net.processors.dest;
using pnyx.net.util;
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
            List<String> row = new List<String> { "AA", "BB", "CC", "DD", "EE", "FF", "GG", "HH", "II", "JJ", "KK", "LL", "MM" };
            Print p = new Print { rowConverter = new CsvRowConverter() };
            String actual = p.print(format, null, row);
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
            Print p = new Print();
            String actual = p.print(format, line, new List<String>());
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void printMultiLine()
        {
            CaptureText capture = new CaptureText(new StreamInformation(new Settings()));
            
            Print p = new Print { formatStrings = new [] { "1 $0", "2 $0" }, processor = capture };
            p.processLine("Adam Smith");
            
            Assert.Equal("1 Adam Smith\r\n2 Adam Smith\r\n", capture.capture.ToString());
        }
        
        [Fact]
        public void printMultiRow()
        {
            List<String> row = new List<String> { "AA", "BB" };
            CaptureText capture = new CaptureText(new StreamInformation(new Settings()));
            
            Print p = new Print { formatStrings = new [] { "1 $1", "2 $2" }, processor = capture };
            p.processRow(row);

            Assert.Equal("1 AA\r\n2 BB\r\n", capture.capture.ToString());
        }
        
    }
}