using Xunit;

namespace Edubase.Web.UI.ValueConverters.UnitTests
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
