using System;
using System.Text;
using pnyx.net.errors;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.fluent
{
    public class PnyxHeaderNameTest
    {
        [Theory]
        [InlineData(3, "John,Header2,Header3", "John")]
        [InlineData(3, "John,Matthew,Header3", "John", "Matthew")]
        [InlineData(3, "John,Matthew,Mark", "John", "Matthew", "Mark")]
        [InlineData(3, "John,Matthew,Mark", "John", "Matthew", "Mark", "Luke")]
        [InlineData(3, "Header1,Header2,John", 3, "John")]
        [InlineData(3, "Header1,John,Matthew", 2, "John", "Matthew")]
        [InlineData(3, "Matthew,Header2,John", 3, "John", 1, "Matthew")]
        public void headerNames(int headers, String expected, params Object[] columnNumbersAndNames)
        {
            StringBuilder source = new StringBuilder();
            for (int i = 1; i <= headers; i++)
                source.Append("Header").Append(i).Append(",");
            source.Length -= 1;
                        
            String actual;
            using (Pnyx p = new Pnyx())
            {
                p.readString(source.ToString());
                p.parseCsv(hasHeader: true);
                p.headerNames(columnNumbersAndNames);
                actual = p.processToString();
            }

            Assert.Equal(expected, actual);            
        }

        [Theory]
        [InlineData("Column number must be 1 or greater: 0", 0)]
        [InlineData("Value should be either an integer index or a header name, but found value '8.545' of type 'Double'", 8.545)]
        [InlineData("Null is not a valid parameter", 1, null, 2)]
        [InlineData("At least one name is required", 1, 2, 3)]
        [InlineData("At least one name is required")]
        public void headerNamesError(String message, params Object[] columnNumbersAndNames)
        {
            using (Pnyx p = new Pnyx())
            {
                p.readString("bb");
                p.parseCsv(hasHeader: true);
                InvalidArgumentException exception = Assert.Throws<InvalidArgumentException>(() => p.headerNames(columnNumbersAndNames));
                Assert.Equal(message, exception.Message);
            }
        }
        
    }
}