using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using pnyx.net.api;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.processors;

public class ObjectTest
{
    const String csvInputA = """
Title,Author,PublicationDate
"Tale of Two Cities","Charles Dickens",1859
"Oliver Twist","Charles Dickens",1839
Odyssey,Homer,-1000
""";

    public class Book
    {
        public String Title { get; set; }
        public String Author { get; set; }
        public int PublicationDate { get; set; }
    }

    public class BookExtended
    {
        public String Title { get; set; }
        public String Author { get; set; }
        public int PublicationDate { get; set; }
        public DateTime Timestamp = DateTime.Now;

        public static BookExtended from(Book x)
        {
            return new BookExtended
            {
                Title = x.Title,
                Author = x.Author,
                PublicationDate = x.PublicationDate
            };
        }
    }

    public class BookConverter : IObjectConverterFromRow, IObjectConverterFromNameValuePair
    {
        public object rowToObject(List<string> row, List<string> header = null)
        {
            return new Book
            {
                Title = row[0],
                Author = row[1],
                PublicationDate = int.Parse(row[2])
            };
        }

        public List<string> objectToRow(object obj)
        {
            Book book = (Book)obj;
            return new List<string>
            {
                book.Title,
                book.Author,
                book.PublicationDate.ToString()
            };
        }

        public List<string> objectToHeader(object obj)
        {
            return new List<string>
            {
                "Title",
                "Author",
                "PublicationDate"
            };
        }


        public object nameValuePairToObject(IDictionary<string, object> row)
        {
            return new Book
            {
                Title = row["Title"] as String,
                Author = row["Author"] as String,
                PublicationDate = int.Parse(row["PublicationDate"].ToString())
            };
        }

        public IDictionary<string, object> objectToNameValuePair(object obj)
        {
            Book book = (Book)obj;
            return new Dictionary<string, object>
            {
                { "Title", book.Title },
                { "Author", book.Author },
                { "PublicationDate", book.PublicationDate }
            };
        }
    }
    
    [Fact]
    public void verify_basic_usage()
    {
        List<Book> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToObject(new BookConverter());
            actual = p.processCaptureObject<Book>();
        }

        Assert.Equal(3, actual.Count);

        Book first = actual[0];
        Assert.Equal("Tale of Two Cities", first.Title);
        Assert.Equal("Charles Dickens", first.Author);
        Assert.Equal(1859, first.PublicationDate);
    }
    
    [Fact]
    public void object_to_from_row()
    {
        String actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToObject(new BookConverter());
            p.objectToRow();
            actual = p.processToString();
        }

        Assert.Equal(csvInputA, actual);
    }

    [Fact]
    public void nameValuePair_to_object()
    {
        List<Book> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(new BookConverter());
            actual = p.processCaptureObject<Book>();
        }

        Assert.Equal(3, actual.Count);

        Book first = actual[0];
        Assert.Equal("Tale of Two Cities", first.Title);
        Assert.Equal("Charles Dickens", first.Author);
        Assert.Equal(1859, first.PublicationDate);
    }
    
    [Fact]
    public void object_to_from_nameValuePair()
    {
        List<IDictionary<String, Object>> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(new BookConverter());
            p.objectToNameValuePair();
            actual = p.processCaptureNameValuePairs();
        }

        Assert.Equal(3, actual.Count);

        IDictionary<String, Object> first = actual[0];
        Assert.Equal("Author,PublicationDate,Title", String.Join(",", first.Keys.Order()));
        Assert.Equal("Tale of Two Cities", first["Title"]);
        Assert.Equal("Charles Dickens", first["Author"]);
        Assert.Equal(1859, first["PublicationDate"]);
    }

    [Fact]
    public void automapper_and_pnyx_kick_ass()
    {
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
//            cfg.CreateMap<IDictionary<String, Object>, Book>();
        });
