using System;
using pnyx.net.api;

namespace pnyx.net.impl;

public class HasLine : ILineFilter
{
    public bool shouldKeepLine(String line)
    {
        return !String.IsNullOrEmpty(line);
    }
}