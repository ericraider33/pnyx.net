using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.fluent;
using pnyx.net.impl.csv;
using pnyx.net.processors;
using pnyx.net.util;

namespace pncs.cmd.examples.documentation.library;

public class ExampleInput
{
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleInput specificEncoding
    public static async Task specificEncoding()
    {
        await using (Pnyx p = new Pnyx())
        {
            p.setSettings(defaultEncoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
            p.read("myfile.txt");
            p.writeStdout();
        }                        
        // Reads a UTF-8 file regardless of BOM (or lack thereof)
    }

    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleInput newlines
    public static async Task newlines()
    {
        const String input = "1\n2\r\n3\r4";
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.writeStdout();
        }     
        // outputs: 1\n2\n3\n4
    }
        
    public class CustomRowSource : IProcessor, IRowPart
    {
        private readonly StreamInformation streamInformation;
        private int countDown;
        private IRowProcessor? processor;

        public CustomRowSource(StreamInformation streamInformation, int countDown)
        {
            this.streamInformation = streamInformation;
            this.countDown = countDown;
        }

        public async Task process()
        {
            DateTime date = new DateTime(2013, 4, 2);
            while (countDown >= 0 && streamInformation.active)
            {
                String columnA = countDown.ToString();
                String columnB = date.ToString(DateUtil.FORMAT_MDYYYY);
                List<String?> row = new List<String?> { columnA, columnB };
                    
                await processor!.processRow(row);
                    
                countDown--;
                date = date.AddDays(-7);
            }
                
            await processor!.endOfFile();  // flushes buffering and output 
        }

        public void setNextRowProcessor(IRowProcessor next)
        {
            processor = next;
        }
    }
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleInput rowPart
    public static async Task rowPart()
    {
        await using (Pnyx p = new Pnyx())
        {
            p.readRow(new CustomRowSource(p.streamInformation, 40), new CsvRowConverter());
            p.head(3);
            p.writeStdout();
        }                        
        // outputs:
        // 40,4/2/2013
        // 39,3/26/2013
        // 38,3/19/2013
    }
        
}