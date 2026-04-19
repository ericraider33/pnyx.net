using System.Threading.Tasks;
using pnyx.net.fluent;
using pnyx.net.impl.columns.discover;

namespace pncs.cmd.examples;

public class DiscoveryExample
{
    public static async Task<int> main()
    {
        await using (Pnyx p = new Pnyx())
        {
            p.read(@"c:/dev/pnyx/examples/test.csv");
            p.parseCsv(hasHeader: true);
            p.rowBuffering(new Discovery());
            p.swapColumnsAndRows();
            p.writeStdout();
        }

        return 0;
    }
        
}