using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using pnyx.net.errors;
using pnyx.net.fluent;

namespace pncs.cmd
{
    // https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples#multi   
    public class CodeParser
    {
        public void parseCode(Pnyx p, String source, bool compilePnyx = true)
        {
            // Compiles
            Script script = CSharpScript.Create(source, 
                globalsType: typeof(Pnyx), 
                options: ScriptOptions.Default.WithReferences(typeof(Pnyx).Assembly)
                );
            
            Task<ScriptState> parseTask = script.RunAsync(p);
            if (parseTask.IsCompletedSuccessfully)
            {
                if (p.state != FluentState.Compiled && compilePnyx)
                    p.compile();                            // builds processor
            }
            else
            {
                if (parseTask.Exception != null)
                    throw new IllegalStateException("Script failed to run with error: {0}", parseTask.Exception.Message);
                
                throw new IllegalStateException("Script failed to run with task in state: {0}", parseTask.Status.ToString());
            }
        }
    }
}