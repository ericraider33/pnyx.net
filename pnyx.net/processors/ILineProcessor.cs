using System;

namespace pnyx.net.processors
{
    public interface ILineProcessor
    {
        void processLine(String line);
        void endOfFile();
    }
}