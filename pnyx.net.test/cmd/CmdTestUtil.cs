using System;
using System.Collections.Generic;
using System.IO;
using pnyx.cmd;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.cmd
{
    public static class CmdTestUtil
    {
        public static void verifyYaml(String sourceYaml, String expectedStdout = null)
        {
            StringReader test = new StringReader(sourceYaml);
            
            YamlParser parser = new YamlParser();
            List<Pnyx> toExecute = parser.parseYaml(test);
            
            Assert.Single(toExecute);
            Pnyx p = toExecute[0];
            
            String actual;
            using (p)
                actual = p.processToString();
            
            Assert.Equal(expectedStdout, actual);
        }

    }
}