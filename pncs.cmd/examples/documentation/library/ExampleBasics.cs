using System;
using System.Threading.Tasks;
using pnyx.net.api;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.impl.sed;
using pnyx.net.processors;
using pnyx.net.processors.dest;
using pnyx.net.processors.lines;
using pnyx.net.processors.sources;
using pnyx.net.util;

namespace pncs.cmd.examples.documentation.library;

public class ExampleBasics
{
    // pnyx -e=documentation ExampleBasics processorChain
    public static async Task processorChain()
    {
        StreamInformation streamInformation = new StreamInformation(new Settings());
                
        // Writes to STDOUT
        ILineProcessor dest = new LineProcessorToStream(streamInformation, Console.OpenStandardOutput());

        // Grep filter / processor pair
        ILineFilter grepFilter = new Grep("world", caseSensitive: false);
        LineFilterProcessor grepProcessor = new (grepFilter);
        grepProcessor.setNextLineProcessor(dest);
            
        // Sed transformer / processor pair
        ILineTransformer sedTransformer = new SedReplace("World", "World, with love from Pnyx..", null);
        LineTransformerProcessor sedProcessor = new(sedTransformer);
        sedProcessor.setNextLineProcessor(grepProcessor);
            
        // Reads from source
        await using (StringStreamFactory streamFactory = new StringStreamFactory("Hello World."))
        {
            await using (StreamToLineProcessor source = new StreamToLineProcessor(streamInformation, streamFactory))
            {
                source.setNextLineProcessor(sedProcessor);            
            
                // Runs 
                await source.process();                // All I/O occurs on this step
            }
        }

        // outputs: Hello World, with love from Pnyx...            
    }

    // pnyx -e=documentation ExampleBasics sedShim
    public static async Task sedShim()
    {
        await using (Pnyx p = new Pnyx())
        {
            p.readString("CSV,INPUT!,\"Go, Pnyx Go\"");
            p.parseCsv();
            p.sed("[,!]", "_", "g");
            p.writeStdout();
        }                        
        // outputs: CSV,INPUT_,"Go_ Pnyx Go"            
    }
}