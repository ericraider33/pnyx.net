using System;
using pnyx.net.api;

namespace pnyx.net.shims;

public class ObjectFilterFunc : IObjectFilter
{
    public Func<object, bool> filterFunc;
        
    public bool shouldKeepObject(object obj)
    {
        return filterFunc(obj);
    }
}

public class ObjectFilterFunc<T> : IObjectFilter
{
    public Func<T, bool> filterFunc;
        
    public bool shouldKeepObject(object obj)
    {
        return filterFunc((T)obj);
    }
}