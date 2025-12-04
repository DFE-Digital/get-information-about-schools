using System;
using Xunit;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        //[Fact]
        //public void SplitNameAndCapitaliseFirstLetter_CamelCase()
        //{
        //    var exception = new Exception("camelCaseString");
        //    var error = new ModelError(exception);
        //    var fieldInfo = typeof(ModelError).GetProperty("ErrorMessage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        //    if (fieldInfo != null)
        //    {
        //        fieldInfo.SetValue(error, "camelCaseString");
        //    }

        //    var result = HtmlHelperExtensions.SplitNameAndCapitaliseFirstLetter(error);

        //    Assert.Equal("Camel case string", result);
        //}

        //[Fact]
        //public void SplitNameAndCapitaliseFirstLetter_EmptyString()
        //{
        //    var exception = new Exception("");
        //    var error = new ModelError(exception);
        //    var fieldInfo = typeof(ModelError).GetProperty("ErrorMessage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        //    if (fieldInfo != null)
        //    {
        //        fieldInfo.SetValue(error, "");
        //    }

        //    var result = HtmlHelperExtensions.SplitNameAndCapitaliseFirstLetter(error);

        //    Assert.Equal("", result);
        //}
    }
}
