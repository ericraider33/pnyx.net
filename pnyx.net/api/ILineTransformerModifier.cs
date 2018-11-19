namespace pnyx.net.api
{
    public interface ILineTransformerModifier
    {
        ILineTransformer modifyLineTransformer(ILineTransformer lineTransformer);
    }
}