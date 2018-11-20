namespace pnyx.net.api
{
    public interface IRowTransformerModifer : IModifier
    {
        IRowTransformer modifyRowTransformer(IRowTransformer rowTransformer);
    }
}