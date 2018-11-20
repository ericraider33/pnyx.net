namespace pnyx.net.api
{
    public interface IRowFilterModifier : IModifier
    {
        IRowFilter modifyRowFilter(IRowFilter rowFilter);
    }
}