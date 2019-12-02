using System;
using System.Linq;
using pnyx.net.errors;
using pnyx.net.fluent;

namespace pnyx.cmd.shared
{
    public class ArgsInputOutput : INumberedInputOutput
    {
        private String[] args;
        private readonly bool[] used;
        private String input;
        private String output;

        public ArgsInputOutput(string[] args)
        {
            this.args = args;
            used = new bool[args.Length];
        }

        public string getImpliedInputFileName()
        {
            if (input != null)
                return input;               // makes method reentrant
            
            if (args.Length == 0)
                return null;                // fall back to STDIN
            
            if (args.Length > 2)
                throw new InvalidArgumentException("Implied input can only be used with 1 or 2 arguments");

            used[0] = true;
            input = args[0];
            return input;
        }

        public string getImpliedOutputFileName()
        {
            if (output != null)
                return output;               // makes method reentrant

            int index = input == null ? 0 : 1;
            if (index >= args.Length)
                return null;                 // fall back to STDOUT
            
            if (args.Length > index+1)
                throw new InvalidArgumentException("Implied output can only be used with 1 or 2 arguments");

            used[index] = true;
            output = args[index];
            return output;
        }

        public string getFileName(int argNumber)
        {
            int index = argNumber - 1;
            if (index < 0)
                throw new InvalidArgumentException("ArgNumber is 1-indexed: must be 1 or greater");
            
            if (index >= args.Length)
                throw new InvalidArgumentException($"ArgNumber {argNumber} is missing from parameters");

            used[index] = true;
            return args[index];
        }

        public bool verifyAllUsed()
        {
            if (used.Length == 0)
                return true;

            return used.All(x => x == true);
        }
    }
}