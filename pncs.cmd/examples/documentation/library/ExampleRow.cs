using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pncs.cmd.examples.documentation.library;

public class ExampleRow
{
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleRow orShim
    public static async Task orShim()
    {
        const String input = @"Line one,a,,c
Line two,a,b,c
Line three,,,c
"; 
        await using (Pnyx p = new Pnyx())
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
    
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleRow andShim
    public static async Task andShim()
    {
        const String input = @"Line one,a,,c
Line two,a,b,c
Line three,,,c
"; 
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.parseCsv();
            p.shimAnd(p2 => p2.hasLine());
            p.writeStdout();
        }                        
        // outputs:
        // "Line two",a,b,c            
    }

    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleRow filterFunc
    public static async Task filterFunc()
    {
        const String input = @"Line one,KEEPER
Line two,Loser
"; 
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.parseCsv();
            p.rowFilter(x => x[1].isUpperCase());
            p.writeStdout();
        }                        
        // outputs:         
        // "Line one",KEEPER
    }
        
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleRow transformerFunc
    public static async Task transformerFunc()
    {
        const String input = @"Line one,KEEPER
Line two,LOSER
"; 
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.parseCsv();
            p.rowTransformer(x => new List<string?> {x[1].toTitleCase()});
            p.writeStdout();
        }                        
        // outputs:            
        // Keeper
        // Loser
    }

    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleRow printDelimiter
    public static async Task print()
    {
        const String input = "Socialism,Communism,Fascism"; 
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.parseCsv();
            p.print("Failed Systems: $1, $2, and $3");
            p.writeStdout();
        }                        
        // outputs: Failed Systems: Socialism, Communism, and Fascism  
    }

    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleRow printDelimiter
    public static async Task printDelimiter()
    {
        const String input = "col1,\"Column, zwei\""; 
        await using (Pnyx p = new Pnyx())
        {
            p.readString(input);
            p.parseCsv();
            p.printDelimiter("|");
            p.writeStdout();
        }                        
        // outputs: col1|Column, zwei
    }
}