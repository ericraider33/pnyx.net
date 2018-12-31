using System;
using System.Collections.Generic;

namespace pnyx.net.processors
{
    public interface IRowProcessor
    {
        void rowHeader(List<String> header);
        void processRow(List<String> row);
        void endOfFile();
    }
}