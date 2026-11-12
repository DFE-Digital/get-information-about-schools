using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace Edubase.Web.UI.Helpers.Tests
{
    public class HtmlHelperTests
    {
        [Fact]
        public void SplitNameAndCapitaliseFirstLetter_CamelCase_Timeout()
        {
            var longInput = new string('a', 10000) + "B" + new string('c', 10000);
            var error = new ModelError(longInput);

            Action act = () => HtmlHelperExtensions.SplitNameAndCapitaliseFirstLetter(error);

            var ex = Record.Exception(act);
            Assert.Null(ex);
        }

        [Fact]
        public void SplitNameAndCapitaliseFirstLetter_CamelCase()
        {
            var error = new ModelError("camelCaseString");
            var result = HtmlHelperExtensions.SplitNameAndCapitaliseFirstLetter(error);
            Assert.Equal("Camel case string", result);
        }

        [Fact]
        public void SplitNameAndCapitaliseFirstLetter_EmptyString()
        {
            var error = new ModelError(string.Empty);
            var result = HtmlHelperExtensions.SplitNameAndCapitaliseFirstLetter(error);
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("Line1\nLine2", "Line1<br/>Line2")]
        [InlineData("Line1\rLine2", "Line1<br/>Line2")]
        [InlineData("Line1\r\nLine2", "Line1<br/>Line2")]
        [InlineData("Line1\n\nLine2", "Line1<br/><br/>Line2")]
        [InlineData("Line1", "Line1")]
        public void HtmlNewLines_CorrectFormatting(string input, string expected)
        {
            var htmlHelper = new Mock<IHtmlHelper>();
            htmlHelper.Setup(x => x.Encode(It.IsAny<string>())).Returns<string>(s => s);

            var result = htmlHelper.Object.HtmlNewlines(input);

            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData("Line1\nLine2", "Line1<br/>Line2")]
        [InlineData("Line1\rLine2", "Line1<br/>Line2")]
        [InlineData("Line1\r\nLine2", "Line1<br/>Line2")]
        public void HtmlNewLines_HtmlString_CorrectFormatting(string input, string expected)
        {
            var htmlHelper = new Mock<IHtmlHelper>();
            htmlHelper.Setup(x => x.Encode(It.IsAny<object>())).Returns<object>(o => o?.ToString() ?? string.Empty);

            var result = htmlHelper.Object.HtmlNewlines(new HtmlString(input));

            Assert.Equal(expected, result.Value);
        }
    }
}
