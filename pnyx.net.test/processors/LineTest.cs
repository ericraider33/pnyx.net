using System.Collections.Generic;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.processors;

public class LineTest
{
    private const string lineInput = """
MSFT
NVDA
AAPL
AMZN
GOOG
META
AVGO
TSLA
""";

    [Fact]
    public void processCaptureLines()
    {
        using Pnyx p = new();
        p.readString(lineInput);
        p.hasLine();
        List<string> lines = p.processCaptureLines();
        
        Assert.Equal(8, lines.Count);
        Assert.Equal("MSFT", lines[0]);
        Assert.Equal("TSLA", lines[7]);
    }
}