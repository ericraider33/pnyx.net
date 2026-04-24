using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using pncs.cmd.examples;
using pncs.cmd.examples.documentation.library;
using pnyx.cmd.shared;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pncs.cmd;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Dictionary<string, string?> switches = ArgsUtil.parseDictionary(ref args);
        try
        {
            if (switches.hasAny("-h", "--help"))
                return printUsage();
            if (switches.hasAny("-v", "--version"))
                return runVersion();
            if (switches.hasAny("-vs", "--verboseSettings"))
                return runVerboseSettings();
                                
            // Reads settings file
            SettingsYaml.parseSetting();

            // Undocumented flag / used for testing
            string? example = switches.value("-e", "--example");
            if (example != null)
                return await runExample(example, args);
                
            return await runCSharp(switches, args);
        }
        catch (Exception e)
        {
            while (e.InnerException != null)
                e = e.InnerException;
            Console.WriteLine(e.Message);
            if (switches.hasAny("-d", "--debug"))
                Console.WriteLine(e.StackTrace);
            return 1;
        }
    }

    private static async Task<int> runCSharp(Dictionary<string, string?> switches, string[] args)
    {
        string source;
        if (switches.hasAny("-i", "--inline"))
        {
            if (args.Length == 0)
                return printUsage("missing inline CSharp script", 3);

            source = args[0];
        }
        else
        {
            if (args.Length == 0)
                return printUsage("missing CSharp file", 2);

            using TextReader reader = new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read));
            source = await reader.ReadToEndAsync();
        }
                        
        Pnyx p = new Pnyx();
        p.setSettings(stdIoDefault: true);              // forces STD-IN/OUT as defaults                         

        // Sets arguments 
        args = args.Skip(1).ToArray();
        p.setNumberedInputOutput(new ArgsInputOutput(args));
            
        CodeParser parser = new CodeParser();
        parser.parseCode(p, source, compilePnyx: true);
        if (p.state != FluentState.Compiled)
            throw new IllegalStateException("Pnyx wasn't compiled properly");
                
        await using (p)
            await p.process();

        return 0;
    }

    private static int runVerboseSettings()
    {
        bool success = SettingsYaml.parseSetting(verboseSettings: true);
        if (success)
            Console.WriteLine("\nShowing settings as configured from settings file:\n");
        else
            Console.WriteLine("\nShowing default settings of application:\n");

        SettingsYaml sy = new SettingsYaml();
        sy.serializeSettings(Console.Out, SettingsHome.settingsFactory.buildSettings());
        return 0;
    }

    private static int runVersion()
    {
        printVersion();
        return 0;
    }

    private static void printVersion()
    {
        AssemblyName? assemblyName = Assembly.GetAssembly(typeof(Pnyx))?.GetName();
        Console.WriteLine("pncs.cmd {0}", assemblyName?.Version);            
    }
        
    private static int printUsage(string? message = null, int errorCode = 0)
    {
        if (message == null && errorCode == 0)
            printVersion();        // shows version when user asks for Help
            
        Console.WriteLine("usage: pncs [-h] commands");
        if (message != null)
        {
            Console.WriteLine("error: {0}", message);
            return errorCode;
        }

        Console.WriteLine();
        Console.WriteLine("Run a file of Pncs commands from CSharp script");
        Console.WriteLine();
        Console.WriteLine("optional arguments:");
        Console.WriteLine("-h, --help              show this help message and exit");
        Console.WriteLine("-d, --debug             prints stack trace upon exception");
        Console.WriteLine("-v, --version           shows version of application");            
        Console.WriteLine("-i, --inline            flag to specify that first parameter is an inline CSharp string instead of a file path");
        Console.WriteLine("-vs, --verboseSettings  flag to display settings. program after displaying settings");
        Console.WriteLine();
        Console.WriteLine("required arguments:");
        Console.WriteLine("commands                path to CSharp file of Pnyx commands, which are compiled and executed");

        return errorCode;
    }

    private static async Task<int> runExample(string example, string[] args)
    {                        
        switch (example.ToLower())
        {
            case "column": return await ColumnDefinitionExample.main();
            case "documentation": return await runDocumentationExample(args);
            case "discovery": return await DiscoveryExample.main();
            default: return printUsage("Unknown example: " + example, 69);                    
        }
    }

    private static async Task<int> runDocumentationExample(string[] args)
    {
        if (args.Length < 2)
            return printUsage("Invalid Parameters: Documentation examples require a class name and a method name as parameters");
            
        string typeName = args[0];
        
        // Fully qualified type name, if using just the class name
        if (!typeName.Contains("."))
        {
            string? exampleType = typeof(ExampleBasics).FullName;
            if (exampleType != null)
                typeName = exampleType.Replace(nameof(ExampleBasics), typeName);
        }
        
        string methodName = args[1];
        string[] methodArgs = args.Skip(2).ToArray();                                  // remaining arguments are passed to the example method  

        // Finds type
        Assembly cmdAssembly = typeof(Program).Assembly;
#pragma warning disable IL2026
        Type? type = cmdAssembly.GetType(typeName);
#pragma warning restore IL2026

        if (type == null)
            throw new InvalidArgumentException("Type '{0}' could not be found in assembly: {1}", typeName, cmdAssembly.FullName ?? "CMD Assembly");
            
#pragma warning disable IL2075
        MethodInfo? method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
#pragma warning restore IL2075
        if (method == null)
            throw new InvalidArgumentException("Static method '{0}' could not be found on type: {1}", methodName, typeName);

        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length == 0)
        {
            // Runs example
            Object? result = method.Invoke(null, []);
            if (result is Task task)
                await task;
        }
        else if (parameters.Length == 1)
        {
            // Runs example
            Object? result = method.Invoke(null, [methodArgs]);
            if (result is Task task)
                await task;
        }
        else
        {
            throw new InvalidArgumentException("Method '{0}' on type: {1} has more than one parameter", methodName, typeName);
        }
            
        return 0;
    }
}