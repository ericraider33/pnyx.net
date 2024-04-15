using System;

namespace pnyx.net.processors;

public interface IObjectProcessor
{
    void processObject(Object obj);
    void endOfFile();
}