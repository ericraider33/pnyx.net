using System;
using System.IO;
using System.Text;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.fluent
{
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
        public void inOut()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(MAGNA_CARTA);
                actual = p.processToString();
            }

            Assert.Equal(MAGNA_CARTA, actual);
        }

        [Fact]
        public void lineFilter()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(MAGNA_CARTA);
                p.grep("god", caseSensitive: false);
                actual = p.processToString();
            }

            const String expected = @"KNOW THAT BEFORE GOD, for the health of our soul and those of our ancestors and heirs, 
to the honour of God, the exaltation of the holy Church, and the better ordering of our";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void lineTransform()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.sed(",", "\t", "g");
                actual = p.processToString();
            }

            const String expected = "Gaia\tTerra\t\"Mother goddess of the earth\"";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void lineBuffering()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.streamInformation.setDefaultNewline(NewLineEnum.Windows);
                p.readString(EARTH);
                p.sedAppendLine("The Lord is my shepherd");
                actual = p.processToString();
            }

            const String expected = @"Gaia,Terra,""Mother goddess of the earth""
The Lord is my shepherd";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void csvInOut()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                actual = p.processToString();
            }

            Assert.Equal(PLANETS_GODS, actual);
        }

        [Fact]
        public void csvInOutFixFormatting()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS_FORMAT_ISSUES);
                p.parseCsv(strict: false);
                actual = p.processToString();
            }

            Assert.Equal(PLANETS_GODS, actual); // verifies that output is formatted properly, even if input is loose
        }

        [Fact]
        public void csvInOutVariant()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS_FORMAT_ISSUES);
                actual = p.processToString((pnyx, stream) => pnyx.writeCsvStream(stream, strict: false));
            }

            Assert.Equal(PLANETS_GODS, actual);
        }

        [Fact]
        public void asCsv()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.asCsv(pn => pn.readString(PLANETS_GODS_FORMAT_ISSUES), strict: false);                
                actual = p.processToString();
            }

            Assert.Equal(PLANETS_GODS, actual);
        }

        [Fact]
        public void hasHeader()
        {
            const String source = @"headerA,headerB
valueA1,valueB1
";
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(source);
                p.parseCsv(hasHeader: true);
                p.sed("[AB]", "X");
                actual = p.processToString();
            }

            const String expected = @"headerA,headerB
valueX1,valueX1
";
            Assert.Equal(expected, actual);            
        }
        
        [Fact]
        public void rowFilter()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                p.grep("war");
                actual = p.processToString();
            }

            const String expected = "Ares,Mars,\"Hated god of war\"\r\n";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void rowTransform()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.parseCsv();
                p.sed("Ter.*", "Forma", "g");
                actual = p.processToString();
            }

            const String expected = "Gaia,Forma,\"Mother goddess of the earth\"";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void rowBuffering()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.streamInformation.setDefaultNewline(NewLineEnum.Windows);
                p.readString(EARTH);
                p.parseCsv();
                p.sedAppendRow(new[] {"The Lord","is", "my shepherd"});
                actual = p.processToString();
            }

            const String expected = @"Gaia,Terra,""Mother goddess of the earth""
