using System;
using Microsoft.CodeAnalysis.Scripting;
using pnyx.cmd;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.cmd
{
    public class CodeParserTest
    {
        [Fact]
        public void helloWorld()
        {
            CodeParser parser = new CodeParser();
            Pnyx p = parser.parseCode("readString(\"hello world\")");
            Assert.Equal(FluentState.Compiled, p.state);
        }
        
        [Fact]
        public void grep()
        {
            CodeParser parser = new CodeParser();
            Pnyx p = parser.parseCode("readString(\"hello world\").grep(\"ll\").writeStdout()");
            Assert.Equal(FluentState.Compiled, p.state);
        }
        
        [Fact]
        public void unknownMethod()
        {
            CodeParser parser = new CodeParser();
            Assert.Throws<CompilationErrorException>(() => parser.parseCode("unknown()"));
        }
        
        [Fact]
        public void invalidSyntax()
        {
            CodeParser parser = new CodeParser();
            Assert.Throws<CompilationErrorException>(() => parser.parseCode("().;."));
        }
                
        [Fact]
        public void semicolon()
        {
            CodeParser parser = new CodeParser();
            Pnyx p = parser.parseCode("readString(\"hello world\");\ngrep(\"ll\");\nwriteStdout();");
            Assert.Equal(FluentState.Compiled, p.state);
        }

        [Fact]
        public void codeFunc()
        {
            CodeParser parser = new CodeParser();
            Pnyx p = parser.parseCode("readString(\"test\").lineFilterFunc(line => true)", compile: false);
            String actual = p.processToString();
            Assert.Equal("test", actual);
        }        
        
        [Fact]
        public void codeBlock()
        {
            CodeParser parser = new CodeParser();
            Pnyx p = parser.parseCode("asCsv(p2 => p2.readString(\"a,b\")).print(\"$2\")", compile: false);
            String actual = p.processToString();
            Assert.Equal("b", actual);                        
        }       
    }
}