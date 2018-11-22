using pnyx.net.api;
using pnyx.net.processors;

namespace pnyx.net.fluent
{
    public class CatModifier : IModifier
    {
        public readonly LineProcessorSequence processSequence = new LineProcessorSequence();
    }
}