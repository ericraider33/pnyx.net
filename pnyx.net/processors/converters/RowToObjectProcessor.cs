using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class RowToObjectProcessor : IRowProcessor, IObjectPart
{
    public IObjectConverterFromRow converter;
    public IObjectProcessor processor;
    private List<string> header;
    
    public void rowHeader(List<string> header)
    {
        this.header = header;
    }

    public void processRow(List<string> row)
    {
        Object obj = converter.rowToObject(row, header);
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