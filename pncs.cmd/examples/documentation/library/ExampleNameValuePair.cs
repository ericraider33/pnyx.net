using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.fluent;

namespace pncs.cmd.examples.documentation.library;

public class ExampleNameValuePair
{
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleNameValuePair filter
    public async Task filter()
    {
        const string csvInputA = """
                                 Title,Author,PublicationDate
                                 Tale of Two Cities,Charles Dickens,1859
                                 Oliver Twist,Charles Dickens,1839
                                 Odyssey,Homer,-1000
                                 """;
        
        await using Pnyx p = new Pnyx();
        p.readString(csvInputA);
        p.parseCsv(hasHeader: true);
        p.rowToNameValuePair();
        p.nameValuePairFilter(x => int.Parse(x["PublicationDate"]?.ToString() ?? "") >= 1840);
        List<IDictionary<string, object?>> actual = await p.processCaptureNameValuePairs();
        Console.WriteLine($"Matched {actual.Count} records");
    }
}