using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.impl.csv;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExamplesUtilities
    {
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExamplesUtilities csvReader
        public static void csvReader()
        {
            using (FileStream stream = new FileStream("my.csv", FileMode.Open, FileAccess.Read))
            {
                using (CsvReader reader = new CsvReader(stream, Encoding.UTF8))
                {
                    reader.settings.setDefaults(strict: true); // throw errors for bad formatting
                    
                    List<String> row;
                    while ((row = reader.readRow()) != null)
                    {
                        // Process data
                        Console.WriteLine("Row has {0} column(s)", row.Count);
                    }
                }
            }
        }            
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExamplesUtilities csvWriter
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