using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class NameValuePairToObjectProcessor : INameValuePairProcessor, IObjectPart
{
    public IObjectConverterFromNameValuePair converter;
    public IObjectProcessor processor;
    
    public void processNameValuePair(IDictionary<string, object> record)
    {
        Object obj = converter.nameValuePairToObject(record);
        processor.processObject(obj);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }

    public void setNextObjectProcessor(IObjectProcessor next)
    {
        processor = next;
    }
}