using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class LineToRowProcessor : ILineProcessor, IRowPart
{
    public bool hasHeader { get; private set; }     
    public IRowConverter rowConverter { get; }
    public IRowProcessor? processor { get; private set; }
    private int lineNumber;

    public LineToRowProcessor(IRowConverter rowConverter, bool hasHeader = false)
    {
        this.rowConverter = rowConverter;
        this.hasHeader = hasHeader;
    }

    public async Task processLine(String line)
    {
        lineNumber++;
            
        List<String?> row = rowConverter.lineToRow(line);
        if (lineNumber == 1 && hasHeader)
        {
            List<String> header = row.Select(x => x ?? "").ToList();
            await processor!.rowHeader(header);
        }
        else
            await processor!.processRow(row);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        processor = next;
    }
}