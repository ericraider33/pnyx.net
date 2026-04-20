using System;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.fluent;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.fluent;

public class EncodingTest
{        
    [Theory]
    [InlineData("psalm23.unix.ansi.txt",         "us-ascii-Unix")]
    [InlineData("psalm23.unix.utf-16-be.txt",    "utf-16BE-Unix")]
    [InlineData("psalm23.unix.utf-16-le.txt",    "utf-16-Unix")]
    [InlineData("psalm23.unix.utf-8.txt",        "utf-8-Unix")]
    public async Task location_unix(String fileName, String expectedEncoding)
    {
        if (Environment.OSVersion.Platform != PlatformID.Unix)
            return;
        
        await verifyEncoding(fileName, expectedEncoding);
    }

    [Theory]
    [InlineData("psalm23.win.ansi.txt",          "us-ascii-Windows")]
    [InlineData("psalm23.win.utf-16-be.txt",     "utf-16BE-Windows")]
    [InlineData("psalm23.win.utf-16-le.txt",     "utf-16-Windows")]
    [InlineData("psalm23.win.utf-8.txt",         "utf-8-Windows")]
    public async Task location_win(String fileName, String expectedEncoding)
    {
        if (Environment.OSVersion.Platform == PlatformID.Unix)
            return;
        
        await verifyEncoding(fileName, expectedEncoding);
    }
    
    private async Task verifyEncoding(String file, String expectedEncoding)
    {
        String inPath = Path.Combine(TestUtil.findTestFileLocation(), "encoding", file);
        String outPath = Path.Combine(TestUtil.findTestOutputLocation(), "encoding", file);
        FileUtil.assureDirectoryStructExists(outPath);

        await using (Pnyx p = new Pnyx())
        {                
            p.read(inPath);
            p.write(outPath);
            await p.process();
                
            String actualEncoding = $"{p.streamInformation.streamEncoding?.WebName}-{p.streamInformation.retrieveStreamNewLineEnum().ToString()}";
            Assert.Equal(expectedEncoding, actualEncoding);
        }
            
        Assert.Null(TestUtil.binaryDiff(inPath, outPath));
    }
        
}