namespace pnyx.net.processors;

public interface INameValuePairPart : IPart
{
    void setNextNameValuePairProcessor(INameValuePairProcessor next);
}