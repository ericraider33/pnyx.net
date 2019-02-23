using System;
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
                p.streamInformation.setNewLine(NewLineEnum.Unix);
                p.read(@"c:/dev/asclepius/prod_import/American Academy of Private Physicians.csv");
                p.parseCsv(hasHeader: true);
                p.hasColumns(true, 2);
                p.rowTransformerFunc(row =>
                {
                    row[3] = TextUtil.extractAlpha(row[3]);            // removes periods from title
                    return row;
                });                
                p.rowTransformerFunc(row =>
                {
                    row[7] = ZipUtil.parseZipCode(row[7], true);
                    return row;
                });
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
                p.rowTransformerFunc(row =>
                {
                    String firstName = row[1];
                    String lastName = row[2];

                    firstName = firstName.Replace(",", " ");
                    lastName = lastName.Replace(",", " ");

                    String wholeName = firstName + " " + lastName;
                    Name name = NameUtil.parseFullName(wholeName);

                    row[1] = name.firstName;
                    row[2] = name.lastName;

                    row = RowUtil.insertColumns(row, 4, name.suffix);
                    row = RowUtil.insertColumns(row, 3, name.middleName);
                    
                    return row;
                });
                p.widthColumns(13);
                p.headerNames("Credentials", "FirstName", "MiddleName", "LastName", "Suffix", "Title", "StreetAddress", "City", "State", "ZipCode", "Phone", "Email", "CompanyName");
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