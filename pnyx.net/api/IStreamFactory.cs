using System.IO;

namespace pnyx.net.api
{
    public interface IStreamFactory
    {
        Stream openStream();
        void closeStream();
    }
}