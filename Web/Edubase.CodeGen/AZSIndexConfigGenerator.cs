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
                var sb2 = new StringBuilder(addFieldPart.Replace("[FIELDNAME]", col.ColumnName).Replace("[TYPE]", Util.Aliases.Get(col.DataType) ?? col.DataType.Name));
                sb2.Replace("[ISSEARCHABLE]", col.ColumnName.Equals("Name").ToString().ToLower());
                sb2.Replace("[ISFILTERABLE]", col.ColumnName.EndsWith("Id", StringComparison.Ordinal).ToString().ToLower());
                sb2.Replace("[ISSORTABLE]", (col.ColumnName.Equals("Name") || col.ColumnName.Equals("Location")).ToString().ToLower());
                sb2.Replace("[ISKEY]", col.ColumnName.Equals("Urn").ToString().ToLower());
                sb2.Replace("[INCLUDEINSUGGESTER]", col.ColumnName.Equals("Name").ToString().ToLower());
                fields.Add(sb2.ToString());
            }

            var fieldList = string.Join("\r\n", fields);
            sb.Replace(S1 + addFieldPart + S2, fieldList);

            return sb.ToString();

        }

    }
}
