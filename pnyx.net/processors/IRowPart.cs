namespace pnyx.net.processors
{
    public interface IRowPart
    {
        void setNextRowProcessor(IRowProcessor next);
    }
}