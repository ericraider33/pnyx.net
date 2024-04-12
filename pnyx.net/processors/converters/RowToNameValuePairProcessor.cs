using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.processors.converters;

public class RowToNameValuePairProcessor : IRowProcessor, INameValuePairPart
{
    private List<string> header;
    public INameValuePairProcessor processor;
    
    public void rowHeader(List<string> header)
    {
        this.header = header.Select(cleanUpHeader).ToList();
    }

    private String cleanUpHeader(String header)
    {
        return ParseExtensions.extractAlphaNumericDash(header);
    }

    public void processRow(List<string> row)
    {
        if (header == null)
            throw new IllegalStateException("Header is required before converting to objects");
        
        IDictionary<string, Object> @object = new ExpandoObject();
        for (int i = 0; i < header.Count; i++)
        {
            String name = header[i];
            String? value = i < row.Count ? row[i].trimEmptyAsNull() : null;
            @object.Add(name, value);
        }
        
        processor.processNameValuePair(@object);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }

    public void setNextNameValuePairProcessor(INameValuePairProcessor next)
    {
        processor = next;
    }
}