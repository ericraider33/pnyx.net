using System;

namespace pnyx.net.api;

public interface IObjectFilter
{
    bool shouldKeepObject(Object obj);
}