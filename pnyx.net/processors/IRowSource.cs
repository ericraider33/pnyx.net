using System.IO;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.processors
{
    public interface IRowSource : IProcessor
    {
        IRowConverter getRowConverter();
        void setSource(StreamInformation streamInformation, Stream stream, IRowProcessor rowProcessor);
    }
}