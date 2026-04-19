using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using pnyx.net.fluent;

namespace pncs.cmd.examples.documentation.library;

public class ExampleFluent
{
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleFluent builder
    public static async Task builder()
    {
        await using (var p = new Pnyx())
            p.readString("a,b,c,d").parseCsv().print("$4|$3|$2|$1").writeStdout();

        // outputs: d|c|b|a           
    }

    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleFluent pnyxMethods
    public static async Task pnyxMethods()
    {
        MethodInfo[] methods = typeof(Pnyx).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
        String names = String.Join("\n", methods.Select(mi => mi.Name));
        await using (var p = new Pnyx())
            p.readString(names).write(@"c:\dev\ee.txt");
    }

    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleFluent pnyxHtml
}