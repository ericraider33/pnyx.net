namespace pnyx.net.api
{
    public interface IStreamFactoryWrapper : IModifier
    {
        IStreamFactory wrapStreamFactory(IStreamFactory source);
    }
}