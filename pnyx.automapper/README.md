# pnyx.automapper
Pnyx.Automapper is a library for plugging in AutoMapper's functionality into the Pnyx ecosystem. Use this library for
mapping CSV files to and from POCOs, database streams and more. 

## Helpful Links
- [View Pnyx Project website](http://pnyx.me/)
- [Learn more about AutoMapper](https://automapper.io/)
- [Usage](http://pnyx.me/library/object#auto-mapper)

## Example Usage
Read a CSV file and map it to a list of Book objects:
```
MapperConfiguration config = new MapperConfiguration(_ => {}, NullLoggerFactory.Instance);

var converter = new AutoMapperObjectConverter<Book>(config.CreateMapper());

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
```

To write a list of Book objects to a CSV file:
```
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
```