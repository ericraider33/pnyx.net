using pnyx.net.api;

namespace pnyx.net.processors;

public interface ILineSource : IPart
{
    IStreamFactory streamFactory { get; }
}