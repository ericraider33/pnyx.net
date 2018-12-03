namespace pnyx.net.api
{
    public interface IRowTransformerShimModifier : IModifier
    {
        IRowTransformer shimLineTransformer(ILineTransformer lineTransformer);
    }
}