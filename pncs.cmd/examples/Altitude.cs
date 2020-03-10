using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.util;

namespace pncs.cmd.examples
{
    public class Altitude
    {
        public static int main()
        {
            using (Pnyx p = new Pnyx())
            {
                p.read("nya.csv");
                p.parseCsv();
                p.rowTransformerFunc(row =>
                {
                    var fullName = row[1];

                    var name = pnyx.net.util.NameUtil.parseFullName(fullName);
                    if (name == null)
                        return null;

                    return pnyx.net.util.RowUtil.replaceColumn(row, 2, name.firstName, name.lastName);
                });
                p.selectColumns(2,3,5);
                p.columnTransformer(3, new pnyx.net.impl.DateTransform { formatSource = "M-d-yyyy", formatDestination = "yyyy-M-d"  });
                p.lineTransformerFunc(x => pnyx.net.util.TextUtil.enocdeSqlValue(x));
                p.print("insert into tmp_name values($1,$2,$3);");
                p.write("nya.sql");
            }

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

                    return pnyx.net.util.RowUtil.replaceColumn(row, 1, name.firstName, name.middleName, name.lastName);
                });
                p.lineTransformerFunc(x => pnyx.net.util.TextUtil.enocdeSqlValue(x));
                p.sortRow(new[] {1, 3});
                p.writeCsv(@"C:\dev\asclepius\prod_import\alt.csv");
            }

            using (Pnyx p = new Pnyx())
            {
                p.read(@"C:\dev\asclepius\prod_import\alt_names.csv");
                p.parseCsv();
                p.columnTransformer(3, new DateTransform { formatSource = DateUtil.FORMAT_MDYYYY, formatDestination = DateUtil.FORMAT_ISO_8601_DATE  });
                p.rowTransformerFunc(row =>
                {
                    for (int i = 0; i < row.Count; i++)
                        row[i] = TextUtil.enocdeSqlValue(row[i]);                    
                    return row;
                });
                p.print("insert into to_import value($1,$2,$3);");
                p.write(@"C:\dev\asclepius\prod_import\names.sql");
            }

            return 0;
        }
    }
}