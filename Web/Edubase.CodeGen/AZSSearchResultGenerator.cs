using Edubase.Data;
using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringExtensions;
using Edubase.Common;
namespace Edubase.CodeGen
{
    public class AZSSearchResultGenerator
    {
        private const string S1 = "[subtemplate:prop]";
        private const string S2 = "[/subtemplate:prop]";
        
        public string Generate(DataTable table, string namespaceName, string className)
        {
            var template = File.ReadAllText("SearchResultDocument.txt");
            var sb = new StringBuilder(template);

            sb.Replace("[NAMESPACE]", namespaceName);
            sb.Replace("[CLASSNAME]", className);

            var propPart = template.Between(S1, S2, StringComparison.OrdinalIgnoreCase);

            var fields = new List<string>();
            foreach (var col in table.Columns.Cast<DataColumn>())
            {
                fields.Add(propPart.Replace("[FIELDNAME]", col.ColumnName).Replace("[TYPE]", Util.Aliases.Get(col.DataType) ?? col.DataType.Name));
            }

            var fieldList = string.Join("\r\n", fields);
            sb.Replace(S1 + propPart + S2, fieldList);

            return sb.ToString();

        }

    }
}
