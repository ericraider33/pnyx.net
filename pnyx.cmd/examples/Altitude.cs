using pnyx.net.fluent;

namespace pnyx.cmd.examples
{
    public class Altitude
    {
        public static int main()
        {
            using (Pnyx p = new Pnyx())
            {
                p.read(@"C:\dev\asclepius\prod_import\alt.txt");
                p.parseTab();
                p.rowTransformerFunc(row =>
                {
                    var fullName = row[0];

                    var name = pnyx.net.util.NameUtil.parseFullName(fullName);
                    if (name == null)
                        return null;

                    return pnyx.net.util.RowUtil.replaceColumn(row, 1, name.firstName, name.middleName ?? "", name.lastName);
                });
                p.sortRow(new[] {1, 3});
                p.writeCsv(@"C:\dev\asclepius\prod_import\alt.csv");
            }

            return 0;
        }
    }
}