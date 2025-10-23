using System.Text.RegularExpressions;

namespace Edubase.Web.UI.Helpers
{
    public class StringUtils
    {
        public static string ElementIdFormat(string text)
        {
            text = text.Replace(".", string.Empty)
                       .Replace("'", string.Empty)
                       .Replace(",", string.Empty)
                       .Replace("(", string.Empty)
                       .Replace(")", string.Empty);

            var pattern = @"[^a-zA-Z0-9_]";
            var replacement = "-";
            var result = Regex.Replace(text, pattern, replacement);

            return result.ToLowerInvariant();
        }
    }
}
