using System;
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

        private const String EARTH = @"Gaia,Terra,""Mother goddess of the earth""";
        
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
                p.grep("god");
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
                p.sedAppend("The Lord is my shepherd");
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
                p.rowCsv();
                actual = p.processToString();
            }
            
            Assert.Equal(PLANETS_GODS, actual);
        }        
        
        [Fact]
        public void rowFilter()
        {
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(PLANETS_GODS);
                p.rowCsv();
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
                p.rowCsv();
                p.sed("Ter.*", "Forma", "g");
                actual = p.processToString();
            }

            const String expected = "Gaia,Forma,\"Mother goddess of the earth\"";                  
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void rowBuffering()
        {
            using (Pnyx p = new Pnyx())
            {
                p.streamInformation.setDefaultNewline(NewLineEnum.Windows);
                p.readString(EARTH);
                p.rowCsv();
                p.sedAppend("The Lord is my shepherd");
                
                Assert.Throws<NotImplementedException>(() => p.processToString());
            }
        }
        
    }
}