using System;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace Edubase.Web.UI.Helpers
{
    public static class FormatHelpers
    {
        public static IHtmlContent HtmlizeNewLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return HtmlString.Empty;
            return new HtmlString(HtmlEncoder.Default.Encode(text).Replace("\n", "<br/>", StringComparison.Ordinal));
        }

        public static string FirstCharToUpper(string input) => input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
    }
}
