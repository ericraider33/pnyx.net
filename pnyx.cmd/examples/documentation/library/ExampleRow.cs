using System;
using System.Collections.Generic;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExampleRow
    {
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleRow andShim
        public static void andShim()
        {
            const String input = @"Line one,a,,c
Line two,a,b,c
Line three,,,c
"; 
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.parseCsv();
                p.shimAnd(p2 => p2.hasLine());
                p.writeStdout();
            }                        
            // outputs:
            // "Line two",a,b,c            
        }
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleRow orShim
        public static void orShim()
        {
            const String input = @"Line one,a,,c
Line two,a,b,c
Line three,,,c
"; 
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.parseCsv();
                p.hasLine();
                p.writeStdout();
            }                        
            // outputs:
            // "Line one",a,,c
            // "Line two",a,b,c
            // "Line three",,,c            
        }

        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleRow filterFunc
        public static void filterFunc()
        {
            const String input = @"Line one,KEEPER
Line two,Loser
"; 
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.parseCsv();
                p.rowFilterFunc(x => TextUtil.isUpperCase(x[1]));
                p.writeStdout();
            }                        
            // outputs:         
            // "Line one",KEEPER
        }
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleRow transformerFunc
        public static void transformerFunc()
        {
            const String input = @"Line one,KEEPER
Line two,LOSER
"; 
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.parseCsv();
                p.rowTransformerFunc(x => new List<string> {NameUtil.toTitleCase(x[1])});
                p.writeStdout();
            }                        
            // outputs:            
            // Keeper
            // Loser
        }

        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleRow printDelimiter
        public static void printDelimiter()
        {
            const String input = "col1,\"Column, zwei\""; 
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.parseCsv();
                p.printDelimiter("|");
                p.writeStdout();
            }                        
            // outputs: col1|Column, zwei
        }

        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleRow printDelimiter
        public static void print()
        {
            const String input = "Socialism,Communism,Fascism"; 
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.parseCsv();
                p.print("Failed Systems: $1, $2, and $3");
                p.writeStdout();
            }                        
            // outputs: Failed Systems: Socialism, Communism, and Fascism  
        }
    }
}