using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using pncs.cmd;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.cmd;

public class CodeParserTest : IAsyncDisposable
{
    private Pnyx? p;
        
    public CodeParserTest()
    {
        p = new Pnyx();
        p.setSettings(stdIoDefault: true, processOnDispose: false);
    }

    public async ValueTask DisposeAsync()
    {
        if (p != null)
        {
            await p.DisposeAsync();
            p = null;
        }
    }
        
    [Fact]
    public void helloWorld()
    {
        CodeParser parser = new CodeParser();
        parser.parseCode(p, "readString(\"hello world\")");
        Assert.Equal(FluentState.Compiled, p.state);
    }
        
    [Fact]
    public void grep()
    {
        CodeParser parser = new CodeParser();
        parser.parseCode(p, "readString(\"hello world\").grep(\"ll\").writeStdout()");
        Assert.Equal(FluentState.Compiled, p.state);
    }
        
    [Fact]
    public void unknownMethod()
    {
        CodeParser parser = new CodeParser();
        Assert.Throws<CompilationErrorException>(() => parser.parseCode(p,"unknown()"));
    }
        
    [Fact]
    public void invalidSyntax()
    {
        CodeParser parser = new CodeParser();
        Assert.Throws<CompilationErrorException>(() => parser.parseCode(p, "().;."));
    }
                
    [Fact]
    public void semicolon()
    {
        CodeParser parser = new CodeParser();
        parser.parseCode(p, "readString(\"hello world\");\ngrep(\"ll\");\nwriteStdout();");
        Assert.Equal(FluentState.Compiled, p.state);
    }

    [Fact]
    public async Task codeFunc()
    {
        CodeParser parser = new CodeParser();
        parser.parseCode(p, "readString(\"test\").lineFilter(line => true)", compilePnyx: false);
        String actual = await p.processToString();
        Assert.Equal("test", actual);
    }        
        
    [Fact]
    public async Task codeBlock()
    {
        CodeParser parser = new CodeParser();
        parser.parseCode(p, "asCsv(p2 => p2.readString(\"a,b\")).print(\"$2\")", compilePnyx: false);
        String actual = await p.processToString();
        Assert.Equal("b", actual);                        
    }       
        
    [Fact]
    public async Task nameUtil()
    {
        const string code = @"
asCsv(p2 => p2.readString(""Jock Lock Blank III""));
rowTransformer(row =>
{
	var fullName = row[0];
	var name = pnyx.net.util.NameUtil.parseFullName(fullName);
	if (name == null)
		return null;

	return pnyx.net.util.RowUtil.replaceColumn(row, 0, name.firstName, name.middleName, name.lastName, name.suffix);
});
"; 
            
        CodeParser parser = new CodeParser();
        parser.parseCode(p, code, compilePnyx: false);
        String actual = await p.processToString();
        Assert.Equal("Jock,Lock,Blank,Iii", actual);                        
    }
}