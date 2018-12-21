using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd.code
{
    public class BaseGlobals
    {
        public readonly StreamInformation streamInformation;
        public readonly Settings settings;

        public BaseGlobals(StreamInformation streamInformation, Settings settings)
        {
            this.streamInformation = streamInformation;
            this.settings = settings;
        }
    }
}