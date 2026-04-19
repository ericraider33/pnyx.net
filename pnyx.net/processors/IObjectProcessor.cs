using System;
using System.Threading.Tasks;

namespace pnyx.net.processors;

public interface IObjectProcessor : IPart
{
    Task processObject(Object obj);
    Task endOfFile();
}