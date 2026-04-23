using System;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.fluent;
using pnyx.net.processors;
using pnyx.net.util;

namespace pncs.cmd.examples.documentation.library;

public class ExampleOutput
{
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleOutput specificEncoding
    public static async Task specificEncoding()
    {
        await using (Pnyx p = new Pnyx())
        {
            p.setSettings(outputEncoding: Encoding.UTF8);
            p.read("a\nb\nc");
            p.write("myfile.txt");
        }                        
        // Writes a UTF-8 file regardless of source
    }

    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleOutput newlines
    public static async Task newlines()
    {
        const String input = "1\r\n2\r\n3\r\n4";
        await using (Pnyx p = new Pnyx())
        {
            p.setSettings(outputNewline: StreamInformation.NEWLINE_UNIX);
            p.readString(input);               
            p.writeStdout();
        }     
        // outputs: converts Windows or Mac to Unix newlines
    }
        
    public class CustomEndLine : ILineProcessor
    {        
        public Task processLine(string line)
        {
            // write to database
            return Task.CompletedTask;
        }

        public Task endOfFile()
        {
            // commit changes
            return Task.CompletedTask;
        }
    }
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleOutput setEndLine
    public static async Task setEndLine()
    {
        CustomEndLine processor = new CustomEndLine();
        await using (Pnyx p = new Pnyx())
        {
            p.readString("a\nb\nc");                
            p.endLine(processor);
        }
    }                
}