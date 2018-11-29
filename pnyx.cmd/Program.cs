using System;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd
{
    class Program
    {
        static void Main(string[] args)
        {            
            using (Pnyx p = new Pnyx())
            {                
                p.read("G:/My Drive/Sales/database/emails/names.csv");
                p.parseCsv();
                p.rowTransformer(row =>
                {
                    row = RowHelper.fixWidth(row, 2);
                    String fullName = row[0];
                    
                    if (row[0].Contains(","))
                    {
                        Tuple<String, String> nameTitle = TextHelper.splitAt(row[0], ",");
                        fullName = nameTitle.Item1.Trim();
                        row[1] = nameTitle.Item2.Trim();
                    }
                    
                    fullName = fullName.Replace(".", "");
                    
                    Tuple<String, String, String> name = NameHelper.parseFullName(fullName);
                    return RowHelper.insertColumns(row, 2, name.Item1, name.Item3);
                });
                p.writeCsv("G:/My Drive/Sales/database/emails/names2.csv");                
                p.process();
            }
            
            using (Pnyx p = new Pnyx())
            {                
                p.read("G:/My Drive/Sales/database/emails/names2.csv");
                p.parseCsv();
                p.rowTransformer(row => RowHelper.fixWidth(row, 4));
                p.lineTransformer(x => TextHelper.enocdeSqlValue(TextHelper.emptyAsNull(x)));
                p.print("update groupexecs set first=$2, last=$3, title=$4 where executivename=$1;");
                p.write("G:/My Drive/Sales/database/emails/names.sql");                
                p.process();
            }
           
            Console.WriteLine("Done");
        }
    }
}