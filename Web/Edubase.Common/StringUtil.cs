using System.Linq;

namespace Edubase.Common
{
    public class StringUtil
    {
        public static string ConcatNonEmpties(string separator, params string[] items)
        {
            return string.Join(separator, items.Where(x => x != null && !string.IsNullOrWhiteSpace(x)));
        }

        /// <summary>
        /// Takes a string array and converts it into a sentence
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string Sentenceify(string[] items)
        {
            items = items.Select(x => x.Clean()).Where(x => x != null).ToArray();
            if (items.Length == 0) return null;
            else if (items.Length == 1) return items[0];
            else if (items.Length == 2) return string.Join(" and ", items);
            else return string.Concat(string.Join(", ", items.Take(items.Length - 1)), " and ", items.Last());
        }
    }
}
