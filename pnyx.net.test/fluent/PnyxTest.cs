using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.impl.sed;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.fluent;

public class PnyxTest
{
    private const String MAGNA_CARTA =
        @"KNOW THAT BEFORE GOD, for the health of our soul and those of our ancestors and heirs, 
to the honour of God, the exaltation of the holy Church, and the better ordering of our
kingdom, at the advice of our reverend fathers Stephen, archbishop of Canterbury, 
primate of all England, and cardinal of the holy Roman Church, Henry archbishop of 
Dublin, William bishop of London, Peter bishop of Winchester, Jocelin bishop of Bath
and Glastonbury, Hugh bishop of Lincoln, Walter Bishop of Worcester, William bishop
of Coventry, Benedict bishop of Rochester, Master Pandulf subdeacon and member of the
papal household, Brother Aymeric master of the knighthood of the Temple in England, 
William Marshal earl of Pembroke, William earl of Salisbury, William earl of Warren, 
William earl of Arundel, Alan de Galloway constable of Scotland, Warin Fitz Gerald, 
Peter Fitz Herbert, Hubert de Burgh seneschal of Poitou, Hugh de Neville, 
Matthew Fitz Herbert, Thomas Basset, Alan Basset, Philip Daubeny, Robert de Roppeley,
John Marshal, John Fitz Hugh, and other loyal subjects:";

    private const String PLANETS_GODS =
        @"Aphrodite,Venus,""Goddess of love and beauty""
Ares,Mars,""Hated god of war""
Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,""God of the sea and earthquakes""
Uranus,Uranus,""Father of the Titans""
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
";

    private const String PLANETS_GODS_TITANS =
        @"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Uranus,Uranus,""Father of the Titans""
";

    const String PLANETS_GODS_FORMAT_ISSUES =
        @"Aphrodite,Venus,Goddess of love and beauty
Ares,Mars,""Hated god ""of war
Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,""God of the sea and earthquakes""
Uranus,Uranus,""Father of the Titans""
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
";

    private const String EARTH = @"Gaia,Terra,""Mother goddess of the earth""";

    private const String ECONOMIC_FREEDOM =
        @"1	Hong Kong	90.2
2	Singapore	88.8
3	New Zealand	84.2
4	Switzerland	81.7
5	Australia	80.9
";

    [Fact]
    public async Task inOut()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(MAGNA_CARTA);
            actual = await p.processToString();
        }

        Assert.Equal(MAGNA_CARTA, actual);
    }

    [Fact]
    public async Task lineFilter()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(MAGNA_CARTA);
            p.grep("god", caseSensitive: false);
            actual = await p.processToString();
        }

        const String expected = @"KNOW THAT BEFORE GOD, for the health of our soul and those of our ancestors and heirs, 
to the honour of God, the exaltation of the holy Church, and the better ordering of our";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task lineTransform()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.sed(",", "\t", "g");
            actual = await p.processToString();
        }

        const String expected = "Gaia\tTerra\t\"Mother goddess of the earth\"";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task lineBuffering()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.setSettings(defaultNewline: StreamInformation.newlineString(NewLineEnum.Windows));
            p.readString(EARTH);
            p.sedAppendLine("The Lord is my shepherd");
            actual = await p.processToString();
        }

        String expected = "Gaia,Terra,\"Mother goddess of the earth\"\r\nThe Lord is my shepherd";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task csvInOut()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            actual = await p.processToString();
        }

        Assert.Equal(PLANETS_GODS, actual);
    }

    [Fact]
    public async Task csvInOutFixFormatting()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS_FORMAT_ISSUES);
            p.parseCsv(strict: false);
            actual = await p.processToString();
        }

        Assert.Equal(PLANETS_GODS, actual); // verifies that output is formatted properly, even if input is loose
    }

    [Fact]
    public async Task csvInOutVariant()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS_FORMAT_ISSUES);
            actual = await p.processToString((pnyx, stream) => pnyx.writeCsvStream(stream, strict: false));
        }

        Assert.Equal(PLANETS_GODS, actual);
    }

    [Fact]
    public async Task asCsv()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.asCsv(pn => pn.readString(PLANETS_GODS_FORMAT_ISSUES), strict: false);                
            actual = await p.processToString();
        }

        Assert.Equal(PLANETS_GODS, actual);
    }

    [Fact]
    public async Task hasHeader()
    {
        const String source = @"headerA,headerB
valueA1,valueB1
";
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(source);
            p.parseCsv(hasHeader: true);
            p.sed("[AB]", "X");
            actual = await p.processToString();
        }

        const String expected = @"headerA,headerB
