using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.util;

namespace pnyx.cmd.examples
{
    public static class BhcProcedures
    {
        public static int main()
        {
            transform();
            columnDefs();
            return 0;
        }

        private static void transform()
        {
            using (Pnyx p = new Pnyx())
            {
                p.setSettings(outputNewline: "\n");
                
                p.read("C:/dev/asclepius/prod_import/bhc_procedures.csv");
                p.parseCsv(hasHeader: true);
                p.withColumns(p2 => p2.lineTransformer(new DateTransform
                {
                    formatSource = DateUtil.FORMAT_MDYYYY, formatDestination = DateUtil.FORMAT_ISO_8601_DATE                        
                }), 12);
                p.tee(p2 =>
                {
                    p2.removeColumns(7,8,9,10,11);                
                    p2.rowFilter(new RepeatFilter());
                    p2.write("C:/dev/asclepius/prod_import/bhc_procedure_base.csv");
                });
                p.removeColumns(3,4,5,6);
                p.write("C:/dev/asclepius/prod_import/bhc_procedure_diagnosis.csv");
                p.process();
            }
        }
        
        private static void columnDefs()
        {
            using (Pnyx p = new Pnyx())
            {
                p.read("C:/dev/asclepius/prod_import/bhc_procedures.csv");
                p.parseCsv();
                p.columnDefinition(maxWidth: true, hasHeaderRow: true, minWidth: true, nullable: true);
                p.swapColumnsAndRows();
                p.writeStdout();
                p.process();
            }
        }
    }
}