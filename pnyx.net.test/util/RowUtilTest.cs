using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.impl.columns;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class RowUtilTest
{
    [Theory]
    [InlineData(0, "1", "_")]
    [InlineData(4, "1", "_,1,2,3,4")]
    [InlineData(4, "5", "1,2,3,4,_")]
    [InlineData(4, "1,2", "_,1,_,2,3,4")]
    [InlineData(4, "1,2,3", "_,1,_,2,_,3,4")]
    [InlineData(4, "1,2,3,4", "_,1,_,2,_,3,_,4")]

    // Junk
    [InlineData(4, "6", "1,2,3,4")]
    public void insertBlankColumns(int original, String columnNumbersText, String expectedText)
    {
        List<String> source = new List<String>(original);
        for (int i = 0; i < original; i++)
            source.Add((i + 1).ToString());

        HashSet<int> columnNumbers = new HashSet<int>(columnNumbersText.parseInts());
        HashSet<ColumnIndex> columnIndices = ColumnIndex.convertColumnNumbersToIndex(columnNumbers);
        List<String> actual = RowUtil.insertBlankColumns(source, columnIndices, pad: "_");

        String actualText = String.Join(",", actual);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]        
    [InlineData(4, "1", "1,1,2,3,4")]
    [InlineData(4, "1,2", "1,1,2,2,3,4")]
    [InlineData(4, "1,2,3", "1,1,2,2,3,3,4")]
    [InlineData(4, "1,2,3,4", "1,1,2,2,3,3,4,4")]
        
    // Junk
    [InlineData(0, "1", "")]        
    [InlineData(4, "5", "1,2,3,4")]
    public void duplicateColumns(int original, String columnNumbersText, String expectedText)
    {
        List<String> source = new List<String>(original);
        for (int i = 0; i < original; i++)
            source.Add((i + 1).ToString());

        HashSet<int> columnNumbers = new HashSet<int>(columnNumbersText.parseInts());
        HashSet<ColumnIndex> columnIndices = ColumnIndex.convertColumnNumbersToIndex(columnNumbers);
        List<String> actual = RowUtil.duplicateColumns(source, columnIndices);

        String actualText = String.Join(",", actual);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData(4, "1", "2,3,4")]
    [InlineData(4, "4", "1,2,3")]
    [InlineData(4, "1,2", "3,4")]
    [InlineData(4, "1,2,3", "4")]
    [InlineData(4, "1,2,3,4", "")]

    [InlineData(4, "5", "1,2,3,4")]        

    [InlineData(0, "1,3,5", "")]
    [InlineData(1, "1,3,5", "")]
    [InlineData(2, "1,3,5", "2")]
    [InlineData(3, "1,3,5", "2")]
    [InlineData(4, "1,3,5", "2,4")]
    public void removeColumns(int original, String columnNumbersText, String expectedText)
    {
        List<String> source = new List<String>(original);
        for (int i = 0; i < original; i++)
            source.Add((i + 1).ToString());

        HashSet<int> columnNumbers = new HashSet<int>(columnNumbersText.parseInts());
        HashSet<ColumnIndex> columnIndices = ColumnIndex.convertColumnNumbersToIndex(columnNumbers);
        List<String> actual = RowUtil.removeColumns(source, columnIndices);

        String actualText = String.Join(",", actual);
        Assert.Equal(expectedText, actualText);
    }
        
    [Theory]
    [InlineData(1, 1, "x,y", "x,y")]
    [InlineData(2, 1, "x,y", "x,y,2")]
    [InlineData(3, 2, "x,y", "1,x,y,3")]
    [InlineData(3, 3, "x,y", "1,2,x,y")]
    [InlineData(3, 4, "x,y", "1,2,3,x,y")]
    [InlineData(3, 5, "x,y", "1,2,3")]
    [InlineData(3, -1, "x,y", null)]
    public void replaceColumns(int original, int columnNumber, String replacementText, String expectedText)
    {
        List<String> source = new List<String>(original);
        for (int i = 0; i < original; i++)
            source.Add((i + 1).ToString());

        String[] replacements = replacementText.Split(',');

        try
        {
            ColumnIndex index = new ColumnIndex(columnNumber-1);
            List<String> actual = RowUtil.replaceColumn(source, index, replacements);

            String actualText = String.Join(",", actual);
            Assert.Equal(expectedText, actualText);
        }
        catch (Exception)
        {
            Assert.Null(expectedText);
        }
    }
}