valueX1,valueX1
";
        Assert.Equal(expected, actual);            
    }
        
    [Fact]
    public async Task rowFilter()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.grep("war");
            actual = await p.processToString();
        }

        String expected = "Ares,Mars,\"Hated god of war\"" + Environment.NewLine;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task rowTransform()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.sed("Ter.*", "Forma", "g");
            actual = await p.processToString();
        }

        const String expected = "Gaia,Forma,\"Mother goddess of the earth\"";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task rowBuffering()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.setSettings(defaultNewline: StreamInformation.newlineString(NewLineEnum.Windows));
            p.readString(EARTH);
            p.parseCsv();
            p.sedAppendRow(["The Lord", "is", "my shepherd"]);
            actual = await p.processToString();
        }

        String expected = "Gaia,Terra,\"Mother goddess of the earth\"\r\n\"The Lord\",is,\"my shepherd\""; 
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task readImproperState()
    {
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.sed("Ter.*", "Forma", "g");
            Assert.Throws<IllegalStateException>(() => p.readString(EARTH));
        }
    }

    [Fact]
    public async Task cat()
    {
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            Assert.Throws<IllegalStateException>(() => p.readString(PLANETS_GODS));
        }
    }

    [Fact]
    public async Task lineToRow()
    {
        String actual;
        const String tabSource = "1) Be fruitful and multiply\t2) and fill the earth and subdue it";
        await using (Pnyx p = new Pnyx())
        {
            p.readString(tabSource);
            p.sed("[\t]", ",", "g");
            p.parseCsv(strict: false);
            actual = await p.processToString();
        }

        const String expected = @"""1) Be fruitful and multiply"",""2) and fill the earth and subdue it""";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task lineToRowImproperState()
    {
        Assert.Throws<IllegalStateException>(() => new Pnyx().parseCsv());

        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            Assert.Throws<IllegalStateException>(() => p.parseCsv());
        }

        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.writeStream(new MemoryStream());
            Assert.Throws<IllegalStateException>(() => p.parseCsv());
        }
    }

    [Fact]
    public async Task rowToLine()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.rowToLine();
            p.sed(",", "\t", "g");
            actual = await p.processToString();
        }

        const String expected = "Gaia\tTerra\t\"Mother goddess of the earth\"";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task printRow()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.print("$3,$2,$1");
            actual = await p.processToString();
        }

        const String expected = "Mother goddess of the earth,Terra,Gaia";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task printLine()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString("Logic");
            p.print("$0 $0");
            actual = await p.processToString();
        }

        const String expected = "Logic Logic";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task columns()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.selectColumns(3, 2, 1);
            actual = await p.processToString();
        }

        const String expected = @"""Mother goddess of the earth"",Terra,Gaia";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task columnToLine()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.printColumn(2);
            actual = await p.processToString();
        }

        const String expected = "Terra";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task tabInOutImplicit()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(ECONOMIC_FREEDOM);
            p.parseTab();
            actual = await p.processToString();
        }

        Assert.Equal(ECONOMIC_FREEDOM, actual);
    }

    [Fact]
    public async Task tabInOutExplicit()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(ECONOMIC_FREEDOM);
            p.parseTab();
            p.printTab();
            actual = await p.processToString();
        }

        Assert.Equal(ECONOMIC_FREEDOM, actual);
    }

    [Fact]
    public async Task tabHasHeader()
    {
        const String source = "headerA\theaderB\nvalueA1\tvalueB1";
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(source);
            p.parseTab(hasHeader: true);
            p.sed("[AB]", "X");
            actual = await p.processToString();
        }

        const String expected = "headerA\theaderB\nvalueX1\tvalueX1";
        Assert.Equal(expected, actual);            
    }
        
    [Fact]
    public async Task withColumnsFilter()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.withColumns(pn => pn.grep("titan", caseSensitive: false), 3);
            actual = await p.processToString();
        }

        Assert.Equal(PLANETS_GODS_TITANS, actual);

        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.withColumns(pn => pn.grep("titan", caseSensitive: false), 1, 2);
            actual = await p.processToString();
        }

        Assert.Equal("", actual);
    }

    [Fact]
    public async Task withColumnsTransformer()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.withColumns(pn => pn.sed("t", "X", "gi"), 3);
            actual = await p.processToString();
        }

        Assert.Equal(@"Gaia,Terra,""MoXher goddess of Xhe earXh""", actual);

        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.withColumns(pn => pn.sed("t", "X", "gi"), 1, 2);
            actual = await p.processToString();
        }

        Assert.Equal(@"Gaia,Xerra,""Mother goddess of the earth""", actual);
    }
        
    [Fact]
    public async Task columnFilter()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.columnFilter(3, new Grep("titan", caseSensitive: false));
            actual = await p.processToString();
        }

        Assert.Equal(PLANETS_GODS_TITANS, actual);

        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.columnFilter(1, new Grep("titan", caseSensitive: false));
            actual = await p.processToString();
        }

        Assert.Equal("", actual);
    }

    [Fact]
    public async Task columnTransformer()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.columnTransformer(3, new SedReplace("t", "X", "gi"));
            actual = await p.processToString();
        }

        Assert.Equal(@"Gaia,Terra,""MoXher goddess of Xhe earXh""", actual);

        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.parseCsv();
            p.columnTransformer(2, new SedReplace("t", "X", "gi"));
            actual = await p.processToString();
        }

        Assert.Equal(@"Gaia,Xerra,""Mother goddess of the earth""", actual);
    }
        
    [Fact]
    public async Task withBoth()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.withColumns(sub =>
            {
                sub.grep("titan", caseSensitive: false);
                sub.sed("n", "X", "gi");
            }, 3);
            actual = await p.processToString();
        }

        // When grouped proper, 'n' will only be replaced in 3rd column
        String expected =
            @"Cronus,Saturn,""TitaX sky god, supreme ruler of the titaXs""
