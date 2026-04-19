using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pnyx.net.processors.converters;

public class NameValuePairToRowProcessor : INameValuePairProcessor, IRowPart
{
    public IRowProcessor? processor { get; private set; }
    private List<string>? header;
    private bool firstRow = true;
    
    public NameValuePairToRowProcessor()
    {
        header = null;
    }

    public NameValuePairToRowProcessor(List<string>? header)
    {
        this.header = header;
    }
    
    public async Task processNameValuePair(IDictionary<string, object?> record)
    {
        if (firstRow)
        {
            if (header == null)
                header = record.Keys.Order().ToList();

            await processor!.rowHeader(header);
            firstRow = false;
        }
        
        List<string?> row = new List<string?>(header!.Count);
        foreach (string key in header)
        {
            if (record.TryGetValue(key, out object? value))
            {
                row.Add(value?.ToString());
            }
            else
            {
                row.Add(null);
            }
        }
        
        await processor!.processRow(row);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextRowProcessor(IRowProcessor processor)
    {
        this.processor = processor;
    }
}