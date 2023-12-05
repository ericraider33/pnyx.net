using System;
using System.IO;
using pnyx.cmd.shared;
using pnyx.net.errors;
using pnyx.net.fluent;
using Xunit;
using YamlDotNet.Core;

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
defaultEncoding: UTF-8
defaultNewline: ""\r\n""        
backupRewrite: false
";
            
            SettingsYaml parser = new SettingsYaml();
            Settings settings = parser.deserializeSettings(new StringReader(source));
            
            Assert.Equal(".", settings.tempDirectory);
            Assert.Equal(100, settings.bufferLines);
            Assert.Equal("\r\n", settings.defaultNewline);
            Assert.Equal("utf-8", settings.defaultEncoding.WebName);
            Assert.False(settings.backupRewrite);
        }

        [Theory]
        [InlineData(@"""\r\n""", "\r\n")]
        [InlineData(@"""\n""", "\n")]
        [InlineData("", null)]
        [InlineData("junk", null)]
        [InlineData("windows", "\r\n")]
        [InlineData("unix", "\n")]
        public void defaultNewline(String value, String expected)
        {
            String yamlText = String.Format("defaultNewline: {0}", value);
            StringReader reader = new StringReader(yamlText);
            
            SettingsYaml parser = new SettingsYaml();
            if (expected == null)
            {                
                Assert.Throws<InvalidArgumentException>(() => parser.deserializeSettings(reader));            
            }
            else
            {
                Settings settings = parser.deserializeSettings(reader);
                Assert.Equal(expected, settings.defaultNewline);
            }
        }

        [Theory]
        [InlineData("utf-7", false)]
        [InlineData("utf-8", true)]
        [InlineData("utf-32", true)]
        [InlineData("us-ascii", true)]
        [InlineData("junk", false)]
        public void defaultEncoding(String value, bool valid)
        {            
            String yamlText = String.Format("defaultEncoding: {0}", value);
            StringReader reader = new StringReader(yamlText);
            
            SettingsYaml parser = new SettingsYaml();
            if (!valid)
            {                
                YamlException error = Assert.Throws<YamlException>(() => parser.deserializeSettings(reader));
                Assert.IsType<InvalidArgumentException>(error.InnerException);
            }
            else
            {
                Settings settings = parser.deserializeSettings(reader);
                Assert.Equal(value, settings.defaultEncoding.WebName);
            }
        }
    }
}