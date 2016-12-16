using Edubase.Data;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Edubase.Import.Helpers
{
    public static class DatabaseHelperExtensions
    {
        /// <summary>
        /// Generates data tables from Entity Framework entities
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Dictionary<Type, DataTable> GenerateDataTables(this ApplicationDbContext context)
        {
            return DbUtil.GenerateDataTables(context, x =>
            {
                var geoColumn = x.Cast<DataColumn>().Where(c => c.DataType.Name.Contains("SqlGeography")).FirstOrDefault();
                if (geoColumn != null) geoColumn.DataType = typeof(SqlGeography);
            });
        }
    }
    
}
