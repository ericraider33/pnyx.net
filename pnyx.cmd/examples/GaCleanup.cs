using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd.examples
{
    public class GaCleanup
    {
        public static int main()
        {
            using (Pnyx p = new Pnyx())
            {
                p.read(@"c:/dev/asclepius/prod_import/events.csv");
                p.grep("CL/C/MonitoringDashboard/");
                p.sed("MonitoringDashboard[/][^.]+[.]", "MonitoringDashboard.");
                p.sed("CL/C/MonitoringDashboard.", "");
                p.parseCsv();
                p.withColumns(p2 => { p2.sed(",", "", "g"); }, 2, 3);
                p.lineTransformerFunc(line => TextUtil.enocdeSqlValue(line));
                p.print("insert into `groupit` values($1,$2);");
                p.write(@"c:/dev/asclepius/prod_import/events.sql");
                
            }

            return 0;
        }
    }
}