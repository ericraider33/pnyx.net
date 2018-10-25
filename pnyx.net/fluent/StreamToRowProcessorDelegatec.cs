using System.IO;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.fluent
{
    public delegate IProcessor StreamToRowProcessorDelegate(StreamInformation streamInformation, Stream stream, IRowProcessor rowProcessor);   
}