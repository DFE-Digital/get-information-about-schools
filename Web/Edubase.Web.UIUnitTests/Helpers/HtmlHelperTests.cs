using System;
using System.IO;
using System.Web.Mvc;
using Xunit;

namespace Edubase.Web.UI.Helpers.Tests
{
    public class HtmlHelperTests
    {

        [Fact]
        public void SplitNameAndCapitaliseFirstLetter_CamelCase_Timeout()
        {
            var longInput = new string('a', 10000) + "B" + new string('c', 10000);
            var exception = new Exception(longInput);
            var error = new ModelError(exception);

            Action act = () => HtmlHelperExtensions.SplitNameAndCapitaliseFirstLetter((ModelError) error);

            var ex = Record.Exception(act);
            Assert.Null(ex);
        }

        [Fact]
        public void SplitNameAndCapitaliseFirstLetter_CamelCase()
        {
            var exception = new Exception("camelCaseString");
            var error = new ModelError(exception);
            var fieldInfo = typeof(ModelError).GetProperty("ErrorMessage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(error, "camelCaseString");
            }

            var result = HtmlHelperExtensions.SplitNameAndCapitaliseFirstLetter(error);

            Assert.Equal("Camel case string", result);
        }

        [Fact]
        public void SplitNameAndCapitaliseFirstLetter_EmptyString()
        {
            var exception = new Exception("");
            var error = new ModelError(exception);
            var fieldInfo = typeof(ModelError).GetProperty("ErrorMessage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(error, "");
            }

            var result = HtmlHelperExtensions.SplitNameAndCapitaliseFirstLetter(error);

            Assert.Equal("", result);
        }

        [Theory]
        [InlineData("Line1\nLine2", "Line1<br/>Line2")]
        [InlineData("Line1\rLine2", "Line1<br/>Line2")]
        [InlineData("Line1\r\nLine2", "Line1<br/>Line2")]
        [InlineData("Line1\n\nLine2", "Line1<br/><br/>Line2")]
        [InlineData("Line1", "Line1")]
        public void HtmlNewLines_CorrectFormatting(string input, string expected)
        {
            // Arrange
            var htmlHelper = CreateHtmlHelper();
           
            // Act
            var result = htmlHelper.HtmlNewlines(input);

            // Assert
            Assert.Equal(expected, result.ToHtmlString());
        }

        private static HtmlHelper CreateHtmlHelper()
        {
            var viewData = new ViewDataDictionary();
            return new HtmlHelper(new ViewContext
                {
                    Writer = new StringWriter(),
                    ViewData = viewData
                },
                new TestViewDataContainer(viewData)
            );
        }

        private sealed class TestViewDataContainer : IViewDataContainer
        {
            public TestViewDataContainer(ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }

            public ViewDataDictionary ViewData { get; set; }
        }
    }
}
