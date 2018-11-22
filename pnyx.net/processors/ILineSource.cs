using pnyx.net.api;

namespace pnyx.net.processors
{
    public interface ILineSource
    {
        IStreamFactory streamFactory { get; }
    }
}