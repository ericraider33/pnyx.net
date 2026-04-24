using System.Threading.Tasks;
using pnyx.net.fluent;

namespace pncs.cmd.examples.documentation.library;

public class ExampleFluent
{
    // dotnet pncs.cmd.dll -e=documentation ExampleFluent builder
    public static async Task builder()
    {
        await using Pnyx p = new Pnyx();
        p.readString("a,b,c,d")
         .parseCsv()
         .print("$4|$3|$2|$1")
         .writeStdout();

        // outputs: d|c|b|a           
    }
    
    // dotnet pncs.cmd.dll -e=documentation ExampleFluent minimalExplicit
    public static async Task minimalExplicit()
    {
        await using Pnyx p = new Pnyx();
        p.readStdin();
        p.writeStdout();
    }
    
    // dotnet pncs.cmd.dll -e=documentation ExampleFluent minimalImplicit
    public static async Task minimalImplicit()
    {
        await using Pnyx p = new Pnyx();
        p.setSettings(stdIoDefault: true);
    }
    
    // dotnet pncs.cmd.dll -e=documentation ExampleFluent argsExplicit
    public static async Task argsExplicit(string[] args)
    {
        await using Pnyx p = new Pnyx();
        p.setCommandLineArgs(args);
        p.readArg(1);
        p.writeArg(2);
    }
    
    // dotnet pncs.cmd.dll -e=documentation ExampleFluent argsImplicit
    public static async Task argsImplicit(string[] args)
    {
        await using Pnyx p = new Pnyx();
        p.setCommandLineArgs(args);
    }
}