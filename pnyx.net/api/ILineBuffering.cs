using System;
using System.Collections.Generic;

namespace pnyx.net.api
{
    public interface ILineBuffering
    {
        List<String> bufferingLine(String line);
        List<String> endOfFile();
    }
}