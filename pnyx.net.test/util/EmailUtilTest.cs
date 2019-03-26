using System;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util
{
    public class EmailUtilTest
    {
        [Fact]
        public void testRepair()
        {
            Assert.Equal("MARSHA.DAUGHERTY@YAHOO.com", EmailUtil.validateAndRepair("MARSHA.DAUGHERTY@YAHOO"));        
            Assert.Equal("johnathan.j.shinnie@gmail.com", EmailUtil.validateAndRepair("johnathan.j.shinnie@gmail"));        
            Assert.Equal("dl.harper@hotmail.com", EmailUtil.validateAndRepair("dl.harper@hotmail"));        
            
            Assert.Equal("abc@mysite.com", EmailUtil.validateAndRepair("abc@mysite.com"));
            Assert.Equal("abc@mysite.com", EmailUtil.validateAndRepair("abc@mysite,com"));
            Assert.Equal("abc@mysite.com", EmailUtil.validateAndRepair("abc@mysitecom"));
            Assert.Equal("abc@mysite.net", EmailUtil.validateAndRepair("abc@mysitenet"));
            Assert.Equal("abc@mysite.CoM", EmailUtil.validateAndRepair("abc@mysiteCoM"));
            Assert.Equal("abc@gmail.com", EmailUtil.validateAndRepair("abc@gmail"));
            Assert.Equal("abc@yahoo.com", EmailUtil.validateAndRepair("abc@yahoo"));
            Assert.Equal("abc@me.com", EmailUtil.validateAndRepair("abc@me"));
            Assert.Equal(null, EmailUtil.validateAndRepair("abc@mysitecomdomtom"));        // no TLD
            Assert.Equal(null, EmailUtil.validateAndRepair("abc@mysitegmailhail"));        // no TLD

            Assert.Null(EmailUtil.validateAndRepair("abcmysitecom"));
        }

        [Theory]
        [InlineData("x@x.c0m", "x@x.com")]
        [InlineData("x@x.con", "x@x.com")]
        public void testRepairEndings(String source, String expected)
        {
            Assert.Equal(expected, EmailUtil.validateAndRepair(source));
        }

        [Fact]
        public void testNoEmail()
        {           
            Assert.Null(EmailUtil.validateAndRepair("NO@EMAIL.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NO_eMAIL@YAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NO_EMAIL@YAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("nobody@yahoo.com"));
            Assert.Null(EmailUtil.validateAndRepair("NOEAMIL@YAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAI@YAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@email.com"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@EMAIL.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@hotmail.com"));
            Assert.Null(EmailUtil.validateAndRepair("noemail@noemail.com"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@YAHO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("noemail@yahoo.com"));
            Assert.Null(EmailUtil.validateAndRepair("noemail@YAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@yahoo.com"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@yAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@YAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@YAOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMAIL@YHAOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("noemial@noemail.com"));
            Assert.Null(EmailUtil.validateAndRepair("NOEMIAL@YAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("NOMAIL@YAHOO.COM"));
            Assert.Null(EmailUtil.validateAndRepair("nomemail@yahoo.com"));
        }
        
    }
}