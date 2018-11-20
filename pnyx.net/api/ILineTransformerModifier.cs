namespace pnyx.net.api
{
    public interface ILineTransformerModifier : IModifier
    {
        ILineTransformer modifyLineTransformer(ILineTransformer lineTransformer);
    }
}