using System.Collections.Generic;
using System.Threading.Tasks;
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
    public async Task processCaptureLines()
    {
        await using Pnyx p = new();
        p.readString(lineInput);
        p.hasLine();
        List<string> lines = await p.processCaptureLines();
        
        Assert.Equal(8, lines.Count);
        Assert.Equal("MSFT", lines[0]);
        Assert.Equal("TSLA", lines[7]);
    }
}