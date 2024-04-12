namespace pnyx.net.processors;

public interface INameValuePairPart
{
    void setNextNameValuePairProcessor(INameValuePairProcessor next);
}