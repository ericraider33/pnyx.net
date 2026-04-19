using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class RowToObjectProcessor : IRowProcessor, IObjectPart
{
    public IObjectConverterFromRow converter { get; }
    public IObjectProcessor? processor { get; private set; }
    private List<string>? header;

    public RowToObjectProcessor(IObjectConverterFromRow converter)
    {
        this.converter = converter;
    }

    public Task rowHeader(List<string> header_)
    {
        header = header_;
        return Task.CompletedTask;
    }

    public async Task processRow(List<string?> row)
    {
        Object obj = converter.rowToObject(row, header);
        await processor!.processObject(obj);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextObjectProcessor(IObjectProcessor next)
    {
        processor = next;
    }
}