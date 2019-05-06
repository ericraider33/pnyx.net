using System;
using System.Collections.Generic;
using pnyx.net.api;
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
        
        private class DateFixer : IRowTransformer
        {
            private String lastPatient = "";
            private String lastDate = "";
            public List<string> transformHeader(List<string> header)
            {
                return header;
            }

            public List<string> transformRow(List<string> row)
            {
                if (row.Count < 12)
                    return row;
                
                String patientAccountNumber = row[1];
                if (lastPatient == patientAccountNumber)
                {
                    row[11] = lastDate;                        // forces all dates to match first record
                }
                else
                {
                    lastPatient = patientAccountNumber;
                    lastDate = row[11];
                }
                
                return row;
            }
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
                p.rowTransformer(new DateFixer());
                p.tee(p2 =>
                {
                    p2.removeColumns(7,8,9,10,11);    
                    p2.withColumns(p3 => p3.rowFilter(new RepeatFilter()), 1,2,7);
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