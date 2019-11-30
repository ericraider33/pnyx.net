using pnyx.net.fluent;

namespace pncs.cmd.examples.documentation.library
{
    public class ExampleStateMachine
    {
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleStateMachine cat
        public static void cat()
        {
            using (Pnyx p = new Pnyx())
            {
                p.cat(pn =>
                {
                    pn.readString("Line one");
                    pn.readString("Line two");
                    pn.readString("Line three");
                    // ...
                });
                p.writeStdout();
            }

            // outputs:
            // Line one
            // Line two
            // Line three
        }

        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleStateMachine catCsv
        public static void catCsv()
        {
            using (Pnyx p = new Pnyx())
            {
                p.cat(p2 =>
                {
                    p2.asCsv(p3 =>
                    {
                        p3.readString("Line,one");
                        p3.readString("Line,two");
                        p3.readString("Line,three");
                    });
                });
                p.selectColumns(2, 1);
                p.writeStdout();
            }
            // outputs:
            // one,Line
            // two,Line
            // three,Line
        }

        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleStateMachine tee
        public static void tee()
        {
            using (Pnyx p = new Pnyx())
            {
                p.readString("1975,218M,\"Love Will Keep Us Together\"\n");
                p.parseCsv();
                p.tee(p2 =>
                {
                    p2.selectColumns(1, 2);
                    p2.write("us_population_by_year.csv");
                    // outputs: 1975,218M                    
                });
                p.selectColumns(1, 3);
                p.write("top_songs_by_year.csv");
                // outputs: 1975,"Love Will Keep Us Together"
            }
        }
        
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleStateMachine teeMultiple
        public static void teeMultiple()
        {
            using (Pnyx p = new Pnyx())
            {
                p.readString("clientId: 123456\n");
                p.tee(p2 =>
                {
                    p2.write("copy.txt");
                    // outputs: clientId: 123456
                });
                p.parseDelimiter(": ");                
                p.selectColumns(2);
                p.tee(p2 =>
                {
                    p2.write("ids.txt");
                    // outputs: 123456
                });
                p.print("delete from client where id = $0;");
                p.writeStdout();
                // outputs: delete from client where id = 123456;
            }
        }
    }
}