""The Lord"",is,""my shepherd"""; 
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void readImproperState()
        {
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.sed("Ter.*", "Forma", "g");
                Assert.Throws<IllegalStateException>(() => p.readString(EARTH));
            }
        }

        [Fact]
        public void cat()
        {
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                Assert.Throws<IllegalStateException>(() => p.readString(PLANETS_GODS));
            }
        }

        [Fact]
        public void lineToRow()
        {
            String actual;
            const String tabSource = "1) Be fruitful and multiply\t2) and fill the earth and subdue it";
            using (Pnyx p = new Pnyx())
            {
                p.readString(tabSource);
                p.sed("[\t]", ",", "g");
                p.parseCsv(strict: false);
                actual = p.processToString();
            }

            const String expected = @"""1) Be fruitful and multiply"",""2) and fill the earth and subdue it""";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void lineToRowImproperState()
        {
            Assert.Throws<IllegalStateException>(() => new Pnyx().parseCsv());

            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                Assert.Throws<IllegalStateException>(() => p.parseCsv());
            }

            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                p.writeStream(new MemoryStream());
                Assert.Throws<IllegalStateException>(() => p.parseCsv());
            }
        }

        [Fact]
        public void rowToLine()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.parseCsv();
                p.rowToLine();
                p.sed(",", "\t", "g");
                actual = p.processToString();
            }

            const String expected = "Gaia\tTerra\t\"Mother goddess of the earth\"";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void printRow()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.parseCsv();
                p.print("$3,$2,$1");
                actual = p.processToString();
            }

            const String expected = "Mother goddess of the earth,Terra,Gaia";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void printLine()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString("Logic");
                p.print("$0 $0");
                actual = p.processToString();
            }

            const String expected = "Logic Logic";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void columns()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.parseCsv();
                p.selectColumns(3, 2, 1);
                actual = p.processToString();
            }

            const String expected = @"""Mother goddess of the earth"",Terra,Gaia";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void columnToLine()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.parseCsv();
                p.printColumn(2);
                actual = p.processToString();
            }

            const String expected = "Terra";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void tabInOutImplicit()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(ECONOMIC_FREEDOM);
                p.parseTab();
                actual = p.processToString();
            }

            Assert.Equal(ECONOMIC_FREEDOM, actual);
        }

        [Fact]
        public void tabInOutExplicit()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(ECONOMIC_FREEDOM);
                p.parseTab();
                p.printTab();
                actual = p.processToString();
            }

            Assert.Equal(ECONOMIC_FREEDOM, actual);
        }

        [Fact]
        public void tabHasHeader()
        {
            const String source = "headerA\theaderB\nvalueA1\tvalueB1";
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(source);
                p.parseTab(hasHeader: true);
                p.sed("[AB]", "X");
                actual = p.processToString();
            }

            const String expected = "headerA\theaderB\nvalueX1\tvalueX1";
            Assert.Equal(expected, actual);            
        }
        
        [Fact]
        public void withColumnsFilter()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                p.withColumns(pn => pn.grep("titan", caseSensitive: false), 3);
                actual = p.processToString();
            }

            Assert.Equal(PLANETS_GODS_TITANS, actual);

            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                p.withColumns(pn => pn.grep("titan", caseSensitive: false), 1, 2);
                actual = p.processToString();
            }

            Assert.Equal("", actual);
        }

        [Fact]
        public void withColumnsTransformer()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.parseCsv();
                p.withColumns(pn => pn.sed("t", "X", "gi"), 3);
                actual = p.processToString();
            }

            Assert.Equal(@"Gaia,Terra,""MoXher goddess of Xhe earXh""", actual);

            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.parseCsv();
                p.withColumns(pn => pn.sed("t", "X", "gi"), 1, 2);
                actual = p.processToString();
            }

            Assert.Equal(@"Gaia,Xerra,""Mother goddess of the earth""", actual);
        }

        [Fact]
        public void withBoth()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                p.withColumns(sub =>
                {
                    sub.grep("titan", caseSensitive: false);
                    sub.sed("n", "X", "gi");
                }, 3);
                actual = p.processToString();
            }

            // When grouped proper, 'n' will only be replaced in 3rd column
            String expected =
                @"Cronus,Saturn,""TitaX sky god, supreme ruler of the titaXs""
Uranus,Uranus,""Father of the TitaXs""
";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void groupFilters()
        {
            String actual;

            // Verify 0 works
            using (Pnyx p = new Pnyx())
            {
                p.readString(EARTH);
                p.and(pn => { });
                actual = p.processToString();
            }

            Assert.Equal(EARTH, actual);

            // Verify 1 works
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.and(pn => pn.grep("titan", caseSensitive: false));
                actual = p.processToString();
            }

            Assert.Equal(PLANETS_GODS_TITANS, actual);

            // Verify X works
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.and(pn =>
                {
                    pn.grep("ti");
                    pn.grep("sky");
                });
                actual = p.processToString();
            }

            Assert.Equal(@"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
", actual);
        }

        [Fact]
        public void beforeAfterLine()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.beforeAfterFilter(1, 1, pn => pn.grep("mercury", caseSensitive: false));
                actual = p.processToString();
            }
            
            const String expected =                 
                @"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,""God of the sea and earthquakes""
";                
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void beforeAfterRow()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                p.beforeAfterFilter(1, 1, pn => pn.grep("mercury", caseSensitive: false));
                actual = p.processToString();
            }
            
            const String expected =                 
                @"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,""God of the sea and earthquakes""
";                
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void or()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.or(pn =>
                {
                    pn.grep("Cronus");
                    pn.grep("Uranus");
                });
                actual = p.processToString();
            }
            
            Assert.Equal(PLANETS_GODS_TITANS, actual);            
        }        

        [Fact]
        public void not()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.not(pn =>
                {
                    pn.grep("god", caseSensitive: false);
                });
                actual = p.processToString();
            }

            const String expected = @"Uranus,Uranus,""Father of the Titans""
";
            Assert.Equal(expected, actual);            
        }        

        [Fact]
        public void xor()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.xor(pn =>
                {
                    pn.grep("titan", caseSensitive: false);
                    pn.grep("Cronus", caseSensitive: false);
                });
                actual = p.processToString();
            }

            const String expected = @"Uranus,Uranus,""Father of the Titans""
";
            Assert.Equal(expected, actual);            
        }        

        [Fact]
        public void catLine()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.cat(pn =>
                {
                    pn.readString(EARTH);
                    pn.readString(EARTH);                    
                });
                actual = p.processToString();
            }

            const String expected =
                @"Gaia,Terra,""Mother goddess of the earth""