Uranus,Uranus,""Father of the TitaXs""
";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task groupFilters()
    {
        String actual;

        // Verify 0 works
        await using (Pnyx p = new Pnyx())
        {
            p.readString(EARTH);
            p.and(_ => { });
            actual = await p.processToString();
        }

        Assert.Equal(EARTH, actual);

        // Verify 1 works
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.and(pn => pn.grep("titan", caseSensitive: false));
            actual = await p.processToString();
        }

        Assert.Equal(PLANETS_GODS_TITANS, actual);

        // Verify X works
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.and(pn =>
            {
                pn.grep("ti");
                pn.grep("sky");
            });
            actual = await p.processToString();
        }

        Assert.Equal(@"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
", actual);
    }

    [Fact]
    public async Task beforeAfterLine()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.beforeAfterFilter(1, 1, pn => pn.grep("mercury", caseSensitive: false));
            actual = await p.processToString();
        }
            
        const String expected =                 
            @"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,""God of the sea and earthquakes""
";                
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task beforeAfterRow()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.beforeAfterFilter(1, 1, pn => pn.grep("mercury", caseSensitive: false));
            actual = await p.processToString();
        }
            
        const String expected =                 
            @"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,""God of the sea and earthquakes""
";                
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task or()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.or(pn =>
            {
                pn.grep("Cronus");
                pn.grep("Uranus");
            });
            actual = await p.processToString();
        }
            
        Assert.Equal(PLANETS_GODS_TITANS, actual);            
    }        

    [Fact]
    public async Task not()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.not(pn =>
            {
                pn.grep("god", caseSensitive: false);
            });
            actual = await p.processToString();
        }

        const String expected = @"Uranus,Uranus,""Father of the Titans""
