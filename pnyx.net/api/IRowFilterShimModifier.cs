namespace pnyx.net.api
{
    public interface IRowFilterShimModifier : IModifier
    {
        IRowFilter shimLineFilter(ILineFilter lineFilter);
    }
}