Gaia,Terra,""Mother goddess of the earth""";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void catRow()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.cat(p2 =>
                {
                    p2.asCsv(p3 =>
                    {
                        p3.readString(EARTH);
                        p3.readString(EARTH);
                    });
                });
                actual = p.processToString();
            }

            const String expected =
                @"Gaia,Terra,""Mother goddess of the earth""
Gaia,Terra,""Mother goddess of the earth""";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void headLine()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(MAGNA_CARTA);
                p.head(2);
                actual = p.processToString();
            }

            const String expected =
                @"KNOW THAT BEFORE GOD, for the health of our soul and those of our ancestors and heirs, 
to the honour of God, the exaltation of the holy Church, and the better ordering of our
";
            
            Assert.Equal(expected, actual);                       
        }

        [Fact]
        public void headRow()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                p.head(2);
                actual = p.processToString();
            }

            const String expected =
@"Aphrodite,Venus,""Goddess of love and beauty""
Ares,Mars,""Hated god of war""
"; 
            Assert.Equal(expected, actual);                       
        }

        [Fact]
        public void tailLine()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.tailStream(p2 => p2.readString(MAGNA_CARTA), 2);
                actual = p.processToString();
            }

            const String expected =
                @"Matthew Fitz Herbert, Thomas Basset, Alan Basset, Philip Daubeny, Robert de Roppeley,
John Marshal, John Fitz Hugh, and other loyal subjects:";
            
            Assert.Equal(expected, actual);
            

            // Uses buffer instead of manipulating the stream
            using (Pnyx p = new Pnyx())
            {
                p.readString(MAGNA_CARTA);
                p.tail(2);
                actual = p.processToString();
            }
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void tailStreamRow()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.tailStream(p2 => p2.readString(PLANETS_GODS), 2);
                p.parseCsv();
                actual = p.processToString();
            }

            const String expected =
                @"Uranus,Uranus,""Father of the Titans""
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
"; 
            Assert.Equal(expected, actual);                       
            

            // Uses buffer instead of manipulating the stream
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.parseCsv();
                p.tail(2);
                actual = p.processToString();
            }
            Assert.Equal(expected, actual);                       
        }

        [Fact]
        public void teeLine()
        {
            StringBuilder capture = new StringBuilder();
            String actualA;

            Pnyx p = new Pnyx();
            Pnyx teeP = null;
            using (p)
            {
                p.readString(PLANETS_GODS_TITANS);
                p.tee(pn =>
                {
                    teeP = pn;
                    pn.captureText(capture);
                });
                actualA = p.processToString();
            }
            String actualB = capture.ToString();
            
            Assert.Equal(actualA, PLANETS_GODS_TITANS);
            Assert.Equal(actualB, PLANETS_GODS_TITANS);
            Assert.Equal(FluentState.Disposed, p.state);
            Assert.Equal(FluentState.Disposed, teeP.state);
        }

        [Fact]
        public void teeRow()
        {
            MemoryStream capture = new MemoryStream();
            String actualA;

            Pnyx p = new Pnyx();
            Pnyx teeP = null;
            using (p)
            {
                p.readString(PLANETS_GODS_FORMAT_ISSUES);
                p.parseCsv(strict: false);
                p.tee(pn =>
                {
                    teeP = pn;
                    pn.writeStream(capture);
                });
                actualA = p.processToString();
            }
            String actualB = p.streamInformation.encoding.GetString(capture.ToArray());
            
            Assert.Equal(actualA, PLANETS_GODS);
            Assert.Equal(actualB, PLANETS_GODS);
            Assert.Equal(FluentState.Disposed, p.state);
            Assert.Equal(FluentState.Disposed, teeP.state);
        }

        [Fact]
        public void teeServile()
        {
            MemoryStream capture = new MemoryStream();

            Pnyx teeP = null;            
            Pnyx p = new Pnyx();
            p.readString(PLANETS_GODS_TITANS);
            p.tee(pn =>
            {
                teeP = pn;
                pn.writeStream(capture);
            });
            
            Assert.Equal(FluentState.CompiledServile, teeP.state);
            Assert.Throws<IllegalStateException>(() => teeP.process());            
        }
        
        [Fact]
        public void rowFilterShimOr()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS_TITANS);
                p.parseCsv();
                p.grep("u");                        // U needs to be present in any column
                actual = p.processToString();
            }

            Assert.Equal(PLANETS_GODS_TITANS, actual);
        }
        
        [Fact]
        public void rowFilterShimAnd()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS_TITANS);
                p.parseCsv();
                p.shimAnd(p2 =>
                {
                    p2.grep("u");                    // U must be present in each column
                });
                actual = p.processToString();
            }

            const String expected = @"Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
";
            Assert.Equal(expected, actual);
        }
    }
}