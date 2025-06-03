using System.Collections.Generic;

namespace pnyx.net.processors.lines;

public class CaptureLineProcessor : ILineProcessor
{
    public List<string> records { get; } = new();
    public bool eof { get; private set; }
    
    public void processLine(string line)
    {
        records.Add(line);
    }

    public void endOfFile()
    {
        eof = true;
    }
}