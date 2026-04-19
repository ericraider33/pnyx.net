using System;
using System.Threading.Tasks;

namespace pnyx.net.processors.dest;

public class LineTeeProcessor : ILineProcessor, ILinePart
{
    public ILineProcessor? processor { get; private set; }
    public ILineProcessor tee { get; }

    public LineTeeProcessor(ILineProcessor tee)
    {
        this.tee = tee;
    }

    public async Task processLine(String line)
    {
        await processor!.processLine(line);
        await tee.processLine(line);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
        await tee.endOfFile();
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        processor = next;
    }
}