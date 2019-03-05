using pnyx.net.fluent;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExampleFluent
    {
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleFluent builder
        public static void builder()
        {
            using (var p = new Pnyx())
                p.readString("a,b,c,d").parseCsv().print("$4|$3|$2|$1").writeStdout();

            // outputs: d|c|b|a           
        }
        
    }
}