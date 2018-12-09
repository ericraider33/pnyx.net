using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.cmd
{
// https://stackoverflow.com/questions/53844/how-can-i-evaluate-a-c-sharp-expression-dynamically            
// https://github.com/davideicardi/DynamicExpresso/
// https://github.com/aaubry/YamlDotNet            
// https://www.nuget.org/packages/YamlDotNet/     
// https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples#multi            
    public class Program
    {
        public static int Main(string[] args)
        {
            TextReader yamlInput = null;
            try
            {
                Dictionary<String, String> switches = ArgsUtil.parseDictionary(ref args);

                if (switches.hasAny("-h", "--help"))
                    return printUsage();
                if (args.Length > 1)
                    return printUsage("unknown arguments specified. Pnyx only expected 1 parameter but found: " + args.Length, 4);

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

                PnyxYaml parser = new PnyxYaml();
                List<Pnyx> toExecute = parser.parseYaml(yamlInput);
                foreach (Pnyx pnyx in toExecute)
                {
                    using (pnyx)
                        pnyx.process();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
            finally
            {
                if (yamlInput != null)
                    yamlInput.Dispose();
            }            
        }
        
        public static int printUsage(String message = null, int errorCode = 0)
        {
            Console.WriteLine("usage: pnyx [-h] commands.yaml");
            if (message != null)
            {
                Console.WriteLine("error: {0}", message);
                return errorCode;
            }

            Console.WriteLine();
            Console.WriteLine("Run a YAML file of Pnyx commands");
            Console.WriteLine();
            Console.WriteLine("optional arguments:");
            Console.WriteLine("-h, --help              show this help message and exit");
            Console.WriteLine("-i, --inline            flag to specify that first parameter is an inline YAML string instead of a file path");            
            Console.WriteLine();
            Console.WriteLine("required arguments:");
            Console.WriteLine("commands.yaml           path to YAML file of Pnyx commands, which are compiled and executed");

            return errorCode;
        }
        
    }
}