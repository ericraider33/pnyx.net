using System;

namespace pnyx.net.api;

public interface ILineFilter
{
    bool shouldKeepLine(String line);
}