namespace pnyx.net.processors
{
    public interface IRowPart : IRowProcessor
    {
        void setNext(IRowProcessor next);
    }
}