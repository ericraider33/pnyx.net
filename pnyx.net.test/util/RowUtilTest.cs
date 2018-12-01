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
            String[] source = new string[original];
            for (int i = 0; i < source.Length; i++)
                source[i] = (i + 1).ToString();

            HashSet<int> columnNumbers = new HashSet<int>(TextHelper.parseInts(columnNumbersText));
            String[] actual = RowHelper.insertBlankColumns(source, columnNumbers, pad: "_");

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
            String[] source = new string[original];
            for (int i = 0; i < source.Length; i++)
                source[i] = (i + 1).ToString();

            HashSet<int> columnNumbers = new HashSet<int>(TextHelper.parseInts(columnNumbersText));
            String[] actual = RowHelper.duplicateColumns(source, columnNumbers, pad: "_");

            String actualText = String.Join(",", actual);
            Assert.Equal(expectedText, actualText);
        }

        [Theory]
        [InlineData(4, "1", "2,3,4")]
        [InlineData(4, "4", "1,2,3")]
        [InlineData(4, "1,2", "3,4")]
        [InlineData(4, "1,2,3", "4")]
        [InlineData(4, "1,2,3,4", "")]
        public void removeColumns(int original, String columnNumbersText, String expectedText)
        {
            String[] source = new string[original];
            for (int i = 0; i < source.Length; i++)
                source[i] = (i + 1).ToString();

            HashSet<int> columnNumbers = new HashSet<int>(TextHelper.parseInts(columnNumbersText));
            String[] actual = RowHelper.removeColumns(source, columnNumbers);

            String actualText = String.Join(",", actual);
            Assert.Equal(expectedText, actualText);
        }
    }
}