using System;
using System.Collections.Generic;
using System.Text;
using pnyx.net.fluent;
using pnyx.net.processors;
using Xunit;

namespace pnyx.net.test.fluent
{
    public class TestEndLine : ILineProcessor
    {
        private readonly StringBuilder builder = new StringBuilder();
        
        public void processLine(string line)
        {
            builder.Append(line).Append("\n");
        }

        public void endOfFile()
        {
            builder.Append("EOF").Append("\n");
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
    
    public class TestEndRow : IRowProcessor
    {
        private readonly StringBuilder builder = new StringBuilder();

        public void rowHeader(List<string> header)
        {
            builder.Append("Header: ").Append(String.Join("|", header)).Append("\n");            
        }

        public void processRow(List<string> row)
        {
            builder.Append(String.Join("|", row)).Append("\n");            
        }

        public void endOfFile()
        {
            builder.Append("EOF").Append("\n");
        }
        
        public override string ToString()
        {
            return builder.ToString();
        }
    }
    
    public class PnyxEndLineTest
    {
        [Fact]
        public void setEndRow()
        {
            TestEndRow processor = new TestEndRow();
            using (Pnyx p = new Pnyx())
            {
                p.readString("a,1\nb,2\nc,3");
                p.parseCsv();
                p.endRow(processor);
            }

            Assert.Equal("a|1\nb|2\nc|3\nEOF\n", processor.ToString());
        }        
        
        [Fact]
        public void setEndLine()
        {
            TestEndLine processor = new TestEndLine();
            using (Pnyx p = new Pnyx())
            {
                p.readString("a\nb\nc");                
                p.endLine(processor);
            }

            Assert.Equal("a\nb\nc\nEOF\n", processor.ToString());
        }        
    }
}