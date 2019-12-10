using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using pnyx.cmd.shared;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd
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
                
                return runYaml(switches, args);
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

        private static int runYaml(Dictionary<String, String> switches, String[] args)
        {
            TextReader yamlInput;
            if (switches.hasAny("-i", "--inline"))
            {
                if (args.Length == 0)
                    return printUsage("missing inline YAML text", 3);

                yamlInput = new StringReader(args[0]);
            }
            else
            {
                if (args.Length == 0)
                    return printUsage("missing YAML file", 2);

                yamlInput = new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read));
            }

            // Sets arguments 
            args = args.Skip(1).ToArray();
            ArgsInputOutput argsIo = new ArgsInputOutput(args);

            using (yamlInput)
            {
                YamlParser parser = new YamlParser();
                List<Pnyx> toExecute = parser.parseYaml(yamlInput, argsIo);
                foreach (Pnyx pnyx in toExecute)
                {
                    using (pnyx)
                        pnyx.process();
                }            
            }

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
            Console.WriteLine("pnyx.cmd {0}", assemblyName.Version);            
        }
        
        private static int printUsage(String message = null, int errorCode = 0)
        {
            if (message == null && errorCode == 0)
                printVersion();        // shows version when user asks for Help
            
            Console.WriteLine("usage: pnyx [-h] commands");
            if (message != null)
            {
                Console.WriteLine("error: {0}", message);
                return errorCode;
            }

            Console.WriteLine();
            Console.WriteLine("Run a file of Pnyx commands from YAML script");
            Console.WriteLine();
            Console.WriteLine("optional arguments:");
            Console.WriteLine("-h, --help              show this help message and exit");
            Console.WriteLine("-d, --debug             prints stack trace upon exception");
            Console.WriteLine("-v, --version           shows version of application");            
            Console.WriteLine("-i, --inline            flag to specify that first parameter is an inline YAML String instead of a file path");
            Console.WriteLine("-vs, --verboseSettings  flag to display settings. program after displaying settings");
            Console.WriteLine();
            Console.WriteLine("required arguments:");
            Console.WriteLine("commands                path to YAML file of Pnyx commands, which are compiled and executed");

            return errorCode;
        }
    }
}