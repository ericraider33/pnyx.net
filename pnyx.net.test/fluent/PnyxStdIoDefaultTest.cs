using System;
using pnyx.net.errors;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.fluent
{
    public class PnyxStdIoDefaultTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void grep(bool stdIoDefault)
        {
            verifyStdIo(stdIoDefault, p => p.grep("x"), FluentState.Line);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void parseCsv(bool stdIoDefault)
        {
            verifyStdIo(stdIoDefault, p => p.parseCsv(), FluentState.Row);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void stdOutGrep(bool stdIoDefault)
        {
            verifyStdIo(stdIoDefault, p => p.grep("x").compile(), FluentState.Compiled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void stdOutParseCsv(bool stdIoDefault)
        {
            verifyStdIo(stdIoDefault, p => p.parseCsv().compile(), FluentState.Compiled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void stdInOut(bool stdIoDefault)
        {
            verifyStdIo(stdIoDefault, p => p.compile(), FluentState.Compiled);
        }
        
        private Pnyx verifyStdIo(bool stdIoDefault, Action<Pnyx> toTest, FluentState? expected = null)
        {
            Pnyx p = new Pnyx();
            p.setSettings(stdIoDefault: stdIoDefault);

            if (stdIoDefault)
            {
                toTest(p);
                Assert.Equal(expected, p.state);
            }
            else
            {                
                Assert.Throws<IllegalStateException>(() => toTest(p));
            }

            return p;
        }
    }
}