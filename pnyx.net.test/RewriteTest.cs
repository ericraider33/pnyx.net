using System;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.fluent;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test;

public class RewriteTest
{
    [Fact]
    public async Task rewriteLine()
    {
        String inPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", "us_census_surnames.csv");
        String outPath = Path.Combine(TestUtil.findTestOutputLocation(), "rewrite", "rewriteLine.csv");
        FileUtil.assureDirectoryStructExists(outPath);

        await using (Pnyx p = new Pnyx())
            await p.read(inPath).grep("schenbach", caseSensitive: false).write(outPath).process();                

        String expectedPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", "us_census_schenbach.csv");
        String diff = TestUtil.binaryDiff(expectedPath, outPath);
        Assert.Null(diff);

        await using (Pnyx p = new Pnyx())
            await p.read(outPath).grep("eschenbach", caseSensitive: false).rewrite().process();
            
        expectedPath = Path.Combine(TestUtil.findTestFileLocation(), "csv", "us_census_eschenbach.csv");
        diff = TestUtil.binaryDiff(expectedPath, outPath);
        Assert.Null(diff);            
    }
}