using System;
using System.Collections.Generic;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.impl.csv;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExampleInput
    {
        public class CustomRowSource : IProcessor, IRowPart
        {
            private StreamInformation streamInformation;
            private int countDown;
            private IRowProcessor next;

            public CustomRowSource(StreamInformation streamInformation, int countDown)
            {
                this.streamInformation = streamInformation;
                this.countDown = countDown;
            }

            public void process()
            {
                DateTime date = new DateTime(2013, 4, 2);
                while (countDown >= 0 && streamInformation.active)
                {
                    String columnA = countDown.ToString();
                    String columnB = date.ToString(DateUtil.FORMAT_MDYYYY);
                    List<String> row = new List<String> { columnA, columnB };
                    
                    next.processRow(row);
                    
                    countDown--;
                    date = date.AddDays(-7);
                }
                
                next.endOfFile();  // flushes buffering and output 
            }

            public void setNextRowProcessor(IRowProcessor next)
            {
                this.next = next;
            }
        }
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleInput rowPart
        public static void rowPart()
        {
            using (Pnyx p = new Pnyx())
            {
                p.startRow(new CustomRowSource(p.streamInformation, 40), new CsvRowConverter());
                p.head(3);
                p.writeStdout();
            }                        
            // outputs:
            // 40,4/2/2013
            // 39,3/26/2013
            // 38,3/19/2013
        }
        
    }
}