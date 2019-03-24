using System;
using System.Text;
using pnyx.net.fluent;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExampleOutput
    {
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleOutput specificEncoding
        public static void specificEncoding()
        {
            using (Pnyx p = new Pnyx())
            {
                p.setSettings(outputEncoding: Encoding.UTF8);
                p.read("a\nb\nc");
                p.write("myfile.txt");
            }                        
            // Writes a UTF-8 file regardless of source
        }

        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleOutput newlines
        public static void newlines()
        {
            const String input = "1\r\n2\r\n3\r\n4";
            using (Pnyx p = new Pnyx())
            {
                p.setSettings(outputNewline: StreamInformation.NEWLINE_UNIX);
                p.readString(input);               
                p.writeStdout();
            }     
            // outputs: converts windows to unix newlines
        }
        
        public class CustomEndLine : ILineProcessor
        {        
            public void processLine(string line)
            {
                // write to databse
            }

            public void endOfFile()
            {
                // commit changes
            }
        }
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleOutput setEndLine
        public static void setEndLine()
        {
            CustomEndLine processor = new CustomEndLine();
            using (Pnyx p = new Pnyx())
            {
                p.readString("a\nb\nc");                
                p.endLine(processor);
            }
        }                
    }
}