using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.lines;

public class CaptureLineProcessor : ILineProcessor
{
    public List<string> records { get; } = new();
    public bool eof { get; private set; }
    
    public Task processLine(string line)
    {
        records.Add(line);
        return Task.CompletedTask;
    }

    public Task endOfFile()
    {
        eof = true;
        return Task.CompletedTask;
    }
}