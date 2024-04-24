using System;
using Microsoft.CodeAnalysis.Scripting;
using pncs.cmd;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.cmd
{
    public class CodeParserTest : IDisposable
    {
        private Pnyx p;
        
        public CodeParserTest()
        {
            p = new Pnyx();
            p.setSettings(stdIoDefault: true, processOnDispose: false);
        }

        public void Dispose()
        {
            if (p != null)
            {
                p.Dispose();
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
        public void codeFunc()
        {
            CodeParser parser = new CodeParser();
            parser.parseCode(p, "readString(\"test\").lineFilter(line => true)", compilePnyx: false);
            String actual = p.processToString();
            Assert.Equal("test", actual);
        }        
        
        [Fact]
        public void codeBlock()
        {
            CodeParser parser = new CodeParser();
            parser.parseCode(p, "asCsv(p2 => p2.readString(\"a,b\")).print(\"$2\")", compilePnyx: false);
            String actual = p.processToString();
            Assert.Equal("b", actual);                        
        }       
        
        [Fact]
        public void nameUtil()
        {
            const string code = @"
asCsv(p2 => p2.readString(""Jock Lock Blank III""));
rowTransformer(row =>
{
	var fullName = row[0];
	var name = pnyx.net.util.NameUtil.parseFullName(fullName);
	if (name == null)
		return null;

	return pnyx.net.util.RowUtil.replaceColumn(row, 1, name.firstName, name.middleName, name.lastName, name.suffix);
});
"; 
            
            CodeParser parser = new CodeParser();
            parser.parseCode(p, code, compilePnyx: false);
            String actual = p.processToString();
            Assert.Equal("Jock,Lock,Blank,Iii", actual);                        
        }
    }
}