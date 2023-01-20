using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Helpers
{
    public class StringUtils
    {
        public static string ElementIdFormat(string text)
        {
            text = text.Replace(".", string.Empty)
                       .Replace("'", string.Empty);

            var pattern = @"[^a-zA-Z0-9_]";
            var replacement = "-";
            var result = Regex.Replace(text, pattern, replacement);

            return result.ToLowerInvariant();
        }
    }
}
