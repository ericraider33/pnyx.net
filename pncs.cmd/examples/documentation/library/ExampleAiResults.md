# Examples Results from Comparing Various AI Models

See [full article]([https://open.substack.com/pub/ericeschenbach/p/comparing-various-ai-models]):

```
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.fluent;

namespace pncs.cmd.examples.documentation.library;

/// <summary>
///  From prompt:
/// using dotnet and the Pnyx library, create a method to parse a CSV, filter the first column by matching "Atlanta",
/// and then print back out as a CSV
/// 
/// </summary>
public class ExampleAiResults
{
    public static async Task manus(string inputCsvPath)
    {
        await using var p = new Pnyx();
        p.read(inputCsvPath)
            .parseCsv()
            .columnFilter(1, value => string.Equals(value, "Atlanta", StringComparison.Ordinal))
            .writeStdout();
        await p.process();
    }
    
    public static async Task<string> grok(
        string input,                    // CSV content as string or file path
        bool isFilePath = false,
        bool caseSensitive = true,
        bool hasHeader = true)
    {
        await using Pnyx p = new Pnyx();

        // Input
        if (isFilePath)
            p.readFile(input);
        else
            p.readString(input);

        // Parse as CSV (handles quoting, etc.)
        p.parseCsv(hasHeader: hasHeader);

        // Filter on first column (index 1)
        if (caseSensitive)
            p.grep("^Atlanta$");           // exact match on column 1
        else
            p.grep("^Atlanta$", caseSensitive: false);

        // Output as CSV string
        return await p.processToString();
    }    
    
    public static void claude(string filePath)
    {
        using (Pnyx p = new Pnyx())
        {
            p.read(filePath);
            p.parseCsv();
            p.grep(row => row.Count > 0 && row[0] == "Atlanta");
            p.writeStdout();
        }
    }    

    public static async Task gemini(string inputPath, string outputPath)
    {
        // The fluent API mimics a Unix pipeline: cat | grep | redirect
        await PnyxFile.Open(inputPath)
            .AsTabular(delimiter: ',') // Treat as CSV
            .Where(row => row[0].Equals("Atlanta", StringComparison.OrdinalIgnoreCase))
            .SaveAsTabular(outputPath);
        
        Console.WriteLine($"Filtered CSV saved to: {outputPath}");
    }    
    
    public static string perpelexity(string inputCsv)
    {
        using var reader = new StringReader(inputCsv);
        using var writer = new StringWriter();

        var input = Pnyx.Pnyx.From(reader)
            .Csv();

        input
            .Where(row => row[0] == "Atlanta")
            .ToCsv(writer);

        return writer.ToString();
    }    
    
    public static void copilot(string inputPath, string outputPath)
    {
        using var input = File.OpenRead(inputPath);
        using var output = File.Create(outputPath);

        Pnyx.Read(input)
            .Csv()                       // parse CSV into string[] rows
            .Where(row => row[0] == "Atlanta")
            .WriteCsv(output);           // write rows back out as CSV
    }
    
    public static void chatGpt(
        string inputCsvPath,
        string outputCsvPath)
    {
        List<List<string>> filteredRows = new();
        List<string>? header = null;

        using (Pnyx p = new Pnyx())
        {
            // Read the CSV
            p.readCsv(inputCsvPath);

            bool isFirstRow = true;

            p.readRowFunc(() =>
                {
                    foreach (var row in p.Rows)
                    {
                        // Save header
                        if (isFirstRow)
                        {
                            header = row.ToList();
                            isFirstRow = false;
                            continue;
                        }

                        // Filter where first column == "Atlanta"
                        if (row.Count > 0 &&
                            row[0].Equals("Atlanta", StringComparison.OrdinalIgnoreCase))
                        {
                            filteredRows.Add(row.ToList());
                        }
                    }

                    return filteredRows;
                },
                header: () => header ?? new List<string>());

            // Write filtered rows back to CSV
            p.writeCsv(outputCsvPath);
        }

        Console.WriteLine($"Filtered CSV written to: {outputCsvPath}");
    }    
}
```