using System;
using System.Collections.Generic;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class RowUtilTest
    {
        [Theory]
        [InlineData(0, "1", "_")]
        [InlineData(4, "1", "_,1,2,3,4")]
        [InlineData(4, "5", "1,2,3,4,_")]
        [InlineData(4, "1,2", "_,1,_,2,3,4")]
        [InlineData(4, "1,2,3", "_,1,_,2,_,3,4")]
        [InlineData(4, "1,2,3,4", "_,1,_,2,_,3,_,4")]
        public void insertBlankColumns(int original, String columnNumbersText, String expectedText)
        {
            String[] source = new String[original];
            for (int i = 0; i < source.Length; i++)
                source[i] = (i + 1).ToString();

            HashSet<int> columnNumbers = new HashSet<int>(TextUtil.parseInts(columnNumbersText));
            String[] actual = RowUtil.insertBlankColumns(source, columnNumbers, pad: "_");

            String actualText = String.Join(",", actual);
            Assert.Equal(expectedText, actualText);
        }

        [Theory]
        [InlineData(0, "1", "_")]
        [InlineData(4, "1", "1,1,2,3,4")]
        [InlineData(4, "5", "1,2,3,4,_")]
        [InlineData(4, "1,2", "1,1,2,2,3,4")]
        [InlineData(4, "1,2,3", "1,1,2,2,3,3,4")]
        [InlineData(4, "1,2,3,4", "1,1,2,2,3,3,4,4")]
        public void duplicateColumns(int original, String columnNumbersText, String expectedText)
        {
            String[] source = new String[original];
            for (int i = 0; i < source.Length; i++)
                source[i] = (i + 1).ToString();

            HashSet<int> columnNumbers = new HashSet<int>(TextUtil.parseInts(columnNumbersText));
            String[] actual = RowUtil.duplicateColumns(source, columnNumbers, pad: "_");

            String actualText = String.Join(",", actual);
            Assert.Equal(expectedText, actualText);
        }

        [Theory]
        [InlineData(4, "1", "2,3,4")]
        [InlineData(4, "4", "1,2,3")]
        [InlineData(4, "1,2", "3,4")]
        [InlineData(4, "1,2,3", "4")]
        [InlineData(4, "1,2,3,4", "")]

        [InlineData(0, "1,3,5", "")]
        [InlineData(1, "1,3,5", "")]
        [InlineData(2, "1,3,5", "2")]
        [InlineData(3, "1,3,5", "2")]
        [InlineData(4, "1,3,5", "2,4")]
        public void removeColumns(int original, String columnNumbersText, String expectedText)
        {
            String[] source = new String[original];
            for (int i = 0; i < source.Length; i++)
                source[i] = (i + 1).ToString();

            HashSet<int> columnNumbers = new HashSet<int>(TextUtil.parseInts(columnNumbersText));
            String[] actual = RowUtil.removeColumns(source, columnNumbers);

            String actualText = String.Join(",", actual);
            Assert.Equal(expectedText, actualText);
        }
    }
}