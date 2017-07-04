using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Web;

namespace Edubase.Common
{
    public class StringUtil
    {
        private const string SPACE = " ";

        public static string ConcatNonEmpties(string separator, params string[] items) => 
            string.Join(separator, items.Where(x => x.Clean() != null)).Clean();

        public enum SentenceifyOptions
        {
            AND,
            OR
        }

        /// <summary>
        /// Takes a string array and converts it into a sentence
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string Sentenceify(string[] items, SentenceifyOptions option = SentenceifyOptions.AND)
        {
            items = items.Select(x => x.Clean()).Where(x => x != null).ToArray();
            if (items.Length == 0) return null;
            else if (items.Length == 1) return string.Concat("&lsquo;<span class=\"bold-small\">", items[0], "</span>&rsquo;");
            else return string.Concat("&lsquo;<span class=\"bold-small\">", 
                string.Join("</span>&rsquo;, &lsquo;<span class=\"bold-small\">",
                items.Take(items.Length - 1)),"</span>&rsquo;", SPACE,  option.ToString().ToLower(), SPACE, 
                "&lsquo;<span class=\"bold-small\">", items.Last(),"</span>&rsquo;");
        }


        public static string SentencifyNoFormating(string[] items, SentenceifyOptions option = SentenceifyOptions.AND)
        {
            items = items.Select(x => x.Clean()).Where(x => x != null).ToArray();
            if (items.Length == 0) return null;
            else if (items.Length == 1) return items[0];
            else if (items.Length == 2) return string.Join(string.Concat(SPACE, option.ToString().ToLower(), SPACE), items);
            else return string.Concat(string.Join(", ", items.Take(items.Length - 1)), SPACE, option.ToString().ToLower(), SPACE, items.Last());
        }

        public static bool Boolify(string data, bool defaultValue = false)
        {
            var retVal = false;
            if (bool.TryParse(data, out retVal)) return retVal;
            else return defaultValue;
        }

        /// <summary>
        /// Preprocesses an establishment name removing special chars and converting "st" => "saint"
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DistillEstablishmentName(string text)
        {
            text = text?.Distill()?.ToLower().Clean();
            if (text != null)
            {
                if (text.StartsWith("st. ")) text = string.Concat("saint ", text.Remove(0, 4));
                if (text.Contains(" st. ")) text = text.Replace(" st. ", " saint ");

                if (text.StartsWith("st ")) text = string.Concat("saint ", text.Remove(0, 3));
                if (text.Contains(" st ")) text = text.Replace(" st ", " saint ");
            }
            return text;
        }

        public static TEnum? ToEnum<TEnum>(string text) where TEnum : struct
        {
            TEnum output;
            if (Enum.TryParse(text, out output)) return output;
            else return null;
        }

    }
}
