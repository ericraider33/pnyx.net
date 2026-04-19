using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.processors.converters;

public class RowToNameValuePairProcessor : IRowProcessor, INameValuePairPart
{
    private List<string>? header;
    public INameValuePairProcessor? processor;
    
    public Task rowHeader(List<string> rowHeader)
    {
        header = rowHeader.Select(cleanUpHeader).ToList();
        return Task.CompletedTask;
    }

    private String cleanUpHeader(String headerName)
    {
        return ParseExtensions.extractAlphaNumericDash(headerName);
    }

    public async Task processRow(List<string?> row)
    {
        if (header == null)
            throw new IllegalStateException("Header is required before converting to objects");
        
        IDictionary<string, Object?> @object = new ExpandoObject();
        for (int i = 0; i < header.Count; i++)
        {
            String name = header[i];
            String? value = i < row.Count ? row[i].trimEmptyAsNull() : null;
            @object.Add(name, value);
        }
        
        await processor!.processNameValuePair(@object);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextNameValuePairProcessor(INameValuePairProcessor next)
    {
        processor = next;
    }
}