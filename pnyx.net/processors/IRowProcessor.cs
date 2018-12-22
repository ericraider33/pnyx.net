using System;

namespace pnyx.net.processors
{
    public interface IRowProcessor
    {
        void rowHeader(String[] header);
        void processRow(String[] row);
        void endOfFile();
    }
}