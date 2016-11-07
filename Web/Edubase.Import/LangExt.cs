using Edubase.Common;
using Microsoft.SqlServer.Types;
using MoreLinq;
using System;
using System.Data;
using System.Data.Entity.Spatial;
using System.Linq;

namespace Edubase.Import
{
    public static class LangExt
    {
        /// <summary>
        /// Cleans a string, trims it.
        /// Any other object is converted to DBNull when null.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object SQLify(this object input)
        {
            if (input != null && input.GetType() == typeof(string))
                input = (input as string).Clean();
            return input ?? DBNull.Value;
        }
        
        public static SqlGeography ToSqlGeography(this DbGeography coord) => coord != null ? SqlGeography.Parse(coord.AsText()).MakeValid() : null;

        /// <summary>
        /// Creates a row, populates all columns with a default of DBNull, add the row and returns it.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="populate"></param>
        /// <returns></returns>
        public static DataRow CreateRow(this DataTable table, Action<DataRow> populate)
        {
            var row = table.NewRow();
            table.Columns.Cast<DataColumn>().ForEach(x => row[x] = DBNull.Value);
            populate(row);
            table.Rows.Add(row);
            return row;
        }
        


    }
}
