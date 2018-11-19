namespace pnyx.net.api
{
    public interface ILineFilterModifier
    {
        ILineFilter modifyLineFilter(ILineFilter lineFilter);
    }
}