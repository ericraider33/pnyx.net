using System;
using System.IO;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.processors.sort
{
    public class LineSortProcessorTest
    {
        [Theory]
        [InlineData("us_census_surnames.csv", false, false, "us_census_surnames_sorted.csv")]
        [InlineData("us_census_surnames.csv", true, false, "us_census_surnames_descending.csv")]
        public void sort(String source, bool descending, bool caseSensitive, String expected)
        {  
            String inPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", source);
            String outPath = Path.Combine(TestUtil.findTestOutputLocation(), "csv", "line_" + expected);
            FileUtil.assureDirectoryStructExists(outPath);

            using (Pnyx p = new Pnyx())
            {                
                p.read(inPath);
                p.lineFilter(new LineNumberSkip(1));
                p.sortLine(descending, caseSensitive, tempDirectory: Path.Combine(TestUtil.findTestOutputLocation(), "csv"));
                p.write(outPath);
                p.process();                                
            }
            
            String expectedPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", expected);
            Assert.Null(TestUtil.binaryDiff(expectedPath, outPath));
        }              
    }
}