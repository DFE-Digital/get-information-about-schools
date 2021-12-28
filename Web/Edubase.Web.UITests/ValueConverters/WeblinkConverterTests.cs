using Xunit;
using Edubase.Web.UI.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Web.UI.ValueConverters.Tests
{
    public class WeblinkConverterTests
    {
        [Theory()]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("    ", null)]
        [InlineData("w3c.org", "http://w3c.org")]
        [InlineData("  w3c.org  ", "http://w3c.org")]
        [InlineData("http://w3c.org", "http://w3c.org")]
        [InlineData("  http://w3c.org  ", "http://w3c.org")]
        public void ConvertTest(string intialText, string expected)
        {
            var result = WeblinkConverter.Convert(intialText);

            Assert.Equal(expected, result);
        }
    }
}
