namespace pnyx.net.processors;

public interface IObjectPart
{
    void setNextObjectProcessor(IObjectProcessor next);
}