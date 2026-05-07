using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.impl.columns;
using pnyx.net.impl.sed;
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
    
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleRow rowFilterColumn
    public static async Task rowFilterColumn()
    {
        const String input = @"
Line one,Atlanta,City located in the southwestern United States.
Line two,New York,The most populous city in the United States.
Line three,Tampa,Sunny and warm city on the west coast of Florida.
";
        await using Pnyx p = new Pnyx();
        p.readString(input);
        p.parseCsv();
        p.rowFilterColumn(RowConstants.B, new Grep("a"));
        p.writeStdout();
        // outputs:
        // Line one,Atlanta,City located in the southwestern United States.
        // Line three,Tampa,Sunny and warm city on the west coast of Florida.
    }
    
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleRow rowTransformerColumn
    public static async Task rowTransformerColumn()
    {
        const String input = @"
Line one,Atlanta
Line two,New York
Line three,Tampa
";
        await using Pnyx p = new Pnyx();
        p.readString(input);
        p.parseCsv();
        p.rowTransformerColumn(RowConstants.B, new SedReplace(".*", @"City of \0"));
        p.writeStdout();
        // outputs:
        // Line one,City of Atlanta
        // Line two,City of New York
        // Line three,City of Tampa
    }

    
    // pnyx -e=documentation ExampleRow withColumnsFilter
    public static async Task withColumnsFilter()
    {
        const String input = @"
Line one,Atlanta,City located in the southwestern United States.
Line two,New York,The most populous city in the United States.
Line three,Tampa,Sunny and warm city on the west coast of Florida.
";
        await using Pnyx p = new Pnyx();
        p.readString(input);
        p.parseCsv();
        p.withColumns(p2 => p2.grep("a"), RowConstants.B);
        p.writeStdout();
        // outputs:
        // Line one,Atlanta,City located in the southwestern United States.
        // Line three,Tampa,Sunny and warm city on the west coast of Florida.
    }
    
    // pnyx -e=documentation ExampleRow withColumnsTransformer
    public static async Task withColumnsTransformer()
    {
        const String input = @"
Line one,Atlanta
Line two,New York
Line three,Tampa
";
        await using Pnyx p = new Pnyx();
        p.readString(input);
        p.parseCsv();
        p.withColumns(p2 => p2.sed(".*", @"City of \0"), RowConstants.B);
        p.writeStdout();
        // outputs:
        // Line one,City of Atlanta
        // Line two,City of New York
        // Line three,City of Tampa
    }
    
}