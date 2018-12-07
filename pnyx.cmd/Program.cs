using System;
using System.Collections.Generic;
using System.IO;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd
{
    class Program
    {
        static void Main(string[] args)
        {            
// https://stackoverflow.com/questions/53844/how-can-i-evaluate-a-c-sharp-expression-dynamically            
// https://github.com/davideicardi/DynamicExpresso/
// https://github.com/aaubry/YamlDotNet            
// https://www.nuget.org/packages/YamlDotNet/     
// https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples#multi            

            StringReader test = new StringReader(
@"readString: Hello World
writeStdout:
");
            
            PnyxYaml parser = new PnyxYaml();
            List<Pnyx> toExecute = parser.parseYaml(test);
            foreach (Pnyx pnyx in toExecute)
            {
                using (pnyx)
                    pnyx.process();
            }
           
            Console.WriteLine("Done");
        }
    }
}