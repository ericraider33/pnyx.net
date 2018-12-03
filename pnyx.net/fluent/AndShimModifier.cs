using pnyx.net.api;
using pnyx.net.shims;

namespace pnyx.net.fluent
{
    public class AndShimModifier : IRowFilterShimModifier, IRowTransformerShimModifier
    {
        public IRowFilter shimLineFilter(ILineFilter lineFilter)
        {
            return new RowFilterShimAnd { lineFilter = lineFilter };
        }

        public IRowTransformer shimLineTransformer(ILineTransformer lineTransformer)
        {
            return new RowTransformerShimAnd { lineTransformer = lineTransformer };
        }
    }
}