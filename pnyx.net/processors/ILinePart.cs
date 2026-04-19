namespace pnyx.net.processors;

public interface ILinePart : IPart
{        
    void setNextLineProcessor(ILineProcessor next);
}