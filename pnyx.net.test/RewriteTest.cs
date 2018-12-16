using System;
using System.IO;
using pnyx.net.fluent;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test
{
    public class RewriteTest
    {
        [Fact]
        public void rewriteLine()
        {
            String inPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", "us_census_surnames.csv");
            String outPath = Path.Combine(TestUtil.findTestOutputLocation(), "rewrite", "rewriteLine.csv");
            FileUtil.assureDirectoryStructExists(outPath);

            using (Pnyx p = new Pnyx())
                p.read(inPath).grep("schenbach", caseSensitive: false).write(outPath).process();                

            String expectedPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", "us_census_schenbach.csv");
            Assert.Null(TestUtil.binaryDiff(expectedPath, outPath));           

            using (Pnyx p = new Pnyx())
                p.read(outPath).grep("eschenbach", caseSensitive: false).rewrite().process();
            
            expectedPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", "us_census_eschenbach.csv");
            Assert.Null(TestUtil.binaryDiff(expectedPath, outPath));           
        }
    }
}