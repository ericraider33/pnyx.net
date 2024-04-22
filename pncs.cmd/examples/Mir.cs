using System.Linq;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pncs.cmd.examples
{
    public class Mir
    {
        public static int fix()
        {
            using (Pnyx p = new Pnyx())
            {
                p.setSettings(outputNewline: "\n");
                
                p.read("C:/dev/asclepius/prod_import/mirPatients.csv");
                p.parseCsv(hasHeader: false);
                p.widthColumns(10, null);
                p.rowTransformer(x => new System.Collections.Generic.List<string> { x[1], x[4], x[5], x[9]??x[8]??x[7]??x[6]??x[5]??x[4] });
                p.rowTransformer(row =>
                {
                    var fullName = row[0];

                    var name = pnyx.net.util.NameUtil.parseFullName(fullName);
                    if (name == null)
                        return null;

                    return pnyx.net.util.RowUtil.replaceColumn(row, 1, name.firstName, name.lastName);
                });
                p.rowTransformer(x =>
                {
                    x[2] = PhoneUtil.parsePhone(x[2]);
                    x[3] = PhoneUtil.parsePhone(x[3]);
                    return x.ToList();
                });
                
                p.tee(px => px.writeStdout());
                p.write("C:/dev/asclepius/prod_import/mirPatients.out.csv");
                p.process();
            }
            
            return 0;
        }
    }
}