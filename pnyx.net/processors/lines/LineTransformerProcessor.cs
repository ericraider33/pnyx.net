using System;
using pnyx.net.api;

namespace pnyx.net.processors.lines;

public class LineTransformerProcessor : ILinePart, ILineProcessor
{
    public ILineTransformer transform;
    public ILineProcessor processor;

    public void processLine(String line)
    {
        line = transform.transformLine(line);
        if (line != null)
            processor.processLine(line);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        processor = next;
    }
}