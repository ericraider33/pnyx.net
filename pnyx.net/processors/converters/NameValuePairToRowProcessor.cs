using System.Collections.Generic;
using System.Linq;

namespace pnyx.net.processors.converters;

public class NameValuePairToRowProcessor : INameValuePairProcessor, IRowPart
{
    public IRowProcessor next { get; private set; }
    private List<string> header;
    private bool firstRow = true;
    
    public NameValuePairToRowProcessor()
    {
        header = null;
    }

    public NameValuePairToRowProcessor(List<string> header)
    {
        this.header = header;
    }
    
    public void processNameValuePair(IDictionary<string, object> record)
    {
        if (firstRow)
        {
            if (header == null)
                header = record.Keys.Order().ToList();

            next.rowHeader(header);
            firstRow = false;
        }
        
        List<string> row = new List<string>(header.Count);
        foreach (string key in header)
        {
            if (record.TryGetValue(key, out object value))
            {
                row.Add(value?.ToString());
            }
            else
            {
                row.Add(null);
            }
        }
        
        next.processRow(row);
    }

    public void endOfFile()
    {
        next.endOfFile();
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        this.next = next;
    }
}