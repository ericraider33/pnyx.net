namespace pnyx.net.processors;

public interface IRowPart : IPart
{
    void setNextRowProcessor(IRowProcessor next);
}