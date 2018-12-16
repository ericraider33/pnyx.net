using System;
using System.IO;
using pnyx.cmd;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.cmd
{
    public class SettingYamlTest
    {
        [Fact]
        public void sample()
        {
            const String source = @"
# Settings for for pnyx cmd
tempDirectory: .
bufferLines: 100
# defaultEncoding: UTF-8
defaultNewline: ""\r\n""        
";
            
            SettingsYaml parser = new SettingsYaml();
            Settings settings = parser.parseReader(new StringReader(source));
            
            Assert.Equal(".", settings.tempDirectory);
            Assert.Equal(100, settings.bufferLines);
            Assert.Equal("\r\n", settings.defaultNewline);
            Assert.Equal("US-ASCII", settings.defaultEncoding.EncodingName);
        }
        
    }
}