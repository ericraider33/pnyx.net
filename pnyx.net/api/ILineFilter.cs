using System;

namespace pnyx.net.api
{
    public interface ILineFilter
    {
        bool shouldKeep(String line);
    }
}