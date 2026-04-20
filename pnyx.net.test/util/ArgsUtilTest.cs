using System;
using System.Collections.Generic;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class ArgsUtilTest
{
    [Fact]
    public void testParseDictionary()
    {
        Dictionary<String, String?> switches;

        switches = verifyParseDictionary(["--birm", "6", "..\\yy", "-o=..\\xx"], ["6", "..\\yy"]);
        Assert.True(switches.hasAny("--birm"));
        Assert.Equal("..\\xx", switches.value("-o"));

        switches = verifyParseDictionary(["--birm", "-o=..\\xx", "6", "..\\yy"], ["6", "..\\yy"]);
        Assert.True(switches.hasAny("--birm"));
        Assert.Equal("..\\xx", switches.value("-o"));

        switches = verifyParseDictionary(["-html=<x=1>"], []);
        Assert.Equal("<x=1>", switches.value("-html"));
    }

    private Dictionary<String, String?> verifyParseDictionary(String[] input, String[] expected)
    {
        Dictionary<String, String?> switches = ArgsUtil.parseDictionary(ref input);
        Assert.Equal(expected, input);
        return switches;
    }
}