using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using pnyx.automapper.converters;
using pnyx.net.api;
using pnyx.net.fluent;

namespace pncs.cmd.examples.documentation.library;

public class ExampleObject
{
    public class Book
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int Revisions { get; set; }
    }
    
    public class BookConverter : IObjectConverterFromRow
    {
        public object rowToObject(List<string?> row, List<string>? header = null)
        {
            return new Book
            {
                Title = row[0],
                Author = row[1],
                Revisions = int.Parse(row[2]!)
            };
        }

        public List<string?> objectToRow(object obj)
        {
            Book book = (Book)obj;
            return new List<string?>
            {
                book.Title,
                book.Author,
                book.Revisions.ToString()
            };
        }

        public List<string> objectToHeader(object obj)
        {
            return new List<string>
            {
                "Title",
                "Author",
                "Revisions"
            };
        }
    }
    
    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleObject customConverter
    public static async Task customConverter()
    {
        const string input = """
Title,Author,Revisions
"Tale of Two Cities","Charles Dickens",3
"Oliver Twist","Charles Dickens",2
Odyssey,Homer,100
""";
        
        await using Pnyx p = new Pnyx();
        p.readString(input);
        p.parseCsv(hasHeader: true);
        p.rowToObject(new BookConverter());
        p.objectFilter(x => ((Book)x).Revisions > 2);
        p.objectToRow();
        p.writeStdout();
        // Output:
        // Title,Author,Revisions
        // Odyssey,Homer,100
    }

    // pnyx -e=documentation pncs.cmd.examples.documentation.library.ExampleObject autoMapper
    public static async Task autoMapper()
    {
        MapperConfiguration config = new MapperConfiguration(_ => {}, NullLoggerFactory.Instance);
        
        IObjectConverterFromNameValuePair converter = new AutoMapperObjectConverter<Book>(config.CreateMapper());
        
        const string input = """
Title,Author,Revisions
"Tale of Two Cities","Charles Dickens",3
"Oliver Twist","Charles Dickens",2
Odyssey,Homer,100
""";
        
        await using Pnyx p = new Pnyx();
        p.readString(input);
        p.parseCsv(hasHeader: true);
        p.rowToNameValuePair();
        p.nameValuePairToObject(converter);
        List<Book> books = await p.processCaptureObject<Book>();
        Console.WriteLine($"Read {books.Count} books.");
    }

    public static async Task autoMapperToCsv()
    {
        MapperConfiguration config = new MapperConfiguration(_ => { }, NullLoggerFactory.Instance);

        IObjectConverterFromNameValuePair converter = new AutoMapperObjectConverter<Book>(config.CreateMapper());

        List<object> books = new();
        // populate from database or from file/Pnyx

        await using Pnyx p = new Pnyx();
        p.readObject(() => books);
        p.objectToNameValuePair(converter);
        p.nameValuePairToRow(header: new List<string>{"Title","Author","Revisions"});
        p.rowToLine();
        string csvOutput = await p.processToString();
        Console.WriteLine(csvOutput);
    }
}