using System;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.lines;

public class LineTransformerProcessor : ILinePart, ILineProcessor
{
    public ILineTransformer transform { get; }
    public ILineProcessor? processor { get; private set; }

    public LineTransformerProcessor(ILineTransformer transform)
    {
        this.transform = transform;
    }

    public async Task processLine(String line)
    {
        string? result = transform.transformLine(line);
        if (result != null)
            await processor!.processLine(result);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        processor = next;
    }
}