using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using pncs.cmd.examples;
using pnyx.cmd.shared;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pncs.cmd
{
    public class Program
    {
        public static int Main(String[] args)
        {
            Dictionary<String, String> switches = ArgsUtil.parseDictionary(ref args);
            try
            {
                if (switches.hasAny("-h", "--help"))
                    return printUsage();
                if (switches.hasAny("-v", "--version"))
                    return runVersion();
                if (switches.hasAny("-vs", "--verboseSettings"))
                    return runVerboseSettings(switches, args);
                                
                // Reads settings file
                SettingsYaml.parseSetting();

                // Undocumented flag / used for testing
                String example = switches.value("-e", "--example");
                if (example != null)
                    return runExample(example, switches, args);
                
                if (args.Length > 1)
                    return printUsage("unknown arguments specified. Pncs only expected 1 parameter but found: " + args.Length, 4);                
                
                return runCSharp(switches, args);
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

        private static int runCSharp(Dictionary<String, String> switches, String[] args)
        {
            String source;
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

                using (TextReader reader = new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read)))
                    source = reader.ReadToEnd();
            }
                        
            Pnyx p = new Pnyx();
            p.setSettings(stdIoDefault: true);              // forces STD-IN/OUT as defaults                         

            CodeParser parser = new CodeParser();
            parser.parseCode(p, source, compilePnyx: true);
            if (p.state != FluentState.Compiled)
                throw new IllegalStateException("Pnyx wasn't compiled properly");
                
            using (p)
                p.process();

            return 0;
        }

        private static int runVerboseSettings(Dictionary<String, String> switches, String[] args)
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
            AssemblyName assemblyName = Assembly.GetAssembly(typeof(Pnyx)).GetName();
            Console.WriteLine("pncs.cmd {0}", assemblyName.Version);            
        }
        
        private static int printUsage(String message = null, int errorCode = 0)
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
            Console.WriteLine("-i, --inline            flag to specify that first parameter is an inline CSharp String instead of a file path");
            Console.WriteLine("-vs, --verboseSettings  flag to display settings. program after displaying settings");
            Console.WriteLine();
            Console.WriteLine("required arguments:");
            Console.WriteLine("commands                path to CSharp file of Pnyx commands, which are compiled and executed");

            return errorCode;
        }

        private static int runExample(String example, Dictionary<String, String> switches, String[] args)
        {                        
            switch (example.ToLower())
            {
                case "bhcdischarge": return BhcDischarge.main();
                case "bhcprocedure": return BhcProcedures.main();
                case "altitude": return Altitude.main();
                case "ga": return GaCleanup.main();
                case "column": return ColumnDefinitionExample.main();
                case "documentation": return runDocumentationExample(switches, args);
                default: return printUsage("Unknown example: " + example, 69);                    
            }
        }

        private static int runDocumentationExample(Dictionary<String, String> switches, String[] args)
        {
            if (args.Length != 2)
                return printUsage("Invalid Parameters: Documentation examples require a class name and a method name as parameters");
            
            String typeName = args[0];
            String methodName = args[1];

            // Finds type
            Assembly cmdAssembly = typeof(Program).Assembly;
            Type type = cmdAssembly.GetType(typeName);
            
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                throw new InvalidArgumentException("Static method '{0}' could not be found on type: {1}", methodName, typeName);
            
            // Runs example
            method.Invoke(null, new Object[0]);

            return 0;
        }
    }
}
