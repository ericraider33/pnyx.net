using System;
using pnyx.net.api;

namespace pnyx.net.shims;

public class ObjectFilterFunc : IObjectFilter
{
    public Func<object, bool> filterFunc { get; }

    public ObjectFilterFunc(Func<object, bool> filterFunc)
    {
        this.filterFunc = filterFunc;
    }

    public bool shouldKeepObject(object obj)
    {
        return filterFunc(obj);
    }
}

public class ObjectFilterFunc<T> : IObjectFilter
{
    public Func<T, bool> filterFunc { get; }

    public ObjectFilterFunc(Func<T, bool> filterFunc)
    {
        this.filterFunc = filterFunc;
    }

    public bool shouldKeepObject(object obj)
    {
        return filterFunc((T)obj);
    }
}