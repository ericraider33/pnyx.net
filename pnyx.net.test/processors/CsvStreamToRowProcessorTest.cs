using System;
using System.Collections.Generic;
using System.IO;
using pnyx.net.errors;
using pnyx.net.processors;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.processors
{
    public class CsvStreamToRowProcessorTest
    {
        [Theory]
        [InlineData("", null, null)]
        [InlineData("a", new String[] { "a" }, null)]
        [InlineData(" ", new String[] { " " }, null)]
        [InlineData("a,b", new String[] { "a", "b" }, null)]
        [InlineData("a,b\n", new String[] { "a", "b" }, null)]
        [InlineData("a,b\nc", new String[] { "a", "b" }, new String[] { "c" })]
        [InlineData("a,b\nc\n", new String[] { "a", "b" }, new String[] { "c" })]
        [InlineData("\"a\"", new String[] { "a" }, null)]
        [InlineData("\"a\",\"b\"", new String[] { "a", "b" }, null)]
        [InlineData("\"a\",\"b\"\n", new String[] { "a", "b" }, null)]
        [InlineData("\"a\",\"b\"\n\"c\"", new String[] { "a", "b" }, new String[] { "c" })]
        [InlineData("\"a\",\"b\"\n\"c\"\n", new String[] { "a", "b" }, new String[] { "c" })]
        [InlineData("\n", new String[0], null)]
        [InlineData("\n\n", new String[0], new String[0])]        
        public void line(string source, string[] rowA, string[] rowB)
        {
            verifyRows(source, rowA, rowB);
        }

        [Theory]
        [InlineData("a\"b", "Line 1 contains a quote that isn't wrapped with quotes")]
        [InlineData("\"a\"b", "Line 1 contains an unexpected character b after quote")]
        [InlineData("\"ab", "File ends with open quotes")]
        public void exceptions(String source, String expectedMessage)
        {
            IllegalStateException err = Assert.Throws<IllegalStateException>(() => parseRows(source));
            Assert.Equal(expectedMessage, err.Message);
        }
        
        [Theory]
        [InlineData("a\"b", new String[] { "a\"b" })]
        [InlineData("\"a\"b", new String[] { "ab" })]
        [InlineData("\"ab", new String[] { "ab" })]
        public void settings(String source, String[] rowA)
        {
            verifyRows(source, rowA, null,
                x =>
                {
                    x.allowStrayQuotes = true;
                    x.allowTextAfterClosingQuote = true;
                    x.terminateQuoteOnEndOfFile = true;
                });
        }
        
        private void verifyRows(string source, string[] rowA, string[] rowB, Action<CsvStreamToRowProcessor> callback = null)
        {
            string[] actualA = null, actualB = null;
            
            List<String[]> rows = parseRows(source, callback);
            if (rows.Count > 0) actualA = rows[0];
            if (rows.Count > 1) actualB = rows[1];
            
            Assert.True(EqualsHelper.areArraysEquals(actualA, rowA));
            Assert.True(EqualsHelper.areArraysEquals(actualB, rowB));
        }

        private List<String[]> parseRows(String source, Action<CsvStreamToRowProcessor> callback = null)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            StreamInformation si = new StreamInformation();
            CaptureRowProcessor capture = new CaptureRowProcessor();
            CsvStreamToRowProcessor csvProcess = new CsvStreamToRowProcessor(si, stream, capture);

            if (callback != null)
                callback(csvProcess);
            
            try
            {
                writer.Write(source);
                writer.Flush();
                stream.Position = 0;

                csvProcess.process();
                return capture.rows;
            }
            finally
            {
                try { writer.Dispose(); } catch (Exception) { }
                try { csvProcess.Dispose(); } catch (Exception) { }
                try { stream.Dispose(); } catch (Exception) { }
            }
        }
        
    }
}