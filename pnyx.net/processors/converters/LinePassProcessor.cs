using System;
using System.Threading.Tasks;

namespace pnyx.net.processors.converters;

public class LinePassProcessor : ILineProcessor, ILinePart
{
    public ILineProcessor? processor { get; private set; }
        
    public async Task processLine(String line)
    {
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