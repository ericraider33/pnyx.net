namespace pnyx.net.api
{
    public interface IRowFilterModifier
    {
        IRowFilter modifyRowFilter(IRowFilter rowFilter);
    }
}