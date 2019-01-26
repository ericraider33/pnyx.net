using System;
using System.IO;
using System.Runtime.CompilerServices;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.processors.sort
{
    public class RowSortProcessorTest
    {
        [Theory]
        [InlineData("us_census_surnames.csv", new[] {1}, true, false, false, false, "us_census_surnames_sorted.csv")]
        [InlineData("super_bowl_winners.csv", new[] {1,2}, false, false, false, false, "super_bowl_winners_sorted.csv")]
        [InlineData("super_bowl_winners.csv", new[] {1,2}, false, true, false, true, "super_bowl_winners_sorted_unique.csv")]        
        public void sort(String source, int[] columnNumbers, bool hasHeader, bool unique, bool descending, bool caseSensitive, String expected)
        {  
            String inPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", source);
            String outPath = Path.Combine(TestUtil.findTestOutputLocation(), "csv", "row_" + expected);
            FileUtil.assureDirectoryStructExists(outPath);

            using (Pnyx p = new Pnyx())
            {                
                p.read(inPath);
                p.parseCsv();   
                if (hasHeader)
                    p.lineFilter(new LineNumberSkip(1));                
                p.sortRow(columnNumbers, descending: descending, caseSensitive: caseSensitive, unique: unique, tempDirectory: Path.Combine(TestUtil.findTestOutputLocation(), "csv"));
                p.write(outPath);
                p.process();                                
            }
            
            String expectedPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", expected);
            Assert.Null(TestUtil.binaryDiff(expectedPath, outPath));
        }

        [Fact]
        public void verifyColumnSorting()
        {
            const String input =
@"a,a,9
a,c,7
a,b,5
c,a,1
a,d,8
";
            String actual;
            using (Pnyx p = new Pnyx())
            {                
                p.readString(input);
                p.parseCsv();                
                p.sortRow(new []{ 1 , 3 });
                actual = p.processToString();
            }
            
            const String expect =
@"a,b,5
a,c,7
a,d,8
a,a,9
c,a,1
";            
            Assert.Equal(expect, actual);
        }                
    }
}