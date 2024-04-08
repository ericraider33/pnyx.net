using System;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class TextUtilTest
    {
        [Fact]
        public void Concat()
        {
            Assert.Equal("", TextUtil.concate(","));
            Assert.Equal("", TextUtil.concate(",", null));
            Assert.Equal("", TextUtil.concate(",", null, null));
            Assert.Equal("", TextUtil.concate(",", ""));
            Assert.Equal("", TextUtil.concate(",", "", ""));

            Assert.Equal("a", TextUtil.concate(",", "a"));
            Assert.Equal("a,b", TextUtil.concate(",", "a","b"));
            Assert.Equal("a,b,c", TextUtil.concate(",", "a","b","c"));

            Assert.Equal("a", TextUtil.concate(",", "a", null));
            Assert.Equal("a", TextUtil.concate(",", "a", ""));
            Assert.Equal("a,b", TextUtil.concate(",", "a", null, "b"));
            Assert.Equal("a,b", TextUtil.concate(",", "a", "", "b"));
            Assert.Equal("a,b,c", TextUtil.concate(",", null, "a", "b", "c"));
            Assert.Equal("a,b,c", TextUtil.concate(",", "a", "", "b", "c"));
            Assert.Equal("a,b,c", TextUtil.concate(",", "a", "b", "c", null));
        }

        [Fact]
        public void TruncAtWhitespace()
        {
            Assert.Equal("", TextUtil.truncAtWhitespace("", 5));
            Assert.Equal("qce", TextUtil.truncAtWhitespace("qce", 5));
            Assert.Equal("qc jp", TextUtil.truncAtWhitespace("qc jp", 5));
            Assert.Equal("aje", TextUtil.truncAtWhitespace("aje jpe", 5));
            Assert.Equal("12345", TextUtil.truncAtWhitespace("1234567", 5));
            Assert.Equal("12345", TextUtil.truncAtWhitespace("12345 78", 5));
            Assert.Equal("12 45", TextUtil.truncAtWhitespace("12 45 78", 5));
            Assert.Equal("12345", TextUtil.truncAtWhitespace("12345 78 0", 5));
        }

        [Fact]
        public void IsAllNumbers()
        {
            Assert.False(TextUtil.isAllNumbers(null));
            Assert.False(TextUtil.isAllNumbers(""));
            Assert.False(TextUtil.isAllNumbers("a"));
            Assert.False(TextUtil.isAllNumbers(" 1"));
            Assert.False(TextUtil.isAllNumbers("1 "));
            Assert.False(TextUtil.isAllNumbers("1.1"));
            Assert.False(TextUtil.isAllNumbers("1a"));
            Assert.False(TextUtil.isAllNumbers("a1"));
            Assert.True(TextUtil.isAllNumbers("1"));
            Assert.True(TextUtil.isAllNumbers("12"));
            Assert.True(TextUtil.isAllNumbers("123"));
            Assert.True(TextUtil.isAllNumbers("123456789"));
        }

        [Fact]
        public void HasAnyLetters()
        {
            Assert.True(TextUtil.hasAnyLetters("a"));
            Assert.True(TextUtil.hasAnyLetters("1a"));
            Assert.True(TextUtil.hasAnyLetters("a2"));
            Assert.True(TextUtil.hasAnyLetters("1a2"));
            Assert.False(TextUtil.hasAnyLetters(null));
            Assert.False(TextUtil.hasAnyLetters(""));
            Assert.False(TextUtil.hasAnyLetters("1"));
            Assert.False(TextUtil.hasAnyLetters("1,2"));
        }

        [Fact]
        public void HasAnyNumbers()
        {
            Assert.True(TextUtil.hasAnyNumbers("1fgdf"));
            Assert.False(TextUtil.hasAnyNumbers(" "));
            Assert.True(TextUtil.hasAnyNumbers("f2f"));
            Assert.True(TextUtil.hasAnyNumbers("1a2"));
            Assert.False(TextUtil.hasAnyNumbers(null));
            Assert.False(TextUtil.hasAnyNumbers(""));
            Assert.True(TextUtil.hasAnyNumbers("1324543"));
            Assert.True(TextUtil.hasAnyNumbers("1,2"));
            Assert.True(TextUtil.hasAnyNumbers("dfsdg3"));
        }

        [Fact]
        public void HasAnyWhiteSpace()
        {
            Assert.True(TextUtil.hasAnyWhiteSpace(" "));
            Assert.True(TextUtil.hasAnyWhiteSpace("\n"));
            Assert.True(TextUtil.hasAnyWhiteSpace("\t"));
            Assert.True(TextUtil.hasAnyWhiteSpace("\r"));
            Assert.True(TextUtil.hasAnyWhiteSpace("a a"));
            Assert.True(TextUtil.hasAnyWhiteSpace("b\nb"));
            Assert.True(TextUtil.hasAnyWhiteSpace("c\tc"));
            Assert.True(TextUtil.hasAnyWhiteSpace("d\rd"));
            Assert.False(TextUtil.hasAnyWhiteSpace("aa"));
        }

        [Fact]
        public void HasSpecial()
        {
            Assert.False(TextUtil.hasAnySpecial(" \t\n\r0123456789abcdABCDEF"));
            Assert.False(TextUtil.hasAnySpecial(""));
            Assert.False(TextUtil.hasAnySpecial(null));
            Assert.True(TextUtil.hasAnySpecial("a!b"));
            Assert.True(TextUtil.hasAnySpecial("a=b"));
        }

        [Fact]
        public void HasConsecutiveCharacters()
        {
            Assert.False(TextUtil.hasConsecutiveCharacters(null));
            Assert.False(TextUtil.hasConsecutiveCharacters(""));
            Assert.False(TextUtil.hasConsecutiveCharacters("abc123", 3));
            Assert.False(TextUtil.hasConsecutiveCharacters("aa", 3));
            Assert.True(TextUtil.hasConsecutiveCharacters("aaa", 3));
            Assert.True(TextUtil.hasConsecutiveCharacters("aaaa", 3));

            Assert.False(TextUtil.hasConsecutiveCharacters("aabb", 3));
            Assert.True(TextUtil.hasConsecutiveCharacters("aabbb", 3));

            Assert.False(TextUtil.hasConsecutiveCharacters("aaxbb", 3));
            Assert.True(TextUtil.hasConsecutiveCharacters("aaxbbb", 3));
            
            Assert.False(TextUtil.hasConsecutiveCharacters("aaxbb", 4));
            Assert.False(TextUtil.hasConsecutiveCharacters("aaxbbb", 4));            
            Assert.True(TextUtil.hasConsecutiveCharacters("aaxbbbb", 4));            
        }

        [Fact]
        public void Format()
        {
            Assert.Equal("5", string.Format("{0}", 5));
            Assert.Equal("05", string.Format("{0:00}", 5));
            Assert.Equal("5.4", string.Format("{0:0.0}", 5.41));
        }

        [Fact]
        public void IsDecimal()
        {            
            verifyIsDecimal("-1", -1m);
            verifyIsDecimal("-185", -185m);
            verifyIsDecimal("1", 1m);
            verifyIsDecimal("185", 185m);
            verifyIsDecimal("1.55", 1.55m);
            verifyIsDecimal("-1.55", -1.55m);

            // partial formats (ASC-648)
            verifyIsDecimal("-199.", -199m, "-199");
            verifyIsDecimal("199.", 199m, "199");
            verifyIsDecimal("-0", 0m, "0");

            // partial formats (ASC-1299)
            verifyIsDecimal("+0", 0m, "0");
            verifyIsDecimal("+6", 6m, "6");
            verifyIsDecimal("+7.", 7m, "7");
            verifyIsDecimal("+7.9", 7.9m, "7.9");

            verifyIsDecimal(null, null);
            verifyIsDecimal("", null);
            verifyIsDecimal("-1x", null);
            verifyIsDecimal("-1,1", null);
            verifyIsDecimal("1x", null);
            verifyIsDecimal("1,1", null);
            verifyIsDecimal("a", null);
            verifyIsDecimal("a1", null);
            verifyIsDecimal("1a", null);        
        }

        private void verifyIsDecimal(string text, decimal? expected, String clean = null)
        {
            bool isDecimal = TextUtil.isDecimal(text);
            Assert.Equal(expected.HasValue, isDecimal);

            if (!isDecimal)
                return;

            decimal actual = decimal.Parse(text);
            Assert.Equal(expected.Value, actual);
            Assert.Equal(clean ?? text, actual.ToString());
        }

        [Fact]
        public void RemoveBeginning()
        {
            Assert.Null(TextUtil.removeBeginning(null, null));
            Assert.Equal("hi", TextUtil.removeBeginning("hi", null));
            Assert.Equal("Big", TextUtil.removeBeginning("Big", "ig"));
            Assert.Equal("", TextUtil.removeBeginning("Big", "Big"));
            Assert.Equal("Show", TextUtil.removeBeginning("BigShow", "Big"));
        }

        [Fact]
        public void RemoveEnding()
        {
            Assert.Null(TextUtil.removeEnding(null, null));
            Assert.Equal("hi", TextUtil.removeEnding("hi", null));
            Assert.Equal("BigShow", TextUtil.removeEnding("BigShow", "big"));
            Assert.Equal("", TextUtil.removeEnding("BigShow", "BigShow"));
            Assert.Equal("Big", TextUtil.removeEnding("BigShow", "Show"));
        }

        [Fact]
        public void IsMixedCase()
        {
            Assert.False(TextUtil.isMixedCase(null));
            Assert.False(TextUtil.isMixedCase(""));
            Assert.False(TextUtil.isMixedCase("456"));
            Assert.False(TextUtil.isMixedCase("asdf456"));
            Assert.False(TextUtil.isMixedCase("ASDF"));
            Assert.True(TextUtil.isMixedCase("ASdf"));
            Assert.True(TextUtil.isMixedCase("A1 d"));
        }

        [Fact]
        public void TrimQuotes()
        {
            Assert.Null(TextUtil.trimQuotes(null));
            Assert.Equal("", TextUtil.trimQuotes(""));
            Assert.Equal("", TextUtil.trimQuotes("\""));
            Assert.Equal("", TextUtil.trimQuotes("\"\""));
            Assert.Equal("x", TextUtil.trimQuotes("\"x\""));
            Assert.Equal("x", TextUtil.trimQuotes("x\""));
            Assert.Equal("x", TextUtil.trimQuotes("\"x"));
            Assert.Equal("xx", TextUtil.trimQuotes("\"xx\""));
            Assert.Equal("xx", TextUtil.trimQuotes("xx\""));
            Assert.Equal("xx", TextUtil.trimQuotes("\"xx"));
            Assert.Equal("xxx", TextUtil.trimQuotes("xxx"));
            Assert.Equal("xxx", TextUtil.trimQuotes("\"xxx\""));
            Assert.Equal("\"xxx", TextUtil.trimQuotes("\"\"xxx\""));
            Assert.Equal("xxx\"", TextUtil.trimQuotes("\"xxx\"\""));
        }

        [Fact]
        public void Trunc()
        {
            Assert.Null(TextUtil.trunc(null, 3));
            Assert.Equal("", TextUtil.trunc("", 3));
            Assert.Equal("a", TextUtil.trunc("a", 3));
            Assert.Equal("abc", TextUtil.trunc("abc", 3));
            Assert.Equal("abc", TextUtil.trunc("abcdef", 3));
        }

        [Fact]
        public void TruncRight()
        {
            Assert.Null(TextUtil.truncRight(null, 3));
            Assert.Equal("", TextUtil.truncRight("", 3));
            Assert.Equal("a", TextUtil.truncRight("a", 3));
            Assert.Equal("abc", TextUtil.truncRight("abc", 3));
            Assert.Equal("def", TextUtil.truncRight("abcdef", 3));
        }

        [Fact]
        public void ProtectedFromExcel()
        {
            Assert.Null(TextUtil.protectedFromExcel(null));
            Assert.Equal("", TextUtil.protectedFromExcel(""));
            Assert.Equal("1", TextUtil.protectedFromExcel("1"));

            Assert.Equal("'1-1", TextUtil.protectedFromExcel("1-1"));
            Assert.Equal("'1-20", TextUtil.protectedFromExcel("1-20"));
            Assert.Equal("'11-20", TextUtil.protectedFromExcel("11-20"));

            Assert.Equal("1-200", TextUtil.protectedFromExcel("1-200"));
            Assert.Equal("100-2", TextUtil.protectedFromExcel("100-2"));
        }

        [Fact]
        public void RemoveParts()
        {
            Assert.Null(TextUtil.removeParts(null, 0, 0));
            Assert.Equal("", TextUtil.removeParts("", 0, 0));
            Assert.Equal("", TextUtil.removeParts("a", 0, 0));
            Assert.Equal("b", TextUtil.removeParts("ab", 0, 0));
            Assert.Equal("a", TextUtil.removeParts("ab", 1, 1));
            Assert.Equal("bc", TextUtil.removeParts("abc", 0, 0));
            Assert.Equal("ac", TextUtil.removeParts("abc", 1, 1));
            Assert.Equal("ab", TextUtil.removeParts("abc", 2, 2));
            Assert.Equal("cd", TextUtil.removeParts("abcd", 0, 1));
            Assert.Equal("ad", TextUtil.removeParts("abcd", 1, 2));
            Assert.Equal("ab", TextUtil.removeParts("abcd", 2, 3));
        }

        [Fact]
        public void RemoveParentheses()
        {
            Assert.Null(TextUtil.removeParentheses(null));
            Assert.Equal("", TextUtil.removeParentheses(""));
            Assert.Equal("freedom", TextUtil.removeParentheses("freedom"));
            Assert.Equal("freedom(", TextUtil.removeParentheses("freedom("));
            Assert.Equal("freedom)", TextUtil.removeParentheses("freedom)"));

            Assert.Equal("freedom", TextUtil.removeParentheses("freedom()"));
            Assert.Equal("freedom", TextUtil.removeParentheses("freedom()()()"));
            Assert.Equal("freedom", TextUtil.removeParentheses("freedom(age)"));

            Assert.Equal("freedom of ", TextUtil.removeParentheses("freedom(age) of (destruction)"));

            Assert.Equal("freedom", TextUtil.removeParentheses("freedom((((()"));
            Assert.Equal("freedom))", TextUtil.removeParentheses("freedom()))"));
        }

        [Fact]
        public void EndsWith()
        {
            Assert.True("a".endsWithIgnoreCase("a"));
            Assert.True("a".endsWithIgnoreCase("A"));
            Assert.True("A".endsWithIgnoreCase("a"));
            Assert.True("Yo ma".endsWithIgnoreCase("ma"));
            Assert.True("Yo MA".endsWithIgnoreCase("ma"));

            Assert.False(TextUtil.endsWithIgnoreCase("", "a"));
            Assert.False(TextUtil.endsWithIgnoreCase(null, "a"));
            Assert.False("Yo ma no hands".endsWithIgnoreCase("ma"));
        }
    }
}