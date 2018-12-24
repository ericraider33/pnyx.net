using System;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.util;

namespace pnyx.cmd.examples
{
    public static class BhcDischarge
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
                p.read("C:/dev/asclepius/prod_import/BHC Patients from 1-1-15 thru 10-31-2018.csv");
                p.parseCsv();
                p.withColumns(p2 => p2.lineTransformer(new DateTransform
                {
                    formatSource = DateUtil.FORMAT_MDYYYY, formatDestination = DateUtil.FORMAT_ISO_8601_DATE                        
                }), 4,5,6);
                p.rowTransformerFunc(row =>
                {
                    String fullName = row[2];

                    Name name = NameUtil.parseFullName(fullName);
                    if (name == null)
                        return null;

                    // Expands name into 4 columns
                    row = RowUtil.replaceColumn(row, 3, name.firstName, name.middleName, name.lastName, name.suffix);                    
                    return row;
                });
                p.tee(p2 =>
                {
                    p2.removeColumns(7+3, 8+3, 9+3);                // plus 3 from name split above
                    p2.rowFilter(new RepeatFilter());
                    p2.write("C:/dev/asclepius/prod_import/bhc_discharges.csv");
                });
                p.widthColumns(9+3);                                  // plus 3 from name split above
                p.write("C:/dev/asclepius/prod_import/bhc_discharges_diagnosis.csv");
                p.process();
            }
        }
        
        private static void columnDefs()
        {
            using (Pnyx p = new Pnyx())
            {
                p.read("C:/dev/asclepius/prod_import/bhc_discharges.csv");
                p.parseCsv();
                p.columnDefinition(maxWidth: true, hasHeaderRow: true, minWidth: true, nullable: true);
                p.swapColumnsAndRows();
                p.writeStdout();
                p.process();
            }

            using (Pnyx p = new Pnyx())
            {
                p.read("C:/dev/asclepius/prod_import/bhc_discharges_diagnosis.csv");
                p.parseCsv();
                p.columnDefinition(maxWidth: true, hasHeaderRow: true, minWidth: true, nullable: true);
                p.swapColumnsAndRows();
                p.writeStdout();
                p.process();
            }
        }        
        
    }
}