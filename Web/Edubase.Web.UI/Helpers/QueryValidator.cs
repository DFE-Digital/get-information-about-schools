using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Helpers
{
    public static class QueryValidator
    {
        private static readonly char[] ALLOWED_SYMBOLS = new char[5] { '\'', '.', '-', '!', ',' };

        public static bool ValidatePlaceSuggestionQuery(string query)
        {
            return !string.IsNullOrWhiteSpace(query) && query.Length <= 50
                && query.All((char character) => char.IsLetterOrDigit(character) || char.IsWhiteSpace(character) || ALLOWED_SYMBOLS.Contains(character));
        }
    }
}
