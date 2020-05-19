using pnyx.net.fluent;
using pnyx.net.impl.columns.discover;

namespace pncs.cmd.examples
{
    public class DiscoveryExample
    {
        public static int main()
        {
            using (Pnyx p = new Pnyx())
            {
                p.read(@"c:/dev/asclepius/prod_import/test.csv");
                p.parseCsv(hasHeader: true);
                p.rowBuffering(new Discovery());
                p.swapColumnsAndRows();
                p.writeStdout();
            }

            return 0;
        }
        
    }
}