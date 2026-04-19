using System;
using pnyx.net.api;

namespace pnyx.net.shims;

public class LineFilterFunc : ILineFilter
{
    public Func<String, bool> lineFilterFunc { get; }

    public LineFilterFunc(Func<string, bool> lineFilterFunc)
    {
        this.lineFilterFunc = lineFilterFunc;
    }

    public bool shouldKeepLine(String line)
    {
        return lineFilterFunc(line);
    }
}