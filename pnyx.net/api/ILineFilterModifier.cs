namespace pnyx.net.api
{
    public interface ILineFilterModifier : IModifier
    {
        ILineFilter modifyLineFilter(ILineFilter lineFilter);
    }
}