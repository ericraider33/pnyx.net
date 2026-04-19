using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.sources;

public class LineProcessorFunc : ILinePart, IProcessor
{
    public Func<IEnumerable<String>> source { get; }
    public ILineProcessor? processor { get; private set; }

    public LineProcessorFunc(Func<IEnumerable<string>> source)
    {
        this.source = source;
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        this.processor = next;
    }

    public async Task process()
    {
        IEnumerable<String> data = source();
        foreach (String line in data)
            await processor!.processLine(line);
            
        await processor!.endOfFile();
    }
}