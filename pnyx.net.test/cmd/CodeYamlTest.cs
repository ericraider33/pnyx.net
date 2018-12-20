using System;
using Xunit;

namespace pnyx.net.test.cmd
{
    public class CodeYamlTest
    {
        [Fact]
        public void blockSingle()
        {
            const String source = @"
- readString: 'Line One

Line Two'
- lineFilter:
      filter: 'code_cs(String line)
 
        return line.Contains(""Line"");'
";
            const String expected = "Line One\nLine Two";
            CmdTestUtil.verifyYaml(source, expected);
        }                        
        
    }
}