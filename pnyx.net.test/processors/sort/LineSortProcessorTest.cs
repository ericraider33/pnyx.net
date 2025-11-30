using System;
using System.IO;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.processors.sort;

public class LineSortProcessorTest
{
    [Theory]
    [InlineData("us_census_surnames.csv", true, false, false, false, "us_census_surnames_sorted.csv")]
    [InlineData("us_census_surnames.csv", true, true, false, false, "us_census_surnames_descending.csv")]
    [InlineData("super_bowl_winners.csv", false, false, false, false, "super_bowl_winners_sorted.csv")]
    [InlineData("super_bowl_winners.csv", false, false, false, true, "super_bowl_winners_sorted_unique.csv")]
    public void sort(String source, bool hasHeader, bool descending, bool caseSensitive, bool unique, String expected)
    {  
        String inPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", source);
        String outPath = Path.Combine(TestUtil.findTestOutputLocation(), "csv", "line_" + expected);
        FileUtil.assureDirectoryStructExists(outPath);

        using (Pnyx p = new Pnyx())
        {                
            p.read(inPath);
            if (hasHeader)
                p.lineFilter(new SkipSpecificFilter(1));
            p.sort(descending, caseSensitive, unique, tempDirectory: Path.Combine(TestUtil.findTestOutputLocation(), "csv"));
            p.write(outPath);
            p.process();                                
        }
            
        String expectedPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", expected);
        Assert.Null(TestUtil.binaryDiff(expectedPath, outPath));
    }              
}