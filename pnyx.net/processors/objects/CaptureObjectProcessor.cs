using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.objects;

public class CaptureObjectProcessor<T> : IObjectProcessor
{
    public List<T> records { get; } = new();
    public bool eof { get; private set; }
    
    public Task processObject(object obj)
    {
        records.Add((T)obj);
        return Task.CompletedTask;
    }

    public Task endOfFile()
    {
        eof = true;
        return Task.CompletedTask;
    }
}