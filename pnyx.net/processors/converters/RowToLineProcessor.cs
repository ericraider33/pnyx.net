using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.processors.converters;

public class RowToLineProcessor : IRowProcessor, ILinePart
{
    public IRowConverter rowConverter { get; }
    public ILineProcessor? processor { get; private set; }
    
    public RowToLineProcessor(IRowConverter rowConverter)
    {
        this.rowConverter = rowConverter;
    }

    public async Task rowHeader(List<String> header)
    {
        String line = rowConverter.rowToLine(header.toRow());
        await processor!.processLine(line);
    }

    public async Task processRow(List<String?> row)
    {
        String line = rowConverter.rowToLine(row);
        await processor!.processLine(line);
    }
    
    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        processor = next;
    }
}