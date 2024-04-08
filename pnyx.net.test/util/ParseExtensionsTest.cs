using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.util;

namespace pnyx.net.test.util;

using Xunit;

public class ParseExtensionsTest
{
    [Fact]
    public void ParseInts()
    {
        verifyInt(ParseExtensions.parseInts(null));
        verifyInt(ParseExtensions.parseInts(""));
        verifyInt(ParseExtensions.parseInts("1"), 1);
        verifyInt(ParseExtensions.parseInts("1,2"), 1, 2);
        verifyInt(ParseExtensions.parseInts(" 1 , 2 "), 1, 2);

        verifyInt(ParseExtensions.parseInts(" 1 \n\r 2 ", new[] { '\n' }), 1, 2);
        verifyInt(ParseExtensions.parseInts(" 1 \n\r 2 \n\r", new[] { '\n' }), 1, 2);
    }

    private void verifyInt(List<int> list, params int[] expected)
    {
        List<int> expectedList = new List<int>(expected);
        if (Enumerable.SequenceEqual(expectedList, list))
            return;

        String actualText = String.Join(",", list);
        String expectedText = String.Join(",", expected);
        Console.WriteLine("Source: {0}", actualText);
        Console.WriteLine("Expect: {0}", expectedText);
        Assert.Equal(expectedText, actualText);
    }

    [Fact]
    public void ExtractAlphaNumeric()
    {
        Assert.Equal("", ParseExtensions.extractAlphaNumeric(null));
        Assert.Equal("", ParseExtensions.extractAlphaNumeric(""));
        Assert.Equal("", ParseExtensions.extractAlphaNumeric("~"));
        Assert.Equal("a", ParseExtensions.extractAlphaNumeric("~a"));
        Assert.Equal("ab", ParseExtensions.extractAlphaNumeric("a b"));
        Assert.Equal("ab", ParseExtensions.extractAlphaNumeric("a b&"));
    }

    [Fact]
    public void ExtractNumeric()
    {
        Assert.Equal("", ParseExtensions.extractNumeric(null));
        Assert.Equal("", ParseExtensions.extractNumeric(""));
        Assert.Equal("", ParseExtensions.extractNumeric("~"));
        Assert.Equal("1", ParseExtensions.extractNumeric("~1a"));
        Assert.Equal("12", ParseExtensions.extractNumeric("1 2 a b"));
        Assert.Equal("12", ParseExtensions.extractNumeric("1 2&"));
    }

    [Fact]
    public void ExtractNonWhitespace()
    {
        Assert.Equal("", ParseExtensions.extractNotWhitespace(null));
        Assert.Equal("", ParseExtensions.extractNotWhitespace(""));
        Assert.Equal("~", ParseExtensions.extractNotWhitespace(" ~ "));
        Assert.Equal("~a", ParseExtensions.extractNotWhitespace(" ~ a "));
        Assert.Equal("ab", ParseExtensions.extractNotWhitespace(" ab "));
        Assert.Equal("ab&", ParseExtensions.extractNotWhitespace(" a b& "));
    }

    [Fact]
    public void SplitSpace()
    {
        verifySplitSpace(null, new string[0]);
        verifySplitSpace("", new string[0]);
        verifySplitSpace(" ", new string[0]);
        verifySplitSpace("a", new string[] { "a" });
        verifySplitSpace(" a", new string[] { "a" });
        verifySplitSpace("a ", new string[] { "a" });
        verifySplitSpace("a b", new string[] { "a", "b" });
        verifySplitSpace("a b ", new string[] { "a", "b" });
        verifySplitSpace(" a b", new string[] { "a", "b" });
    }

    private void verifySplitSpace(string input, string[] tokens)
    {
        string[] actual = input.splitSpace();
        
        Assert.Equal(tokens, actual);
    }


    [Fact]
    public void SplitAt()
    {
        Assert.Null(ParseExtensions.splitAt("", ","));
        Assert.Null(ParseExtensions.splitAt(null, ","));
        Assert.Equal(new Tuple<String, String>("a", ""), ParseExtensions.splitAt("a", ","));
        Assert.Equal(new Tuple<String, String>("a", ""), ParseExtensions.splitAt("a,", ","));
        Assert.Equal(new Tuple<String, String>("", "a"), ParseExtensions.splitAt(",a", ","));
        Assert.Equal(new Tuple<String, String>("a", "a"), ParseExtensions.splitAt("a,a", ","));
        Assert.Equal(new Tuple<String, String>("a", "b,c"), ParseExtensions.splitAt("a,b,c", ","));
    }

    [Fact]
    public void SplitAtIndex()
    {
        Assert.Null(ParseExtensions.splitAtIndex("", 0));
        Assert.Null(ParseExtensions.splitAtIndex(null, 0));
        Assert.Equal(new Tuple<String, String>("a", ""), ParseExtensions.splitAtIndex("a", 1));
        Assert.Equal(new Tuple<String, String>("a", ""), ParseExtensions.splitAtIndex("a", 2));
        Assert.Equal(new Tuple<String, String>("", "a"), ParseExtensions.splitAtIndex("a", 0));
        Assert.Equal(new Tuple<String, String>("a", "a"), ParseExtensions.splitAtIndex("aa", 1));
        Assert.Equal(new Tuple<String, String>("a", "bc"), ParseExtensions.splitAtIndex("abc", 1));
        Assert.Equal(new Tuple<String, String>("ab", "c"), ParseExtensions.splitAtIndex("abc", 2));
        Assert.Equal(new Tuple<String, String>("abc", ""), ParseExtensions.splitAtIndex("abc", 3));
    }
}