";
        Assert.Equal(expected, actual);            
    }        

    [Fact]
    public async Task xor()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.xor(pn =>
            {
                pn.grep("titan", caseSensitive: false);
                pn.grep("Cronus", caseSensitive: false);
            });
            actual = await p.processToString();
        }

        const String expected = @"Uranus,Uranus,""Father of the Titans""
";
        Assert.Equal(expected, actual);            
    }        

    [Fact]
    public async Task catLine()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.cat(pn =>
            {
                pn.readString(EARTH);
                pn.readString(EARTH);                    
            });
            actual = await p.processToString();
        }

        const String expected =
            @"Gaia,Terra,""Mother goddess of the earth""
Gaia,Terra,""Mother goddess of the earth""";
            
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task catRow()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.cat(p2 =>
            {
                p2.asCsv(p3 =>
                {
                    p3.readString(EARTH);
                    p3.readString(EARTH);
                });
            });
            actual = await p.processToString();
        }

        const String expected =
            @"Gaia,Terra,""Mother goddess of the earth""
Gaia,Terra,""Mother goddess of the earth""";
            
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task headLine()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(MAGNA_CARTA);
            p.head(2);
            actual = await p.processToString();
        }

        const String expected =
            @"KNOW THAT BEFORE GOD, for the health of our soul and those of our ancestors and heirs, 
to the honour of God, the exaltation of the holy Church, and the better ordering of our
";
            
        Assert.Equal(expected, actual);                       
    }

    [Fact]
    public async Task headRow()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.head(2);
            actual = await p.processToString();
        }

        const String expected =
            @"Aphrodite,Venus,""Goddess of love and beauty""
Ares,Mars,""Hated god of war""
"; 
        Assert.Equal(expected, actual);                       
    }

    [Fact]
    public async Task tailLine()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.tailStream(p2 => p2.readString(MAGNA_CARTA), 2);
            actual = await p.processToString();
        }

        const String expected =
            @"Matthew Fitz Herbert, Thomas Basset, Alan Basset, Philip Daubeny, Robert de Roppeley,
