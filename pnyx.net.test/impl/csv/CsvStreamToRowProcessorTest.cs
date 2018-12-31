using System;
using System.Collections.Generic;
using pnyx.net.errors;
using pnyx.net.impl.csv;
using pnyx.net.processors.sources;
using pnyx.net.test.processors;
using pnyx.net.test.util;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.impl
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
        public void line(String source, String[] rowA, String[] rowB)
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
            verifyRows(source, rowA, null, x => { x.setStrict(false); });
        }
        
        private void verifyRows(String source, String[] rowA, String[] rowB, Action<CsvStreamToRowProcessor> callback = null)
        {
            List<String> actualA = null, actualB = null;
            
            List<List<String>> rows = parseRows(source, callback);
            if (rows.Count > 0) actualA = rows[0];
            if (rows.Count > 1) actualB = rows[1];
            
            Assert.True(RowUtil.isEqual(actualA, rowA.asRow()));
            Assert.True(RowUtil.isEqual(actualB, rowB.asRow()));
        }

        private List<List<String>> parseRows(String source, Action<CsvStreamToRowProcessor> callback = null)
        {
            StreamInformation si = new StreamInformation();
            CsvStreamToRowProcessor csvProcess = new CsvStreamToRowProcessor();

            StringStreamFactory wrapper = new StringStreamFactory(source);
            csvProcess.setSource(si, wrapper);
            
            CaptureRowProcessor capture = new CaptureRowProcessor();
            csvProcess.setNextRowProcessor(capture);
            
            if (callback != null)
                callback(csvProcess);
            
            try
            {
                csvProcess.process();
                return capture.rows;
            }
            finally
            {
                try { wrapper.Dispose(); } catch (Exception) { }
                try { csvProcess.Dispose(); } catch (Exception) { }
            }
        }
        
    }
}