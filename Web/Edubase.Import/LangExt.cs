using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common;
using Microsoft.SqlServer.Types;
using System.Data.Entity.Spatial;

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
            if (input!= null && input.GetType() == typeof(string))
                input = (object)(input as string).Clean() ?? DBNull.Value;

            return input ?? DBNull.Value;
        }

        public static SqlGeography ToSqlGeography(this DbGeography coord) => coord != null ? SqlGeography.Parse(coord.AsText()).MakeValid() : null;
    }
}
