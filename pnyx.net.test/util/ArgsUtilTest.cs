using System;
using System.Collections.Generic;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class ArgsUtilTest
    {
        [Fact]
        public void testParseDictionary()
        {
            Dictionary<String, String> switches;

            switches = verifyParseDictionary(new String[] { "--birm", "6", "..\\yy", "-o=..\\xx" }, new string[] { "6", "..\\yy" });
            Assert.True(switches.hasAny("--birm"));
            Assert.Equal("..\\xx", switches.value("-o"));

            switches = verifyParseDictionary(new String[] { "--birm", "-o=..\\xx", "6", "..\\yy" }, new string[] { "6", "..\\yy" });
            Assert.True(switches.hasAny("--birm"));
            Assert.Equal("..\\xx", switches.value("-o"));

            switches = verifyParseDictionary(new String[] { "-html=<x=1>" }, new string[] {});
            Assert.Equal("<x=1>", switches.value("-html"));
        }

        private Dictionary<String, String> verifyParseDictionary(String[] input, String[] expected)
        {
            Dictionary<String, String> switches = ArgsUtil.parseDictionary(ref input);
            Assert.Equal(expected, input);
            return switches;
        }
    }
}