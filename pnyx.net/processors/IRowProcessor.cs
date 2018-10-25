using System;

namespace pnyx.net.processors
{
    public interface IRowProcessor
    {
        void processRow(String[] row);
        void endOfFile();
    }
}