John Marshal, John Fitz Hugh, and other loyal subjects:";
            
        Assert.Equal(expected, actual);
            

        // Uses buffer instead of manipulating the stream
        await using (Pnyx p = new Pnyx())
        {
            p.readString(MAGNA_CARTA);
            p.tail(2);
            actual = await p.processToString();
        }
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task tailStreamRow()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.tailStream(p2 => p2.readString(PLANETS_GODS), 2);
            p.parseCsv();
            actual = await p.processToString();
        }

        const String expected =
            @"Uranus,Uranus,""Father of the Titans""
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
"; 
        Assert.Equal(expected, actual);                       
            

        // Uses buffer instead of manipulating the stream
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.tail(2);
            actual = await p.processToString();
        }
        Assert.Equal(expected, actual);                       
    }

    [Fact]
    public async Task teeLine()
    {
        StringBuilder capture = new StringBuilder();
        String actualA;

        Pnyx p = new Pnyx();
        Pnyx? teeP = null;
        await using (p)
        {
            p.readString(PLANETS_GODS_TITANS);
            p.tee(pn =>
            {
                teeP = pn;
                pn.captureText(capture);
            });
            actualA = await p.processToString();
        }
        String actualB = capture.ToString();
            
        Assert.Equal(actualA, PLANETS_GODS_TITANS);
        Assert.Equal(actualB, PLANETS_GODS_TITANS);
        Assert.Equal(FluentState.Disposed, p.state);
        Assert.Equal(FluentState.Disposed, teeP?.state);
    }

    [Fact]
    public async Task teeRow()
    {
        MemoryStream capture = new MemoryStream();
        String actualA;

        Pnyx p = new Pnyx();
        Pnyx? teeP = null;
        await using (p)
        {
            p.readString(PLANETS_GODS_FORMAT_ISSUES);
            p.parseCsv(strict: false);
            p.tee(pn =>
            {
                teeP = pn;
                pn.writeStream(capture);
            });
            actualA = await p.processToString();
        }
        String actualB = p.streamInformation.getOutputEncoding().GetString(capture.ToArray());
            
        Assert.Equal(actualA, PLANETS_GODS);
        Assert.Equal(actualB, PLANETS_GODS);
        Assert.Equal(FluentState.Disposed, p.state);
        Assert.Equal(FluentState.Disposed, teeP?.state);
    }

    [Fact]
    public async Task teeServile()
    {
        MemoryStream capture = new MemoryStream();

        Pnyx? teeP = null;            
        Pnyx p = new Pnyx();
        p.readString(PLANETS_GODS_TITANS);
        p.tee(pn =>
        {
            teeP = pn;
            pn.writeStream(capture);
        });
            
        Assert.Equal(FluentState.CompiledServile, teeP?.state);
        await Assert.ThrowsAsync<IllegalStateException>(() => teeP?.process());            
    }
        
    [Fact]
    public async Task rowFilterShimOr()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS_TITANS);
            p.parseCsv();
            p.grep("u");                        // U needs to be present in any column
            actual = await p.processToString();
        }

        Assert.Equal(PLANETS_GODS_TITANS, actual);
    }
        
    [Fact]
    public async Task rowFilterShimAnd()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS_TITANS);
            p.parseCsv();
            p.shimAnd(p2 =>
            {
                p2.grep("u");                    // U must be present in each column
            });
            actual = await p.processToString();
        }

        const String expected = @"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true,  false, false, false, "MaxWidth\t1\t11\t4")]
    [InlineData(false, true,  false, false, "Header\t1\tHong Kong\t90.2")]
    [InlineData(false, false, true,  false, "MinWidth\t1\t9\t4")]
    [InlineData(false, false, false, true,  "Nullable\tnot null\tnot null\tnot null")]
    public async Task columnDefinition(bool maxWidth, bool hasHeaderRow, bool minWidth, bool nullable, String expected)
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(ECONOMIC_FREEDOM);
            p.parseTab(hasHeader: hasHeaderRow);
            p.columnDefinition(null, maxWidth, hasHeaderRow, minWidth, nullable, swapRowsAndColumns: false);
            actual = await p.processToString();
        }
        expected += Environment.NewLine;
        Assert.Equal(expected, actual);
    }
        
    [Theory]
    [InlineData(MAGNA_CARTA, 13)]
    [InlineData(PLANETS_GODS, 7)]
    [InlineData(ECONOMIC_FREEDOM, 5)]
    public async Task count(String source, int expected)
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(source);
            p.countLines();
            actual = await p.processToString();
        }

        Assert.Equal(expected, int.Parse(actual));            
    }

    [Fact]
    public async Task countRow()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.countLines();
            actual = await p.processToString();
        }

        Assert.Equal("7,7,7", actual.TrimEnd());                        
    }

    [Fact]
    public async Task readLineFunc()
    {
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.setSettings(outputNewline: "\n");
            p.readLine(() => new[] {"a","b","c"});
            actual = await p.processToString();
        }
            
        Assert.Equal("a\nb\nc", actual);
    }

    [Fact]
    public async Task readRowFunc()
    {
        Func<IEnumerable<List<String?>>> source = () => new[]
        {
            new List<String?> {"a", "1"},
            new List<String?> {"b", "2"},
            new List<String?> {"c", "3"}
        };
            
        String actual;
        await using (Pnyx p = new Pnyx())
        {
            p.setSettings(outputNewline: "\n");
            p.readRow(source);
            actual = await p.processToString();
        }
            
        String expected = "a,1\nb,2\nc,3";
        Assert.Equal(expected, actual);
            
        await using (Pnyx p = new Pnyx())
        {
            p.setSettings(outputNewline: "\n");
            p.readRow(source, header: () => new List<string> { "Letter", "Number" });
            actual = await p.processToString();
        }

        expected = "Letter,Number\n" + expected;
        Assert.Equal(expected, actual);
    }
}