using System;
using System.Collections.Generic;
using System.IO;
using pnyx.cmd.examples;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd
{
    public class Program
    {
        public static int Main(String[] args)
        {
            try
            {
                Dictionary<String, String> switches = ArgsUtil.parseDictionary(ref args);

                if (switches.hasAny("-h", "--help"))
                    return printUsage();
                if (switches.hasAny("-vs", "--verboseSettings"))
                    return runVerboseSettings(switches, args);
                if (args.Length > 1)
                    return printUsage("unknown arguments specified. Pnyx only expected 1 parameter but found: " + args.Length, 4);

                // Reads settings file
                SettingsYaml.parseSetting();

                // Undocumented flag / used for testing
                String example = switches.value("-e", "--example");
                if (example != null)
                    return runExample(example, switches, args);

                if (switches.hasAny("-cs", "--csharp"))
                    return runCSharp(switches, args);
                else
                    return runYaml(switches, args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        private static int runCSharp(Dictionary<String, String> switches, String[] args)
        {
            String source;
            if (switches.hasAny("-i", "--inline"))
            {
                if (args.Length == 0)
                    return printUsage("missing inline YAML text", 3);

                source = args[0];
            }
            else
            {
                if (args.Length == 0)
                    return printUsage("missing YAML file", 2);

                using (TextReader reader = new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read)))
                    source = reader.ReadToEnd();
            }

            CodeParser parser = new CodeParser();
            Pnyx p = parser.parseCode(source);
            if (p.state != FluentState.Compiled)
                throw new IllegalStateException("Pnyx wasn't compiled properly");
                
            using (p)
                p.process();

            return 0;
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

            using (yamlInput)
            {
                YamlParser parser = new YamlParser();
                List<Pnyx> toExecute = parser.parseYaml(yamlInput);
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
        
        public static int printUsage(String message = null, int errorCode = 0)
        {
            Console.WriteLine("usage: pnyx [-h] commands");
            if (message != null)
            {
                Console.WriteLine("error: {0}", message);
                return errorCode;
            }

            Console.WriteLine();
            Console.WriteLine("Run a file of Pnyx commands, either YAML or CSharp script");
            Console.WriteLine();
            Console.WriteLine("optional arguments:");
            Console.WriteLine("-h, --help              show this help message and exit");
            Console.WriteLine("-cs, --csharp           flag to specify that commands are CSharp scripts instead of YAML");            
            Console.WriteLine("-i, --inline            flag to specify that first parameter is an inline YAML/CS String instead of a file path");
            Console.WriteLine("-vs, --verboseSettings  flag to display settings. program after displaying settings");
            Console.WriteLine();
            Console.WriteLine("required arguments:");
            Console.WriteLine("commands                path to YAML/CS file of Pnyx commands, which are compiled and executed");

            return errorCode;
        }

        private static int runExample(String example, Dictionary<String, String> switches, String[] args)
        {
            switch (example.ToLower())
            {
                case "bhcdischarge": return BhcDischarge.main();
                default: return printUsage("Unknown example: " + example, 69);                    
            }
        }        
    }
}