using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using pnyx.automapper.converters;
using pnyx.net.api;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.automapper.test.automapper;

public class ObjectAutoMapperTest
{
    const string csvInputA = """
Title,Author,PublicationDate
"Tale of Two Cities","Charles Dickens",1859
"Oliver Twist","Charles Dickens",1839
Odyssey,Homer,-1000
""";
    
    public class Book
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int PublicationDate { get; set; }
    }
    
    [Fact]
    public async Task automapper_and_pnyx_kick_ass()
    {
        MapperConfiguration config = new MapperConfiguration(_ =>
        {
//            cfg.CreateMap<IDictionary<string, Object>, Book>();
        }, NullLoggerFactory.Instance);
//        config.AssertConfigurationIsValid();

        IObjectConverterFromNameValuePair converter = new AutoMapperObjectConverter<Book>(config.CreateMapper());
        
        List<Book> actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(converter);
            actual = await p.processCaptureObject<Book>();
        }
        
        Assert.Equal(3, actual.Count);

        Book first = actual[0];
        Assert.Equal("Tale of Two Cities", first.Title);
        Assert.Equal("Charles Dickens", first.Author);
        Assert.Equal(1859, first.PublicationDate);
    }
    
    [Fact]
    public async Task automapper_to_csv()
    {
        MapperConfiguration config = new MapperConfiguration(_ => {}, NullLoggerFactory.Instance);

        IObjectConverterFromNameValuePair converter = new AutoMapperObjectConverter<Book>(config.CreateMapper());
        
        List<IDictionary<string, object?>> actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(converter);
            p.objectToNameValuePair();
            actual = await p.processCaptureNameValuePairs();
        }

        Assert.Equal(3, actual.Count);

        IDictionary<string, object?> first = actual[0];
        Assert.Equal("Author,PublicationDate,Title", string.Join(",", first.Keys.Order()));
        Assert.Equal("Tale of Two Cities", first["Title"]);
        Assert.Equal("Charles Dickens", first["Author"]);
        Assert.Equal(1859, first["PublicationDate"]);
    }
    
    [Fact]
    public async Task automapper_object_to_csv()
    {
        List<Book> actual = new List<Book>
        {
            new Book {Title = "Tale of Two Cities", Author = "Charles Dickens", PublicationDate = 1859},
            new Book {Title = "Oliver Twist", Author = "Charles Dickens", PublicationDate = 1839},
            new Book {Title = "Odyssey", Author = "Homer", PublicationDate = -1000}
        };

        Assert.Equal(3, actual.Count);
        
        MapperConfiguration config = new MapperConfiguration(_ => { }, NullLoggerFactory.Instance);

        IObjectConverterFromNameValuePair converter = new AutoMapperObjectConverter<Book>(config.CreateMapper());

        string csvOutputA;
        await using (Pnyx p = new Pnyx())
        {
            p.readObject(() => actual);
            p.objectToNameValuePair(converter);
            p.nameValuePairToRow(header: new List<string>{"Title","Author","PublicationDate"});
            p.rowToLine();
            csvOutputA = await p.processToString();
        }

        Assert.Equal(csvInputA, csvOutputA);
    }
}