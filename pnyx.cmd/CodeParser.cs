using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using pnyx.net.errors;
using pnyx.net.fluent;

namespace pnyx.cmd
{
    // https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples#multi   
    public class CodeParser
    {
        public Pnyx parseCode(String source, bool compile = true)
        {
            // Compiles
            Script script = CSharpScript.Create(source, globalsType: typeof(Pnyx));
            
            Pnyx p = new Pnyx();
            p.setSettings(stdIoDefault: true);              // forces STD-IN/OUT as defaults                         
            
            Task<ScriptState> parseTask = script.RunAsync(p);
            if (parseTask.IsCompletedSuccessfully)
            {
                if (p.state != FluentState.Compiled && compile)
                    p.compile();                            // builds processor

                return p;
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