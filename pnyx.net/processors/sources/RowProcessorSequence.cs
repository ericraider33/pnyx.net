using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.processors.sources;

public class RowProcessorSequence : IRowPart, IProcessor
{
    public List<IProcessor> processors { get; } = new();
    public IRowProcessor? processor { get; private set; }

    private int index;
    private readonly StreamInformation streamInformation;

    public RowProcessorSequence(StreamInformation streamInformation)
    {
        this.streamInformation = streamInformation;
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        this.processor = next;
    }

    public async Task process()
    {
        RowProcessorCollector collector = new RowProcessorCollector(processor!);
        foreach (IProcessor part in processors)
        {
            if (!(part is IRowPart))
                throw new IllegalStateException("Processor must implement IRowPart");
            
            ((IRowPart)part).setNextRowProcessor(collector);
        }
            
        while (index < processors.Count && streamInformation.active)
        {
            IProcessor current = processors[index];                
            await current.process();                
            index++;
        }
            
        await processor!.endOfFile();
    }

    private class RowProcessorCollector : IRowProcessor
    {
        private readonly IRowProcessor next;
        private bool hasRowHeader;

        public RowProcessorCollector(IRowProcessor next)
        {
            this.next = next;
        }

        public async Task rowHeader(List<String> header)
        {
            if (hasRowHeader)
                return;

            await next.rowHeader(header);
            hasRowHeader = true;
        }

        public async Task processRow(List<String?> row)
        {
            await next.processRow(row);
        }

        public Task endOfFile()
        {
            return Task.CompletedTask;
        }
    }
}