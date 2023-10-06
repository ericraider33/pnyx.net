using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.impl.csv;

namespace pncs.cmd.examples.documentation.library
{
    public class ExamplesUtilities
    {
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExamplesUtilities csvReader
        public static void csvReader()
        {
            using (FileStream oStream = new FileStream("C:\\dev\\asclepius\\prod_import\\testing\\stlh.tmp.csv", FileMode.Create, FileAccess.Write))
            {
                using (CsvWriter writer = new CsvWriter(oStream, Encoding.UTF8))
                {
                    using (FileStream stream = new FileStream("C:\\dev\\asclepius\\prod_import\\testing\\stlh.out.csv", FileMode.Open, FileAccess.Read))
                    {
                        using (CsvReader reader = new CsvReader(stream, Encoding.UTF8))
                        {
                            reader.settings.setDefaults(strict: true); // throw errors for bad formatting
                    
                            List<String> row;
                            while ((row = reader.readRow()) != null)
                            {
                                // Process data
                                row.Add("Testing: " + DateTime.Now);
                                writer.writeRow(row);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Finished!");
        }            

        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExamplesUtilities csvReaderAsync
        public static async Task csvReaderAsync()
        {
            using (FileStream oStream = new FileStream("C:\\dev\\asclepius\\prod_import\\testing\\stlh.tmp.csv", FileMode.Create, FileAccess.Write))
            {
                using (CsvWriter writer = new CsvWriter(oStream, Encoding.UTF8))
                {
                    using (FileStream stream = new FileStream("C:\\dev\\asclepius\\prod_import\\testing\\stlh.out.csv", FileMode.Open, FileAccess.Read))
                    {
                        using (CsvReaderAsync reader = new CsvReaderAsync(stream, Encoding.UTF8))
                        {
                            reader.csvSettings.setDefaults(strict: true); // throw errors for bad formatting
                    
                            List<String> row;
                            while ((row = await reader.readRow()) != null)
                            {
                                // Process data
                                row.Add("Testing: " + DateTime.Now);
                                await writer.writeRowAsync(row);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Finished!");
        }            
        
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExamplesUtilities csvWriter
        public static void csvWriter()
        {
            using (var stream = new FileStream("o.csv", FileMode.Create, FileAccess.Write))
            {
                using (CsvWriter writer = new CsvWriter(stream, Encoding.UTF8))
                {
                    int counter = 0;
                    List<String> row = new List<String>();                        
                    for (int i = 0; i < 200; i++)
                    {
                        row.Clear();
                        for (int j = 0; j < 5; j++)
                            row.Add(counter++.ToString());

                        writer.writeRow(row);
                    }
                }
            }
        }            
        
    }
}