using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace Edubase.Common
{
    public static class LanguageExtensions
    {
        public static bool IsValidEmail(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            return Regex.IsMatch(text, @"\A\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b\Z", RegexOptions.IgnoreCase);
        }

        public static string ToTitleCase(this string text)
        {
            if (text.Clean() == null) return null;
            return CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(text);
        }

        public static int? ToInteger(this string text)
        {
            int retVal;
            if (!text.IsNullOrEmpty() && int.TryParse(text, out retVal)) return retVal;
            else return null;
        }

        public static bool IsInteger(this string text) => text.ToInteger() != null;

        public static int ToInteger(this string text, int defaultValue)
        {
            int retVal;
            if (!text.IsNullOrEmpty() && int.TryParse(text, out retVal)) return retVal;
            else return defaultValue;
        }

        /// <summary>
        /// Coalesces to null when empty, whitespace or null; also trims the string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Clean(this string text)
        {
            text = text?.Trim();
            if (text.IsNullOrEmpty()) return null;
            else return text;
        }
        public static DateTime? ToDateTime(this string data, string format)
        {
            if (data == null) return null;
            DateTime temp;
            if (DateTime.TryParseExact(data, format, null, DateTimeStyles.None, out temp)) return temp;
            else return null;
        }

        public static DateTime? ToDateTime(this string data, string[] formats)
        {
            if (string.IsNullOrWhiteSpace(data)) return null;
            else return formats.Select(x => data.ToDateTime(x)).FirstOrDefault(x => x.HasValue);
        }

        public static bool IsNullOrEmpty(this string text) => string.IsNullOrWhiteSpace(text);

        public static string Get(this IDictionary<string, string> data, string key)
        {
            if (data.ContainsKey(key)) return data[key];
            return null;
        }

        /// <summary>
        /// Truncates a string if necessary and appends an ellipsis
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxChars"></param>
        /// <returns></returns>
        public static string Ellipsis(this string text, int maxChars)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Length > maxChars) return text.Substring(0, maxChars - 3) + "...";
            else return text;
        }

    }
}
