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
    public class AZSIndexConfigGenerator
    {
        private const string S1 = "[subtemplate:addfield]";
        private const string S2 = "[/subtemplate:addfield]";
        
        public string Generate(DataTable table, string namespaceName, string className, string indexName, string suggesterName)
        {
            var template = File.ReadAllText("SearchIndexConfig.txt");
            var sb = new StringBuilder(template);

            sb.Replace("[NAMESPACE]", namespaceName);
            sb.Replace("[CLASSNAME]", className);
            sb.Replace("[INDEXNAME]", indexName);
            sb.Replace("[SUGGESTERNAME]", suggesterName);

            var addFieldPart = template.Between(S1, S2, StringComparison.OrdinalIgnoreCase);

            var fields = new List<string>();
            foreach (var col in table.Columns.Cast<DataColumn>())
            {
                fields.Add(addFieldPart.Replace("[FIELDNAME]", col.ColumnName).Replace("[TYPE]", Util.Aliases.Get(col.DataType) ?? col.DataType.Name));
            }

            var fieldList = string.Join("\r\n", fields);
            sb.Replace(S1 + addFieldPart + S2, fieldList);

            return sb.ToString();

        }

    }
}
