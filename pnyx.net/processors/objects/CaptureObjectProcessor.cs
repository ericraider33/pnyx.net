using System.Collections.Generic;

namespace pnyx.net.processors.objects;

public class CaptureObjectProcessor<T> : IObjectProcessor
{
    public List<T> records { get; } = new();
    public bool eof { get; private set; }
    
    public void processObject(object obj)
    {
        records.Add((T)obj);
    }

    public void endOfFile()
    {
        eof = true;
    }
}