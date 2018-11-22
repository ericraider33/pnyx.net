namespace pnyx.net.processors
{
    public interface IRowPart
    {
        void setNext(IRowProcessor next);
    }
}