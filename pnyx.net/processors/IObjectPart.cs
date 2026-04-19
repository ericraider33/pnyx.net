namespace pnyx.net.processors;

public interface IObjectPart : IPart
{
    void setNextObjectProcessor(IObjectProcessor next);
}