using System;
using System.Threading.Tasks;
using pnyx.net.errors;
using Xunit;

namespace pnyx.net.test.cmd;

public class PnyxYamlTest
{
    [Fact]
    public async Task helloWorld()
    {
        const String source = @"
# echo 'Hello world'
- readString: Hello World
";
        const String expected = "Hello World";
        await CmdTestUtil.verifyYaml(source, expected);
    }

    [Fact]
    public async Task unknownMethod()
    {
        const String source = @"- junk:";
        InvalidArgumentException error = await Assert.ThrowsAsync<InvalidArgumentException>(() => CmdTestUtil.verifyYaml(source));
        Assert.Equal("Pnyx method can not be found: junk", error.Message);
    }

    [Fact]
    public async Task sequenceParameters()
    {
        String source = @"
- readString: 'Mx trx miscarrx.
  Shx flx.'
- sed:
  - x
  - y
  - ig  
";
        const String expected = "My try miscarry. Shy fly.";
        await CmdTestUtil.verifyYaml(source, expected);

        source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: ['x','y','ig']
";
        await CmdTestUtil.verifyYaml(source, expected);
    }

    [Fact]
    public async Task sequenceParametersDefaults()
    {
        const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: ['x','y']
";
        const String expected = "My trx miscarrx. Shx flx.";
        await CmdTestUtil.verifyYaml(source, expected);
    }        

    [Fact]
    public async Task sequenceParametersTooFew()
    {
        const String source = @"[{'readString': 'Mx trx miscarrx. Shx flx.'},{'sed': ['x'] }]";
        InvalidArgumentException error = await Assert.ThrowsAsync<InvalidArgumentException>(() => CmdTestUtil.verifyYaml(source));
        Assert.Equal("Too few parameters 1 specified for Pnyx method 'sed', which only has 2 required parameters", error.Message);
    }        

    [Fact]
    public async Task sequenceParametersTooMany()
    {
        const String source = @"[{'readString': 'Mx trx miscarrx. Shx flx.'}, {'sed': ['w','x','y','z']}]";
        InvalidArgumentException error = await Assert.ThrowsAsync<InvalidArgumentException>(() => CmdTestUtil.verifyYaml(source));
        Assert.Equal("Too many parameters 4 specified for Pnyx method 'sed', which only has 3 parameters", error.Message);
    }

    [Fact]
    public async Task dictionaryParameters()
    {
        const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: { 'flags': 'ig', 'pattern': 'x', 'replacement': 'y' }
";            
        const String expected = "My try miscarry. Shy fly.";
        await CmdTestUtil.verifyYaml(source, expected);
    }        

    [Fact]
    public async Task dictionaryParametersDefaults()
    {
        const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: { 'pattern': 'x', 'replacement': 'y' }
";            
        const String expected = "My trx miscarrx. Shx flx.";
        await CmdTestUtil.verifyYaml(source, expected);
    }        

    [Fact]
    public async Task dictionaryParametersMissingRequired()
    {
        const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: { 'pattern': 'x' }
";            
        InvalidArgumentException error = await Assert.ThrowsAsync<InvalidArgumentException>(() => CmdTestUtil.verifyYaml(source));
        Assert.Equal("Pnyx method 'sed' is missing required parameter 'replacement'", error.Message);
    }        

    [Fact]
    public async Task dictionaryParametersTooMany()
    {
        const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: { 'flags': 'ig', 'pattern': 'x', 'replacement': 'y', 'junk': 'xxx' }
";            
        InvalidArgumentException error = await Assert.ThrowsAsync<InvalidArgumentException>(() => CmdTestUtil.verifyYaml(source));
        Assert.Equal("Unknown named parameters 'junk' for Pnyx method 'sed', which has parameters 'pattern,replacement,flags'", error.Message);
    }

    [Fact]
    public async Task blockSingle()
    {
        const String source = @"
- cat: 
    block:
      - readString: Line One
      - readString: Line Two
";
        const String expected = 
            @"Line One
Line Two";
        await CmdTestUtil.verifyYaml(source, expected);
    }

    [Theory]
    [InlineData("head:", "My Text\n")]
    [InlineData("head: { limit: 2 }", "My Text\nGoes\n")]
    [InlineData("head: [2]", "My Text\nGoes\n")]
    [InlineData("head: 2", "My Text\nGoes\n")]
    public async Task integerParameter(String input, String expected)
    {
        String source = @"- readString: ""My Text\nGoes\nHere\nand is\nmulti line\n""                            
- {0}
";
        source = String.Format(source, input);
        await CmdTestUtil.verifyYaml(source, expected);
    }

    [Theory]
    [InlineData("parseTab:", "xbx|xax\nxdx|xcx\n")]
    [InlineData("parseTab: false", "xbx|xax\nxdx|xcx\n")]
    [InlineData("parseTab: true", "b|a\nxdx|xcx\n")]
    [InlineData("parseTab: [false]", "xbx|xax\nxdx|xcx\n")]
    [InlineData("parseTab: [true]", "b|a\nxdx|xcx\n")]
    [InlineData("parseTab: no", "xbx|xax\nxdx|xcx\n")]
    [InlineData("parseTab: yes", "b|a\nxdx|xcx\n")]
    public async Task booleanParameter(String input, String expected)
    {
        String source = @"- readString: ""a\tb\nc\td\n""
- {0}
- sed: ['.*', 'x\0x']
- print: $2|$1
";            
        source = String.Format(source, input);
        await CmdTestUtil.verifyYaml(source, expected);
    }
        
    [Theory]
    [InlineData("removeColumns: ", "a\nb\tc\n")]
    [InlineData("removeColumns: 2", "a\nb\n")]
    [InlineData("removeColumns: [2,3]", "a\nb\n")]
    [InlineData("hasColumns: [false, 1]", "a\nb\tc\n")]
    [InlineData("hasColumns: [false, 2]", "b\tc\n")]
    [InlineData("hasColumns: [false, 1, 2]", "b\tc\n")]
    [InlineData("hasColumns: { verifyColumnHasText: false, columnNumbers: [2] }", "b\tc\n")]                
    public async Task multiParameters(String input, String expected)
    {
        String source = @"- readString: ""a\nb\tc\n""
- parseTab: 
- {0}
";            
        source = String.Format(source, input);
        await CmdTestUtil.verifyYaml(source, expected);
    }
        
    [Theory]
    [InlineData("headerNames: ", null)]
    [InlineData("headerNames: HD1", "HD1\thead2\na1\ta2\n")]
    [InlineData("headerNames: [HD1,HD2]", "HD1\tHD2\na1\ta2\n")]
    [InlineData("headerNames: [2,HD2]", "head1\tHD2\na1\ta2\n")]
    public async Task multiObject(String input, String? expected)
    {
        String source = @"- readString: ""head1\thead2\na1\ta2\n""
- parseTab: true 
- {0}
";            
        source = String.Format(source, input);
        if (expected == null)
            await Assert.ThrowsAsync<InvalidArgumentException>(() => CmdTestUtil.verifyYaml(source));
        else
            await CmdTestUtil.verifyYaml(source, expected);
    }
}