using System.Threading.Tasks;

namespace pnyx.net.processors;

public interface IProcessor : IPart
{
    Task process();
}