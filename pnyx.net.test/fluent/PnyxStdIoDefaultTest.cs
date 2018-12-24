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
            verifyStdin(stdIoDefault, p => p.grep("x"), FluentState.Line);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void parseCsv(bool stdIoDefault)
        {
            verifyStdin(stdIoDefault, p => p.parseCsv(), FluentState.Row);
        }

        private void verifyStdin(bool stdIoDefault, Action<Pnyx> toTest, FluentState? expected = null)
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
        }
    }
}