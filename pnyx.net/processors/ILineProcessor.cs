using System;
using System.Threading.Tasks;

namespace pnyx.net.processors;

public interface ILineProcessor : IPart
{
    Task processLine(String line);
    Task endOfFile();
}