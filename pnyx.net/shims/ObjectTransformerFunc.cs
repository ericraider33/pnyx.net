using System;
using pnyx.net.api;

namespace pnyx.net.shims;

public class ObjectTransformerFunc : IObjectTransformer
{
    public Func<object, object> transformFunc;
    
    public object transformObject(object obj)
    {
        return transformFunc(obj);
    }
}

public class ObjectTransformerFunc<TSource, TDest> : IObjectTransformer
{
    public Func<TSource, TDest> transformFunc;
    
    public object transformObject(object obj)
    {
        return transformFunc((TSource)obj);
    }
}