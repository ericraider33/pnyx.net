using Microsoft.CodeAnalysis.Scripting;
using pnyx.net.api;

namespace pnyx.cmd.code
{
    public class CodeLineFilter : ILineFilter
    {
        private readonly LineGlobals globals;
        private readonly Script<bool> script;

        public CodeLineFilter(LineGlobals globals, Script<bool> script)
        {
            this.globals = globals;
            this.script = script;
        }

        public bool shouldKeepLine(string line)
        {
            globals.line = line;
            return script.RunAsync(globals).Result.ReturnValue;
        }
    }
}