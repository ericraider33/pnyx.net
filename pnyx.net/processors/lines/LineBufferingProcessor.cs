using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.lines;

public class LineBufferingProcessor : ILinePart, ILineProcessor
{
    public ILineBuffering buffering { get; }
    public ILineProcessor? processor { get; private set; }

    public LineBufferingProcessor(ILineBuffering buffering)
    {
        this.buffering = buffering;
    }

    public async Task processLine(String line)
    {
        await forward(buffering.bufferingLine(line));
    }

    public async Task endOfFile()
    {
        await forward(buffering.endOfFile());
        await processor!.endOfFile();
    }

    private async Task forward(List<String>? lines)
    {
        if (lines == null)
            return;
            
        foreach (String line in lines)
            await processor!.processLine(line);
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        processor = next;
    }        
}