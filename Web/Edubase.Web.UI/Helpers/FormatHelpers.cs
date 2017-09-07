using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Helpers
{
    public static class FormatHelpers
    {
        public static IHtmlString HtmlizeNewLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            else return new MvcHtmlString(HttpUtility.HtmlEncode(text).Replace("\n", "<br/>"));
        }

        public static string FirstCharToUpper(string input) => input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
    }
}