using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using MoreLinq;
using System.Text;

namespace Edubase.Common
{
    public static class LanguageExtensions
    {
        public static bool OneOfThese(this int? id, params Enum[] items) => items.Cast<int>().Any(x => x == id);
        public static bool OneOfThese(this Enum flag, params Enum[] items) => items.Cast<int>().Any(x => x == Convert.ToInt32(flag));

        public static bool IsValidEmail(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            return Regex.IsMatch(text, @"\A\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b\Z", RegexOptions.IgnoreCase);
        }

        public static string Remove(this string data, params string[] stringsToRemove)
        {
            if (data == null || string.IsNullOrWhiteSpace(data)) return null;
            foreach (var item in stringsToRemove) data = data.Replace(item, string.Empty);
            return data;
        }

        public static string RemoveSubstring(this string data, string stringToRemove)
        {
            if (data == null || string.IsNullOrWhiteSpace(data)) return null;
            data = data.Replace(stringToRemove, string.Empty);
            return data;
        }

        public static string RemoveSubstrings(this string data, params string[] stringsToRemove)
        {
            if (data == null || string.IsNullOrWhiteSpace(data)) return null;
            stringsToRemove.ForEach(x => data = data.Replace(x, string.Empty));
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
            if (key != null && data.ContainsKey(key)) return data[key];
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
            if (key != null && data.ContainsKey(key)) return data[key];
            return default(T2);
        }

        /// <summary>
        /// Gets data from a dictionary or sets it using the supplied factory; either way, returns the value.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static T2 Get<T1, T2>(this IDictionary<T1, T2> data, T1 key, Func<T2> factory)
        {
            if (data.ContainsKey(key)) return data[key];
            else return data.Set(key, factory());
        }

        /// <summary>
        /// Sets a value to the dictionary and returns it
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T2 Set<T1, T2>(this IDictionary<T1, T2> data, T1 key, T2 val)
        {
            return data[key] = val;
        }

        public static T2 Get<T1, T2>(this IDictionary<T1, T2> data, T1 key, T2 defaultValue)
        {
            if (data.ContainsKey(key)) return data[key];
            return defaultValue;
        }

        /// <summary>
        /// Alias of Add, but returns the value added.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TValue Append<TKey, TValue>(this IDictionary<TKey, TValue> data, TKey key, TValue value)
        {
            data.Add(key, value);
            return value;
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

        public static bool IsNullable(this Type type)
        {
            if (type == null) return true; // obvious
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static Type GetUnderlyingType(this Type type) => (type.IsNullable()) ? (Nullable.GetUnderlyingType(type) ?? type) : type;


        /// <summary>
        /// Returns a string safe to use as a filename
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CleanOfNonChars(this string text, bool allowSpaces = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var sb = new StringBuilder();
            foreach (var c in text)
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || (allowSpaces && c == ' ')) sb.Append(c);
            return sb.ToString();
        }


        public static TSource FirstOrThrow<TSource>(this IEnumerable<TSource> source, Func<Exception> exceptionFactory)
        {
            var item = source.FirstOrDefault();
            if (item == null) throw exceptionFactory();
            return item;
        }

        public static byte[] Compress(this byte[] buffer) => new IO.Compression().Compress(buffer);

        public static byte[] Decompress(this byte[] buffer) => new IO.Compression().Decompress(buffer);

        /// <summary>
        /// Distills a piece of text by removing special chars and optionally replacing international characters with ASCII counterparts.
        /// Also makes lower case.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string Distill(this string text, char replacement = ' ', bool remapInternationalCharToAscii = false)
        {
            text = text.Clean();

            if (text == null) return null;
            
            int len = text.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = text[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append(replacement);
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(remapInternationalCharToAscii ? RemapInternationalCharToAscii(c) : c.ToString());
                    if (prevlen != sb.Length) prevdash = false;
                }
            }

            var retVal = string.Empty;
            if (prevdash) retVal = sb.ToString().Substring(0, sb.Length - 1);
            else retVal = sb.ToString();

            return retVal;
        }

        private static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
    }
}
