using System;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class NameUtilTest
    {
        [Fact]
        public void Name()
        {
            Assert.False(NameUtil.isName(null));
            Assert.False(NameUtil.isName(""));
            Assert.False(NameUtil.isName(" "));
            Assert.False(NameUtil.isName("5"));
            Assert.False(NameUtil.isName(" Jimbo "));
            Assert.False(NameUtil.isName("5 Jimbo"));
            Assert.False(NameUtil.isName("5Jimbo"));
            Assert.False(NameUtil.isName("Jimbo5"));

            Assert.True(NameUtil.isName("Jimbo"));
            Assert.True(NameUtil.isName("La Jimbo"));
            Assert.True(NameUtil.isName("La  Jimbo"));
        }

        [Fact]
        public void ToTitleCase()
        {
            Assert.Equal("Jimbo", "JIMBO".toTitleCase());
            Assert.Equal("Jimbo", "Jimbo".toTitleCase());
            Assert.Equal("Jimbo", "jimbo".toTitleCase());
            Assert.Equal("Last, First", "last, first".toTitleCase());
            Assert.Null(NameUtil.toTitleCase(null));
        }

        [Fact]
        public void AsName()
        {
            Assert.Null(NameUtil.asName(null));
            Assert.Null(NameUtil.asName(""));
            Assert.Null(NameUtil.asName(" "));

            Assert.Null(NameUtil.asName(" 7 "));
            Assert.Null(NameUtil.asName(" @#$%^ "));

            Assert.Equal("Jimbo", NameUtil.asName(" Jimbo "));
            Assert.Equal("Jimbo", NameUtil.asName(" Jimbo 5 "));
            Assert.Equal("Jimbo", NameUtil.asName("5 Jimbo 5 "));

            Assert.Equal("Jimbo", NameUtil.asName("JIMBO"));
            Assert.Equal("Jimbo", NameUtil.asName("jimbo"));
            Assert.Equal("eRic", NameUtil.asName("eRic"));
            Assert.Equal("eriC", NameUtil.asName("eriC"));
        }

        [Theory]
        [InlineData("", null, null, null, null)]
        [InlineData("123456", null, null, null, null)]
        [InlineData("Jimbo", "Jimbo", null, null, null)]
        [InlineData("Jimbo Rankin", "Jimbo", null, "Rankin", null)]
        [InlineData("Jimbo Otto Rankin", "Jimbo", "Otto", "Rankin", null)]
        [InlineData("Jimbo Otto von Rankin", "Jimbo", "Otto", "Von Rankin", null)]

        [InlineData("Jimbo jr Otto Rankin", "Jimbo", "Otto", "Rankin", "Jr")]
        [InlineData("Jimbo Otto jr Rankin", "Jimbo", "Otto", "Rankin", "Jr")]
        [InlineData("Jimbo Otto Rankin jr", "Jimbo", "Otto", "Rankin", "Jr")]

        // Suffix parsing, skipped when firstname matches suffix
        [InlineData("JR Rankin", "Jr", null, "Rankin", null)]
        [InlineData("JD MOLOY", "Jd", null, "Moloy", null)]
        public void parseFullName(string source, string firstName, string middleName, string lastName, String suffix)
        {
            Name name = NameUtil.parseFullName(source);
            if (firstName == null)
            {
                Assert.Null(name);
                return;
            }
            Assert.Equal(firstName, name.firstName);
            Assert.Equal(middleName, name.middleName);
            Assert.Equal(lastName, name.lastName);
            Assert.Equal(suffix, name.suffix);
        }

        [Fact]
        public void LastFirstMiddleName()
        {
            // Edge case testing
            verifyLastFirstMiddleName("", null, null, null);
            verifyLastFirstMiddleName("11", null, null, null);
            verifyLastFirstMiddleName("11, 11", null, null, null);
            verifyLastFirstMiddleName("Jimbo, 11", null, null, null);
            verifyLastFirstMiddleName("11, Jimbo", null, null, null);

            verifyLastFirstMiddleName("Rankin", null, null, "Rankin");
            verifyLastFirstMiddleName("Rankin, Jimbo", "Jimbo", null, "Rankin");
            verifyLastFirstMiddleName("Rankin, Jimbo Otto", "Jimbo", "Otto", "Rankin");
            verifyLastFirstMiddleName("Rankin, Jimbo Otto Von", "Jimbo Otto", "Von", "Rankin");

            // Suffix parsing
            verifyLastFirstMiddleName("Rankin, Jimbo   JR", "Jimbo", null, "Rankin JR");
            verifyLastFirstMiddleName("Rankin, Jimbo Otto MD", "Jimbo", "Otto", "Rankin MD");
            verifyLastFirstMiddleName("Rankin, Jimbo SR Otto Von", "Jimbo Otto", "Von", "Rankin SR");

            // Suffix parsing, skipped when firstname matches suffix
            verifyLastFirstMiddleName("Rankin, JR", "Jr", null, "Rankin");
            verifyLastFirstMiddleName("MOLOY, JD", "Jd", null, "Moloy");            
        }

        private void verifyLastFirstMiddleName(string source, string firstName, string middleName, string lastName)
        {
            Tuple<String, String, String> name = NameUtil.lastFirstMiddleName(source);
            if (firstName == null)
            {
                Assert.Null(name);
                return;
            }
            Assert.Equal(firstName, name.Item1);
            Assert.Equal(middleName, name.Item2);
            Assert.Equal(lastName, name.Item3);
        }

        [Fact]
        public void LastMiddleFirstName()
        {
            // Edge case testing
            verifyLastFirstMiddleName("", null, null, null);
            verifyLastFirstMiddleName("11", null, null, null);
            verifyLastFirstMiddleName("11, 11", null, null, null);
            verifyLastFirstMiddleName("Jimbo, 11", null, null, null);
            verifyLastFirstMiddleName("11, Jimbo", null, null, null);

            verifyLastMiddleFirstName("Rankin", null, null, "Rankin");
            verifyLastMiddleFirstName("Rankin, Jimbo", "Jimbo", null, "Rankin");
            verifyLastMiddleFirstName("Rankin, Otto Jimbo", "Jimbo", "Otto", "Rankin");
            verifyLastMiddleFirstName("Rankin, Otto Von Jimbo", "Von Jimbo", "Otto", "Rankin");

            verifyLastMiddleFirstName("POWELL, J Robin", "Robin", "J", "Powell");

            // Suffix parsing
            verifyLastMiddleFirstName("Rankin, Jimbo JR", "Jimbo", null, "Rankin JR");
            verifyLastMiddleFirstName("Rankin, Otto Jimbo MD", "Jimbo", "Otto", "Rankin MD");
            verifyLastMiddleFirstName("Rankin, Otto SR Jimbo Von", "Jimbo Von", "Otto", "Rankin SR");

            // Suffix parsing, skipped when firstname matches suffix
            verifyLastMiddleFirstName("Rankin, JR", "Jr", null, "Rankin");
            verifyLastMiddleFirstName("MOLOY, JD", "Jd", null, "Moloy");
        }

        private void verifyLastMiddleFirstName(string source, string firstName, string middleName, string lastName)
        {
            Tuple<String, String, String> name = NameUtil.lastMiddleFirstName(source);
            if (firstName == null)
            {
                Assert.Null(name);
                return;
            }
            Assert.Equal(firstName, name.Item1);
            Assert.Equal(middleName, name.Item2);
            Assert.Equal(lastName, name.Item3);
        }

        [Fact]
        public void ExtractNameString()
        {
            Assert.Null(NameUtil.extractNameString(null));
            Assert.Null(NameUtil.extractNameString(""));
            Assert.Null(NameUtil.extractNameString(" "));
            Assert.Null(NameUtil.extractNameString("  "));
            Assert.Null(NameUtil.extractNameString(" *^*&  "));
            Assert.Null(NameUtil.extractNameString("123"));
            Assert.Null(NameUtil.extractNameString(" 123 "));
            Assert.Equal("a", NameUtil.extractNameString("a"));
            Assert.Equal("a", NameUtil.extractNameString(" a"));
            Assert.Equal("a", NameUtil.extractNameString("a "));
            Assert.Equal("a", NameUtil.extractNameString("a--"));
            Assert.Equal("a", NameUtil.extractNameString("a//"));
            Assert.Equal("a", NameUtil.extractNameString("--a "));
            Assert.Equal("jimbo", NameUtil.extractNameString("jimbo1"));
            Assert.Equal("jimbo", NameUtil.extractNameString("$%^$^jimbo1)()*(&&++="));
            Assert.Equal("jimbo", NameUtil.extractNameString(" jimbo "));
            Assert.Equal("jimbo", NameUtil.extractNameString("1j1i1m1bo1"));
            Assert.Equal("jimbo", NameUtil.extractNameString("jimbo"));
            Assert.Equal("o'clark", NameUtil.extractNameString("o'clark"));
            Assert.Equal("o'clark", NameUtil.extractNameString("o'''clark"));
            Assert.Equal("jimbo otto", NameUtil.extractNameString("jimbo otto"));
            Assert.Equal("jimbo otto", NameUtil.extractNameString(" jimbo otto "));
            Assert.Equal("jimbo otto", NameUtil.extractNameString("jimbo  otto"));
            Assert.Equal("jimbo otto", NameUtil.extractNameString("jimbo   otto"));
            Assert.Equal("jimbo otto", NameUtil.extractNameString("jimbo - otto"));
            Assert.Equal("jimbo otto", NameUtil.extractNameString("jimbo / otto"));
            Assert.Equal("jimbo-otto", NameUtil.extractNameString("jimbo-otto"));
            Assert.Equal("jimbo-otto", NameUtil.extractNameString("jimbo--otto"));
            Assert.Equal("jimbo-otto", NameUtil.extractNameString("-jimbo-otto-"));
        }
        
    }
}