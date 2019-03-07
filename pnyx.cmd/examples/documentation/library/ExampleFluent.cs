using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExampleFluent
    {
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleFluent builder
        public static void builder()
        {
            using (var p = new Pnyx())
                p.readString("a,b,c,d").parseCsv().print("$4|$3|$2|$1").writeStdout();

            // outputs: d|c|b|a           
        }

        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleFluent pnyxMethods
        public static void pnyxMethods()
        {
            MethodInfo[] methods = typeof(Pnyx).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
            String names = String.Join("\n", methods.Select(mi => mi.Name));
            using (var p = new Pnyx())
                p.readString(names).write(@"c:\dev\ee.txt");
        }

        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleFluent pnyxHtml
    }
}