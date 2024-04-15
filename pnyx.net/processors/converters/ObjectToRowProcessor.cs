using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class ObjectToRowProcessor : IObjectProcessor, IRowPart
{
    public IObjectConverterFromRow converter;
    public IRowProcessor processor;
    private int lineNumber;
    
    public void processObject(object obj)
    {
        lineNumber++;

        if (lineNumber == 1)
        {
            List<String> header = converter.objectToHeader(obj);
            if (header != null)
                processor.rowHeader(header);
        }
        
        List<String> row = converter.objectToRow(obj);
        processor.processRow(row);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        processor = next;
    }
}