//        config.AssertConfigurationIsValid();

        IObjectConverterFromNameValuePair converter = new AutoMapperObjectConverter<Book>
        {
            mapper = config.CreateMapper() 
        };
        
        List<Book> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(converter);
            actual = p.processCaptureObject<Book>();
        }
        
        Assert.Equal(3, actual.Count);

        Book first = actual[0];
        Assert.Equal("Tale of Two Cities", first.Title);
        Assert.Equal("Charles Dickens", first.Author);
        Assert.Equal(1859, first.PublicationDate);
    }
    
    [Fact]
    public void automapper_to_csv()
    {
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
        });

        IObjectConverterFromNameValuePair converter = new AutoMapperObjectConverter<Book>
        {
            mapper = config.CreateMapper() 
        };
        
        List<IDictionary<String, Object>> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(converter);
            p.objectToNameValuePair();
            actual = p.processCaptureNameValuePairs();
        }

        Assert.Equal(3, actual.Count);

        IDictionary<String, Object> first = actual[0];
        Assert.Equal("Author,PublicationDate,Title", String.Join(",", first.Keys.Order()));
        Assert.Equal("Tale of Two Cities", first["Title"]);
        Assert.Equal("Charles Dickens", first["Author"]);
        Assert.Equal(1859, first["PublicationDate"]);
    }
    
    [Fact]
    public void automapper_object_to_csv()
    {
        List<Book> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToObject(new BookConverter());
            actual = p.processCaptureObject<Book>();
        }

        Assert.Equal(3, actual.Count);
        
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
        });

        IObjectConverterFromNameValuePair converter = new AutoMapperObjectConverter<Book>
        {
            mapper = config.CreateMapper() 
        };

        string csvOutputA = "";
        using (Pnyx p = new Pnyx())
        {
            p.readObject(() => actual);
            p.objectToNameValuePair(converter);
            p.nameValuePairToRow(header: new List<string>{"Title","Author","PublicationDate"});
            p.rowToLine();
            csvOutputA = p.processToString();
        }

        Assert.Equal(csvInputA, csvOutputA);
    }
    
    [Theory]
    [InlineData(-2000, 3)]
    [InlineData(1800, 2)]
    [InlineData(1845, 1)]
    [InlineData(1900, 0)]
    public void filter(int year, int expected)
    {
        List<Book> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(new BookConverter());
            p.objectFilter(x => ((Book)x).PublicationDate >= year);
            actual = p.processCaptureObject<Book>();
        }

        Assert.Equal(expected, actual.Count);
    }  
    
    [Theory]
    [InlineData(-2000, 3)]
    [InlineData(1800, 2)]
    [InlineData(1845, 1)]
    [InlineData(1900, 0)]
    public void filterGeneric(int year, int expected)
    {
        List<Book> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(new BookConverter());
            p.objectFilter<Book>(x => x.PublicationDate >= year);
            actual = p.processCaptureObject<Book>();
        }

        Assert.Equal(expected, actual.Count);
    }  
    
    [Fact]
    public void transform()
    {
        List<BookExtended> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(new BookConverter());
            p.objectTransform(x => BookExtended.from((Book)x));
            actual = p.processCaptureObject<BookExtended>();
        }

        Assert.Equal(3, actual.Count);

        BookExtended first = actual[0];
        Assert.Equal("Tale of Two Cities", first.Title);
        Assert.Equal("Charles Dickens", first.Author);
        Assert.True(first.Timestamp > new DateTime(2024,1,1));
    }
    
    [Fact]
    public void transformGeneric()
    {
        List<BookExtended> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairToObject(new BookConverter());
            p.objectTransform<Book>(BookExtended.from);
            actual = p.processCaptureObject<BookExtended>();
        }

        Assert.Equal(3, actual.Count);

        BookExtended first = actual[0];
        Assert.Equal("Tale of Two Cities", first.Title);
        Assert.Equal("Charles Dickens", first.Author);
        Assert.True(first.Timestamp > new DateTime(2024,1,1));
    }
}