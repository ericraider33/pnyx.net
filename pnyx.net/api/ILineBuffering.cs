using System;

namespace pnyx.net.api
{
    public interface ILineBuffering
    {
        String[] bufferingLine(String line);
        String[] endOfFile();
    }
}