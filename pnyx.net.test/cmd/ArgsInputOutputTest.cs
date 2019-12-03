using System;
using System.IO;
using pnyx.cmd.shared;
using pnyx.net.fluent;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.cmd
{
    public class ArgsInputOutputTest
    {
        [Theory]
        [InlineData(0, true, true, null)]
        [InlineData(1, false, true, null)]
        [InlineData(2, false, false, null)]

        [InlineData(0, true, false, "Pnyx is not in End state: Start")]
        [InlineData(1, true, true, "Not all inputs have been used.")]
        [InlineData(1, false, false, "Pnyx is not in End state: Start")]
        [InlineData(2, false, true, "Not all inputs have been used.")]
        [InlineData(2, true, false, "Implied output can only be used with 1 or 2 arguments")]
        [InlineData(3, false, true, "Implied input can only be used with 1 or 2 arguments")]
        public void impliedArgs(int argCount, bool read, bool write, String error)
        {
            String inPath = Path.Combine(TestUtil.findTestFileLocation(), "encoding", "psalm23.unix.ansi.txt");
            String outPath = Path.Combine(TestUtil.findTestOutputLocation(), "argsOutput", Guid.NewGuid() + ".txt");
            FileUtil.assureDirectoryStructExists(outPath);

            String[] args;
            switch (argCount)
            {
                default: args = new string[0]; break;
                case 1: args = new [] { inPath }; break;
                case 2: args = new [] { inPath, outPath }; break;
                case 3: args = new [] { inPath, outPath, "junk" }; break;
            }
            ArgsInputOutput numbered = new ArgsInputOutput(args);
            
            using (Pnyx p = new Pnyx())
            {
                p.setSettings(processOnDispose: false);
                p.setNumberedInputOutput(numbered);

                try
                {
                    if (read)
                        p.read(inPath);
                    if (write)
                        p.write(outPath);

                    p.compile();
                    
                    Assert.Null(error);
                }
                catch (Exception err)
                {
                    Assert.Equal(error, err.Message);
                    return;
                }
                
                p.process();
            }
            
            Assert.Null(TestUtil.binaryDiff(inPath, outPath));
        }

        [Theory]
        [InlineData(0, null, null, null)]
        [InlineData(1, 1, null, null)]
        [InlineData(2, 1, 2, null)]

        [InlineData(1, null, null, "Not all inputs have been used.")]
        [InlineData(2, 1, null, "Not all inputs have been used.")]
        [InlineData(3, 1, 2, "Not all inputs have been used.")]
        [InlineData(0, 0, null, "ArgNumber is 1-indexed: must be 1 or greater")]
        [InlineData(0, null, 0, "ArgNumber is 1-indexed: must be 1 or greater")]
        [InlineData(2, 3, null, "ArgNumber 3 is missing from parameters")]
        public void explicitArgs(int argCount, int? readArg, int? writeArg, String error)
        {
            String inPath = Path.Combine(TestUtil.findTestFileLocation(), "encoding", "psalm23.unix.ansi.txt");
            String outPath = Path.Combine(TestUtil.findTestOutputLocation(), "argsOutput", Guid.NewGuid() + ".txt");
            FileUtil.assureDirectoryStructExists(outPath);

            String[] args;
            switch (argCount)
            {
                default: args = new string[0]; break;
                case 1: args = new [] { inPath }; break;
                case 2: args = new [] { inPath, outPath }; break;
                case 3: args = new [] { inPath, outPath, "junk" }; break;
            }
            ArgsInputOutput numbered = new ArgsInputOutput(args);
            
            using (Pnyx p = new Pnyx())
            {
                p.setSettings(processOnDispose: false);
                p.setNumberedInputOutput(numbered);

                try
                {
                    if (readArg == null) p.read(inPath);
                    else p.readArg(readArg.Value);

                    if (writeArg == null) p.write(outPath);
                    else p.writeArg(writeArg.Value);

                    p.compile();
                    
                    Assert.Null(error);
                }
                catch (Exception err)
                {
                    Assert.Equal(error, err.Message);
                    return;
                }
                
                p.process();
            }
            
            Assert.Null(TestUtil.binaryDiff(inPath, outPath));
        }
    }
}