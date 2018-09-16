namespace pnyx.net.processors
{
    public interface ILineProcessorPlug
    {
        ILineProcessor getLineProcessor();
        void setLineProcess(ILineProcessor processor);
    }
}