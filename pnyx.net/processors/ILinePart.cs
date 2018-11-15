namespace pnyx.net.processors
{
    public interface ILinePart : ILineProcessor
    {        
        void setNext(ILineProcessor next);
    }
}