namespace pnyx.net.processors
{
    public interface ILinePart
    {        
        void setNextLineProcessor(ILineProcessor next);
    }
}