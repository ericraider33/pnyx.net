namespace pnyx.net.api
{
    public interface IRowTransformerModifer
    {
        IRowTransformer modifyRowTransformer(IRowTransformer rowTransformer);
    }
}