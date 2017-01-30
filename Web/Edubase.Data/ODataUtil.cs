using System.Collections.Generic;
using System.Linq;

namespace Edubase.Data
{
    public class ODataUtil
    {
        private const string OR = " or ";
        private const string AND = " and ";
        private const string EQUALS = " eq ";

        public static string Or(string fieldName, int[] ids) => string.Concat("(", string.Join(OR, ids.Select(x => string.Concat(fieldName, EQUALS, x))), ")");

        public static string And(string fieldName, int[] ids) => string.Concat("(", string.Join(AND, ids.Select(x => string.Concat(fieldName, EQUALS, x))), ")");

        public static string Or(string fieldName, int?[] ids) => string.Concat("(", string.Join(OR, ids.Where(x => x.HasValue).Select(x => string.Concat(fieldName, EQUALS, x.Value))), ")");

        public static string And(string fieldName, int?[] ids) => string.Concat("(", string.Join(AND, ids.Where(x => x.HasValue).Select(x => string.Concat(fieldName, EQUALS, x.Value))), ")");

        public static List<string> OrderBy(string fieldName, bool ascending) => new List<string> { string.Concat(fieldName, " ", (ascending ? "asc" : "desc")) };
    }
}
