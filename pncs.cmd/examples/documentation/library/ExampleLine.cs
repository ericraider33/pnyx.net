using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using pnyx.net.api;
using pnyx.net.fluent;
using pnyx.net.processors;
using pnyx.net.processors.dest;
using pnyx.net.util;

namespace pncs.cmd.examples.documentation.library
{
    public class ExampleLine
    {
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine not
        public static void not()
        {
            const String input = @"Line one: house
Line two: cat, dog
Line three: separation of economics and state
"; 
            using (Pnyx p = new Pnyx())
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
        public static void or()
        {
            const String input = @"Line one: house
Line two: cat, dog
Line three: separation of economics and state
"; 
            using (Pnyx p = new Pnyx())
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
        public static void filterFunc()
        {
            const String input = @"Text1with0numbers
log3message
Oliver Twist
"; 
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.lineFilter(line =>
                {
                    String numbers = ParseExtensions.extractNumeric(line);
                    return numbers.Length > 0 && int.Parse(numbers) > 5;
                });
                p.writeStdout();
            }                        
            // outputs:
            // Text1with0numbers                        
        }
        
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine transformerFunc
        public static void transformerFunc()
        {
            const String input = @"Text1with0numbers
log3message
Oliver Twist
";
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.lineTransformer(line =>
                {
                    String numbers = ParseExtensions.extractNumeric(line);
                    return numbers;
                });
                p.writeStdout();
            }                        
            // outputs:
            // 10                        
        }
        
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine parseDelimiter
        public static void parseDelimiter()
        {
            const String input = "a|b|c|d|e|f|g";
            using (Pnyx p = new Pnyx())
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
            public List<string> lineToRow(string line)
            {
                Tuple<String,String> pair = line.splitAt(":=");
                return new List<String> { pair.Item1.Trim(), pair.Item2.Trim() };
            }

            public string rowToLine(List<string> row)
            {
                return String.Format("{0} := {1}", row[0], String.Concat(row.Skip(1)));
            }

            public IRowProcessor buildRowDestination(StreamInformation streamInformation, Stream stream)
            {
                return null; // return 'null' if standard line parser (StreamToLineProcessor) is acceptable 
            }
        }
        
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine rowConverter
        public static void rowConverter()
        {
            const String input = "set x := (set == 0 ? 0 : 100 / set)";
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.lineToRow(new ExampleRowConverter());
                p.withColumns(p2 => p2.sed("set[ ]*", "var ", "i"), 1); // replace set from first column
                p.writeStdout();                                        // internally converts back to line
            }                        
            // outputs: var x := (set == 0 ? 0 : 100 / set)
        }
                
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine embeddedNewlineParseCsv
        public static void embeddedNewlineParseCsv()
        {
            const String input = "a,\"Long\nText\n\"";
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);          // StreamToLineProcessor
                p.print("$0");                // forces line state
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
        public static void embeddedNewlineAsCsv()
        {
            const String input = "a,\"Long\nText\n\"";
            using (Pnyx p = new Pnyx())
            {
                p.asCsv(p2 => p.readString(input)); // CsvStreamToRowProcessor 
                p.selectColumns(2,1);
                p.writeStdout();     
            }                        
            // outputs: 
            // "Long\nText\n",a
        }     
        
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleLine embeddedNewlineParseCsv
        public static void embeddedNewlineAutoAsCsv()
        {
            const String input = "a,\"Long\nText\n\"";
            using (Pnyx p = new Pnyx())
            {
                p.readString(input); // CsvStreamToRowProcessor (auto-wired)
                p.parseCsv(strict: false);
                p.selectColumns(2,1);
                p.writeStdout();     
            }                        
            // "Long\nText\n",a
        }        
    }
}