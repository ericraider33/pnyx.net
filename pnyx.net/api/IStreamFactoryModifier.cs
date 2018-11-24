using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.api
{
    public interface IStreamFactoryModifier : IModifier
    {
        IProcessor buildProcessor(StreamInformation streamInformation, IStreamFactory streamFactory);
    }
}