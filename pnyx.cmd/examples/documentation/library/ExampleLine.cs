using System;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExampleLine
    {
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleLine not
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
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleLine or
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
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleLine filterFunc
        public static void filterFunc()
        {
            const String input = @"Text1with0numbers
log3message
Oliver Twist
"; 
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.lineFilterFunc(line =>
                {
                    String numbers = TextUtil.extractNumeric(line);
                    return numbers.Length > 0 && int.Parse(numbers) > 5;
                });
                p.writeStdout();
            }                        
            // outputs:
            // Text1with0numbers                        
        }
        
        public static void transformerFunc()
        {
            const String input = @"Text1with0numbers
log3message
Oliver Twist
";
            using (Pnyx p = new Pnyx())
            {
                p.readString(input);
                p.lineTransformerFunc(line =>
                {
                    String numbers = TextUtil.extractNumeric(line);
                    return numbers;
                });
                p.writeStdout();
            }                        
            // outputs:
            // 10                        
        }
    }
}