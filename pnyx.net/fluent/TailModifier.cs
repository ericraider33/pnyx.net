using pnyx.net.api;
using pnyx.net.processors.sources;
using pnyx.net.util;

namespace pnyx.net.fluent
{
    public class TailModifier : IStreamFactoryWrapper
    {
        public int limit;
        private readonly StreamInformation streamInformation;

        public TailModifier(int limit, StreamInformation streamInformation)
        {
            this.limit = limit;
            this.streamInformation = streamInformation;
        }

        public IStreamFactory wrapStreamFactory(IStreamFactory source)
        {
            return new TailStreamFactory(streamInformation, source, limit);
        }
    }
}