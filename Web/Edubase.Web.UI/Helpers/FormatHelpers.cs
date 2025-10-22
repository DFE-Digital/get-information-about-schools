using System.Net;
using Microsoft.AspNetCore.Html;

namespace Edubase.Web.UI.Helpers
{
    public static class FormatHelpers
    {
        public static HtmlString HtmlizeNewLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var encoded = WebUtility.HtmlEncode(text).Replace("\n", "<br/>");
            return new HtmlString(encoded);
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
