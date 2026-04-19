using System;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.lines;

public class LineFilterProcessor : ILinePart, ILineProcessor
{
    public ILineFilter filter { get; }
    public ILineProcessor? processor { get; private set; }

    public LineFilterProcessor(ILineFilter filter)
    {
        this.filter = filter;
    }

    public async Task processLine(String line)
    {
        if (filter.shouldKeepLine(line))
            await processor!.processLine(line);
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