using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.processors.sources;

public class LineProcessorSequence : ILinePart, IProcessor
{
    public readonly List<IProcessor> processors = new List<IProcessor>();
    public ILineProcessor? processor { get; private set; }

    private int index;
    private readonly StreamInformation streamInformation;

    public LineProcessorSequence(StreamInformation streamInformation)
    {
        this.streamInformation = streamInformation;
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        this.processor = next;
    }

    public async Task process()
    {
        LineProcessorCollector collector = new LineProcessorCollector(processor!);
        foreach (IProcessor part in processors)
        {
            if (!(part is ILinePart))
                throw new IllegalStateException($"Processor {part.GetType().Name} is not a line part");
            
            ((ILinePart)part).setNextLineProcessor(collector);
        }
            
        while (index < processors.Count && streamInformation.active)
        {
            IProcessor current = processors[index];                
            await current.process();                
            index++;
        }
            
        await processor!.endOfFile();
    }

    private class LineProcessorCollector : ILineProcessor
    {
        private readonly ILineProcessor next;

        public LineProcessorCollector(ILineProcessor next)
        {
            this.next = next;
        }

        public async Task processLine(String line)
        {
            await next.processLine(line);
        }

        public Task endOfFile()
        {
            return Task.CompletedTask;
        }
    }
}