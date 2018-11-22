using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.processors
{
    public interface IRowSource : IProcessor, IRowPart
    {
        IRowConverter getRowConverter();
        void setSource(StreamInformation streamInformation, IStreamFactory streamFactory);
    }
}