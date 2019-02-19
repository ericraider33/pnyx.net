using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd.examples
{
    public class ColumnDefinitionExample
    {
        public static int main()
        {
            using (Pnyx p = new Pnyx())
            {
                p.read(@"c:/dev/asclepius/prod_import/American Academy of Private Physicians.csv");
                p.parseCsv(hasHeader: true);
                p.hasColumns(true, 2);
                p.rowTransformerFunc(row =>
                {
                    row[8] = PhoneUtil.parsePhone(row[8]);
                    return row;
                });
                p.rowTransformerFunc(row =>
                {
                    row[9] = EmailUtil.validateAndRepair(row[9]);
                    return row;
                });
                p.write(@"c:/dev/asclepius/prod_import/aapp.csv");
            }

            using (Pnyx p = new Pnyx())
            {
                p.read(@"c:/dev/asclepius/prod_import/aapp.csv");
                p.parseCsv();
                p.columnDefinition(hasHeaderRow: true, maxWidth: true, nullable: true);
                p.swapColumnsAndRows();
                p.writeStdout();
            }

            return 0;
        }
    }
}