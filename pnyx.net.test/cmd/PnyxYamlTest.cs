using System;
using System.Collections.Generic;
using System.IO;
using pnyx.cmd;
using pnyx.net.errors;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.cmd
{
    public class PnyxYamlTest
    {
        [Fact]
        public void helloWorld()
        {
            const String source = @"
# echo 'Hello world'
- readString: Hello World
";
            const String expected = "Hello World";
            verify(source, expected);
        }

        [Fact]
        public void unknownMethod()
        {
            const String source = @"- junk:";
            InvalidArgumentException error = Assert.Throws<InvalidArgumentException>(() => verify(source));
            Assert.Equal("Pnyx method can not be found: junk", error.Message);
        }

        [Fact]
        public void sequenceParameters()
        {
            String source = @"
- readString: 'Mx trx miscarrx.
  Shx flx.'
- sed:
  - x
  - y
  - ig  
";
            const String expected = "My try miscarry. Shy fly.";
            verify(source, expected);

            source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: ['x','y','ig']
";
            verify(source, expected);
        }

        [Fact]
        public void sequenceParametersDefaults()
        {
            const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: ['x','y']
";
            const String expected = "My trx miscarrx. Shx flx.";
            verify(source, expected);
        }        

        [Fact]
        public void sequenceParametersTooFew()
        {
            const String source = @"[{'readString': 'Mx trx miscarrx. Shx flx.'},{'sed': ['x'] }]";
            InvalidArgumentException error = Assert.Throws<InvalidArgumentException>(() => verify(source));
            Assert.Equal("Too few parameters 1 specified for Pnyx method 'sed', which only has 2 required parameters", error.Message);
        }        

        [Fact]
        public void sequenceParametersTooMany()
        {
            const String source = @"[{'readString': 'Mx trx miscarrx. Shx flx.'}, {'sed': ['w','x','y','z']}]";
            InvalidArgumentException error = Assert.Throws<InvalidArgumentException>(() => verify(source));
            Assert.Equal("Too many parameters 4 specified for Pnyx method 'sed', which only has 3 parameters", error.Message);
        }

        [Fact]
        public void dictionaryParameters()
        {
            const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: { 'flags': 'ig', 'pattern': 'x', 'replacement': 'y' }
";            
            const String expected = "My try miscarry. Shy fly.";
            verify(source, expected);
        }        

        [Fact]
        public void dictionaryParametersDefaults()
        {
            const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: { 'pattern': 'x', 'replacement': 'y' }
";            
            const String expected = "My trx miscarrx. Shx flx.";
            verify(source, expected);
        }        

        [Fact]
        public void dictionaryParametersMissingRequired()
        {
            const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: { 'pattern': 'x' }
";            
            InvalidArgumentException error = Assert.Throws<InvalidArgumentException>(() => verify(source));
            Assert.Equal("Pnyx method 'sed' is missing required parameter 'replacement'", error.Message);
        }        

        [Fact]
        public void dictionaryParametersTooMany()
        {
            const String source = @"
- readString: 'Mx trx miscarrx. Shx flx.'
- sed: { 'flags': 'ig', 'pattern': 'x', 'replacement': 'y', 'junk': 'xxx' }
";            
            InvalidArgumentException error = Assert.Throws<InvalidArgumentException>(() => verify(source));
            Assert.Equal("Unknown named parameters 'junk' for Pnyx method 'sed', which has parameters 'pattern,replacement,flags'", error.Message);
        }

        [Fact]
        public void blockSingle()
        {
            const String source = @"
- cat: 
    block:
      - readString: Line One
      - readString: Line Two
";
            const String expected = 
@"Line One
Line Two";
            verify(source, expected);
        }
        
        private void verify(String source, String expected = null)
        {
            StringReader test = new StringReader(source);
            
            PnyxYaml parser = new PnyxYaml();
            List<Pnyx> toExecute = parser.parseYaml(test);
            
            Assert.Single(toExecute);
            Pnyx p = toExecute[0];
            
            String actual;
            using (p)
                actual = p.processToString();
            
            Assert.Equal(expected, actual);
        }
    }
}