using System;
using pnyx.net.processors;

namespace pnyx.net;

public static class IPartExtension
{
    public static T? optionalType<T>(this IPart part) where T : class, IPart
    {
        if (part is T typedPart)
            return typedPart;

        return null;
    }
    
    public static T requireType<T>(this IPart part)
    {
        if (part is T typedPart)
            return typedPart;
        
        throw new InvalidCastException($"Cannot cast IPart to {typeof(T).Name}");
    }

    public static Ta requireTypes<Ta, Tb>(this IPart part)
    {
        if (part is Ta typedPart && part is Tb)
            return typedPart;
        
        throw new InvalidCastException($"Cannot cast IPart to {typeof(Ta).Name} or {typeof(Tb).Name}");
    }
}