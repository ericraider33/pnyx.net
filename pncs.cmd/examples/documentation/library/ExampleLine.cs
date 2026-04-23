using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using pnyx.net.api;
using pnyx.net.fluent;
using pnyx.net.processors;
using pnyx.net.util;

namespace pncs.cmd.examples.documentation.library;

public class ExampleLine
{
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine not
    public static async Task not()
    {
        const string input = @"Line one: house
Line two: cat, dog
Line three: separation of economics and state
"; 
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.not(p2 => p2.grep("state"));
            p.writeStdout();
        }                        
        // outputs:
        // Line one: house
        // Line two: cat, dog            
    }
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine or
    public static async Task or()
    {
        const string input = @"Line one: house
Line two: cat, dog
Line three: separation of economics and state
"; 
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.or(p2 =>
            {                    
                p2.grep("cat");
                p2.egrep("Line one.*");
            });
            p.writeStdout();
        }                        
        // outputs:
        // Line one: house
        // Line two: cat, dog            
    }
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine filterFunc
    public static async Task filterFunc()
    {
        const string input = @"Text1with0numbers
log3message
Oliver Twist
"; 
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.lineFilter(line =>
            {
                string numbers = ParseExtensions.extractNumeric(line);
                return numbers.Length > 0 && int.Parse(numbers) > 5;
            });
            p.writeStdout();
        }                        
        // outputs:
        // Text1with0numbers                        
    }
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine transformerFunc
    public static async Task transformerFunc()
    {
        const string input = @"Text1with0numbers
log3message
Oliver Twist
";
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.lineTransformer(line =>
            {
                string numbers = ParseExtensions.extractNumeric(line);
                return numbers;
            });
            p.hasLine();
            p.writeStdout();
        }                        
        // outputs:
        // 10           
        // 3
        //
    }
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine parseDelimiter
    public static async Task parseDelimiter()
    {
        const string input = "a|b|c|d|e|f|g";
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.sed("[aceg]", @"\0\0", "gi");     // duplicates every other char
            p.parseDelimiter("|");
            p.print("$1,$3,$5,$7|$2,$4,$6");
            p.writeStdout();
        }                        
        // outputs: aa,cc,ee,gg|b,d,f
    }
                
    public class ExampleRowConverter : IRowConverter
    {
        public List<string?> lineToRow(string line)
        {
            Tuple<string,string>? pair = line.splitAt(":=");
            if (pair == null)
                return new List<string?>();
            
            return [pair.Item1.Trim(), pair.Item2.Trim()];
        }

        public string rowToLine(List<string?> row)
        {
            return $"{row[0]} := {string.Concat(row.Skip(1))}";
        }

        public IRowProcessor? buildRowDestination(StreamInformation streamInformation, Stream stream)
        {
            // Returns custom stream processor if desired, for parsing newlines, as is needed with 
            // a CSV parser that properly handles embedded newlines.
            // Otherwise, return 'null' when standard line parser (StreamToLineProcessor) is acceptable
            return null;  
        }
    }
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine rowConverter
    public static async Task rowConverter()
    {
        const string input = "set x := (set == 0 ? 0 : 100 / set)";
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.lineToRow(new ExampleRowConverter());
            p.withColumns(p2 => p2.sed("set[ ]*", "var ", "i"), 1); // replace set from first column
            p.writeStdout();                                        // internally converts back to line
        }                        
        // outputs: var x := (set == 0 ? 0 : 100 / set)
    }
                
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine embeddedNewlineParseCsv
    public static async Task embeddedNewlineParseCsv()
    {
        const string input = "a,\"Long\nText\n\"";
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);          // StreamToLineProcessor
            p.print("$0");                // forces line-state to prevent auto-wire to row-state (see below)
            p.parseCsv(strict: false);
            p.selectColumns(2,1);
            p.writeStdout();     
        }                        
        // outputs: 
        // Long,a
        // ,Text
        // ,
    }        
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine embeddedNewlineAsCsv
    public static async Task embeddedNewlineAsCsv()
    {
        const string input = "a,\"Long\nText\n\"";
        await using Pnyx p = new Pnyx();
        p.asCsv(p2 => p2.readString(input)); // CsvStreamToRowProcessor 
        p.selectColumns(2,1);
        p.writeStdout();
        
        // outputs: 
        // "Long\nText\n",a
    }     
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine embeddedNewlineParseCsv
    public static async Task embeddedNewlineAutoAsCsv()
    {
        const string input = "a,\"Long\nText\n\"";
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input); // CsvStreamToRowProcessor (auto-wired)
            p.parseCsv(strict: false);
            p.selectColumns(2,1);
            p.writeStdout();     
        }                        
        // "Long\nText\n",a
    }        
}