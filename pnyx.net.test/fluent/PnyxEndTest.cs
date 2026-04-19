using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.fluent;
using pnyx.net.processors;
using Xunit;

namespace pnyx.net.test.fluent;

public class TestEndLine : ILineProcessor
{
    private readonly StringBuilder builder = new StringBuilder();
        
    public Task processLine(string line)
    {
        builder.Append(line).Append("\n");
        return Task.CompletedTask;
    }

    public Task endOfFile()
    {
        builder.Append("EOF").Append("\n");
        return Task.CompletedTask;
    }

    public override string ToString()
    {
        return builder.ToString();
    }
}
    
public class TestEndRow : IRowProcessor
{
    private readonly StringBuilder builder = new StringBuilder();

    public Task rowHeader(List<string> header)
    {
        builder.Append("Header: ").Append(String.Join("|", header)).Append("\n");            
        return Task.CompletedTask;
    }

    public Task processRow(List<string> row)
    {
        builder.Append(String.Join("|", row)).Append("\n");            
        return Task.CompletedTask;
    }

    public Task endOfFile()
    {
        builder.Append("EOF").Append("\n");
        return Task.CompletedTask;
    }
        
    public override string ToString()
    {
        return builder.ToString();
    }
}
    
public class PnyxEndLineTest
{
    [Fact]
    public async Task setEndRow()
    {
        TestEndRow processor = new TestEndRow();
        await using (Pnyx p = new Pnyx())
        {
            p.readString("a,1\nb,2\nc,3");
            p.parseCsv();
            p.endRow(processor);
        }

        Assert.Equal("a|1\nb|2\nc|3\nEOF\n", processor.ToString());
    }        
        
    [Fact]
    public async Task setEndLine()
    {
        TestEndLine processor = new TestEndLine();
        await using (Pnyx p = new Pnyx())
        {
            p.readString("a\nb\nc");                
            p.endLine(processor);
        }

        Assert.Equal("a\nb\nc\nEOF\n", processor.ToString());
    }        
}