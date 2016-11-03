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

        public static string RemoveSubstring(this string data, string stringToRemove)
        {
            if (data == null || string.IsNullOrWhiteSpace(data)) return null;
            data = data.Replace(stringToRemove, string.Empty);
            return data;
        }

        public static string ToTitleCase(this string text)
        {
            if (text.Clean() == null) return null;
            return CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(text);
        }

        public static int? ToInteger(this string text)
        {
            int retVal;
            text = text.Clean();
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

        public static decimal? ToDecimal(this string text)
        {
            decimal temp;
            if (!text.IsNullOrEmpty() && decimal.TryParse(text, out temp)) return temp;
            else return null;
        }

        public static double? ToDouble(this string text)
        {
            double temp;
            if (!text.IsNullOrEmpty() && double.TryParse(text, out temp)) return temp;
            else return null;
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

        public static DateTime? ToDateTime(this string data)
        {
            if (data == null) return null;
            DateTime temp;
            if (DateTime.TryParse(data, out temp)) return temp;
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
        /// Returns the value from a dictionary or NULL (as opposed to throwing KeyNotFound exception)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T2 Get<T1,T2>(this IDictionary<T1, T2> data, T1 key)
        {
            if (data.ContainsKey(key)) return data[key];
            return default(T2);
        }

        public static T2 Get<T1, T2>(this IDictionary<T1, T2> data, T1 key, T2 defaultValue)
        {
            if (data.ContainsKey(key)) return data[key];
            return defaultValue;
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

        public static string Sentenceify(this string[] data) => StringUtil.Sentenceify(data);


        public static bool IsInFuture(this DateTime? dt) => dt.HasValue ? dt.Value.Date > DateTime.UtcNow.Date : false;
        

        public static string GetPart(this string data, string separator, int index = 0)
        {
            data = data.Clean();
            if (data != null)
            {
                var bits = data.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                if(index <= bits.GetUpperBound(0))
                {
                    return bits[index];
                }
            }

            return null;
        }

    }
}
