using System;
using pnyx.net.api;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.impl.sed;
using pnyx.net.processors;
using pnyx.net.processors.dest;
using pnyx.net.processors.lines;
using pnyx.net.processors.sources;
using pnyx.net.util;

namespace pncs.cmd.examples.documentation.library
{
    public class ExampleProcessorChain
    {
        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleProcessorChain processorChain
        public static void processorChain()
        {
            StreamInformation streamInformation = new StreamInformation(new Settings());
                
            // Writes to STDOUT
            ILineProcessor dest = new LineProcessorToStream(streamInformation, Console.OpenStandardOutput());

            // Grep filter / processor pair
            ILineFilter grepFilter = new Grep {textToFind = "world", caseSensitive = false};
            ILineProcessor grepProcessor = new LineFilterProcessor { filter = grepFilter, processor = dest };
            
            // Sed transformer / processor pair
            ILineTransformer sedTransformer = new SedReplace("World", "World, with love from Pnyx..", null);
            ILineProcessor sedProcessor = new LineTransformerProcessor { transform = sedTransformer, processor = grepProcessor };
            
            // Reads from source
            using (StringStreamFactory streamFactory = new StringStreamFactory("Hello World."))
            {
                using (StreamToLineProcessor source = new StreamToLineProcessor(streamInformation, streamFactory))
                {
                    source.setNextLineProcessor(sedProcessor);            
            
                    // Runs 
                    source.process();                // All I/O occurs on this step
                }
            }

            // outputs: Hello World, with love from Pnyx...            
        }

        // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleProcessorChain sedShim
        public static void sedShim()
        {
            using (Pnyx p = new Pnyx())
            {
                p.readString("CSV,INPUT!,\"Go, Pnyx Go\"");
                p.parseCsv();
                p.sed("[,!]", "_", "g");
                p.writeStdout();
            }                        
            // outputs: CSV,INPUT_,"Go_ Pnyx Go"            
        }
    }
}