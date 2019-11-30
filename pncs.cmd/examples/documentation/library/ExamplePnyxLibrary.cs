using pnyx.net.fluent;
     
namespace pncs.cmd.examples.documentation.library
{
    public class ExamplePnyxLibary
    {
        public static void helloWorld()
        {
            using (Pnyx p = new Pnyx())
            {
                p.readString("Hello World.");
                p.sed("World", "World, with love from Pnyx..");     // transforms each line
                p.grep("world", caseSensitive: false);    // filters each line
                p.writeStdout();
                p.compile();         // Builds processors and wires filters together
                p.process();         // Runs processors (All IO is done here)
            }                        
            // outputs: Hello World, with love from Pnyx...
        }
        
        public static void minimum()
        {
            using (Pnyx p = new Pnyx())
            {
                p.readString("Minimum");
                p.writeStdout();
            }
            // outputs: Minimum
        }
        
    }
}