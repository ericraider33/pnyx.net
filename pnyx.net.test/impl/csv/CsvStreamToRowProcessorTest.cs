using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.impl;
using pnyx.net.impl.csv;
using pnyx.net.processors.rows;
using pnyx.net.processors.sources;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.impl.csv;

public class CsvStreamToRowProcessorTest
{
    [Theory]
    [InlineData("", null, null)]
    [InlineData("a", new[] { "a" }, null)]
    [InlineData(" ", new[] { " " }, null)]
    [InlineData(" , ,,", new[] { " ", " ", "", "" }, null)]
    [InlineData("a,b", new[] { "a", "b" }, null)]
    [InlineData("a,b\n", new[] { "a", "b" }, null)]
    [InlineData("a,b\nc", new[] { "a", "b" }, new [] { "c" })]
    [InlineData("a,b\nc\n", new[] { "a", "b" }, new [] { "c" })]
    [InlineData("\"a\"", new[] { "a" }, null)]
    [InlineData("\"a\",\"b\"", new[] { "a", "b" }, null)]
    [InlineData("\"a\",\"b\"\n", new[] { "a", "b" }, null)]
    [InlineData("\"a\",\"b\"\n\"c\"", new[] { "a", "b" }, new [] { "c" })]
    [InlineData("\"a\",\"b\"\n\"c\"\n", new[] { "a", "b" }, new [] { "c" })]
    [InlineData("\n", new String[0], null)]
    [InlineData("\n\n", new String[0], new String[0])]        
    public async Task line(String source, String[]? rowA, String[]? rowB)
    {
        await verifyRows(source, rowA, rowB);
    }

    [Theory]
    [InlineData("a\"b", "Line 1 contains a quote that isn't wrapped with quotes")]
    [InlineData("\"a\"b", "Line 1 contains an unexpected character b after quote")]
    [InlineData("\"ab", "File ends with open quotes")]
    public async Task exceptions(String source, String expectedMessage)
    {
        IllegalStateException err = await Assert.ThrowsAsync<IllegalStateException>(() => parseRows(source));
        Assert.Equal(expectedMessage, err.Message);
    }
        
    [Theory]
    [InlineData("a\"b", new [] { "a\"b" })]
    [InlineData("\"a\"b", new [] { "ab" })]
    [InlineData("\"ab", new [] { "ab" })]
    public async Task settings(String source, String[] rowA)
    {
        await verifyRows(source, rowA, null, x => { x.settings.setDefaults(strict: false); });
    }
        
    [Theory]
    [InlineData(TrimStyleEnum.None,"""a, b ,,d """, new [] { "a", " b ", "", "d " })]
    [InlineData(TrimStyleEnum.Trim,"""a, b ,,d """, new [] { "a", "b", "", "d" })]
    [InlineData(TrimStyleEnum.TrimToNull,"""a, b ,,d """, new [] { "a", "b", null, "d" })]
    [InlineData(TrimStyleEnum.None,"""a," b ",,d """, new [] { "a", " b ", "", "d " })]
    [InlineData(TrimStyleEnum.Trim,"""a," b ",,d """, new [] { "a", "b", "", "d" })]
    [InlineData(TrimStyleEnum.TrimToNull,"""a," b ",,d """, new [] { "a", "b", null, "d" })]
    public async Task trim(TrimStyleEnum style, String source, String?[] rowA)
    {
        await verifyRows(source, rowA, null, x => { x.settings.setDefaults(trimStyle: style); });
    }
        
    private async Task verifyRows(String source, String?[]? rowA, String?[]? rowB, Action<CsvStreamToRowProcessor>? callback = null)
    {
        List<String?>? actualA = null, actualB = null;
            
        List<List<String?>> rows = await parseRows(source, callback);
        if (rows.Count > 0) actualA = rows[0];
        if (rows.Count > 1) actualB = rows[1];
            
        Assert.True(RowUtil.isEqual(actualA, rowA.asRow()));
        Assert.True(RowUtil.isEqual(actualB, rowB.asRow()));
    }

    private async Task<List<List<String?>>> parseRows(String source, Action<CsvStreamToRowProcessor>? callback = null)
    {
        StreamInformation si = new StreamInformation(new Settings());
        CsvStreamToRowProcessor csvProcess = new CsvStreamToRowProcessor();

        StringStreamFactory wrapper = new StringStreamFactory(source);
        csvProcess.setSource(si, wrapper);
            
        CaptureRowProcessor capture = new CaptureRowProcessor();
        csvProcess.setNextRowProcessor(capture);
            
        if (callback != null)
            callback(csvProcess);
            
        try
        {
            await csvProcess.process();
            return capture.rows;
        }
        finally
        {
            try
            {
                await wrapper.DisposeAsync();
            } 
            catch (Exception) 
            {
                // ignored
            }

            try
            {
                await csvProcess.DisposeAsync();
            } 
            catch (Exception) 
            {
                // ignored
            }
        }
    }
        
}