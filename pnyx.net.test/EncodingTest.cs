using System;
using System.IO;
using pnyx.net.fluent;
using pnyx.net.test.util;
using Xunit;

namespace pnyx.net.test
{
    public class EncodingTest
    {        
        [Theory]
        [InlineData("psalm23.unix.ansi.txt",         "us-ascii-Unix")]
        [InlineData("psalm23.unix.utf-16-be.txt",    "utf-16BE-Unix")]
        [InlineData("psalm23.unix.utf-16-le.txt",    "utf-16-Unix")]
        [InlineData("psalm23.unix.utf-8.txt",        "utf-8-Unix")]
        [InlineData("psalm23.win.ansi.txt",          "us-ascii-Windows")]
        [InlineData("psalm23.win.utf-16-be.txt",     "utf-16BE-Windows")]
        [InlineData("psalm23.win.utf-16-le.txt",     "utf-16-Windows")]
        [InlineData("psalm23.win.utf-8.txt",         "utf-8-Windows")]
        public void location(String fileName, String expectedEncoding)
        {
            verifyEncoding(fileName, expectedEncoding);
        }

        private void verifyEncoding(String file, String expectedEncoding)
        {
            String inPath = Path.Combine(TestHelper.findTestFileLocation(), "encoding", file);
            String outPath = Path.Combine(TestHelper.findTestOutputLocation(), "encoding", file);

            using (Pnyx p = new Pnyx())
            {                
                p.read(inPath);
                p.write(outPath);
                p.process();
                
                String actualEncoding = String.Format("{0}-{1}", p.streamInformation.encoding.WebName, p.streamInformation.retrieveNewLineEnum().ToString());
                Assert.Equal(expectedEncoding, actualEncoding);
            }
            
            Assert.Null(TestHelper.binaryDiff(inPath, outPath));
        }
        
    }
}