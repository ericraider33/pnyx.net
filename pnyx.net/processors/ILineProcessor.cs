using System;

namespace pnyx.net.processors
{
    public interface ILineProcessor
    {
        void process(String line);
        void endOfFile();
    }
}