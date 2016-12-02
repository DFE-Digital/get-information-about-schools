using AutoMapper;
using Edubase.Common;
using Edubase.Data.Entity.Lookups;
using Edubase.Services.Domain;
using Microsoft.SqlServer.Types;
using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Expressions;

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


        public static DataRowChainableContext<TSource> Set<TSource, TField>(this TSource model, DataRow row, Expression<Func<TSource, TField>> field)
        {
            var data = GetNameAndValue(model, field);
            row[data.Item1] = data.Item2.SQLify();
            return new DataRowChainableContext<TSource>(model, row);
        }

        public static DataRowChainableContext<TSource> Set<TSource, TField>(this DataRowChainableContext<TSource> model, Expression<Func<TSource, TField>> field)
        {
            var data = GetNameAndValue(model.Model, field);
            model.Row[data.Item1] = data.Item2.SQLify();
            return model;
        }

        private static Tuple<string, TField> GetNameAndValue<TSource, TField>(TSource model, Expression<Func<TSource, TField>> field)
        {
            var propName = (field.Body as MemberExpression ?? ((UnaryExpression)field.Body).Operand as MemberExpression).Member.Name;
            var value = field.Compile()(model);
            return new Tuple<string, TField>(propName, value);
        }
        
        public static int? Id<T>(this IEnumerable<T> items, string code)
            where T : LookupDto => items.FirstOrDefault(x => x.Code == Program.ProcessCode(code))?.Id;

        public static int? IdFromName<T>(this IEnumerable<T> items, string name)
            where T : LookupDto => items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) )?.Id;
        
        /// <summary>
        /// Ensures the email address is valid and if not, returns null.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToCleanEmail(this string text) => text.Clean().IsValidEmail() ? text.Clean() : null;

        public static object GetPropertyValue(this object item, string propertyName)
        {
            var type =  (item is Type) ? (Type) item : item?.GetType();
            var propInfo = type.GetProperty(propertyName);
            return propInfo?.GetValue(item, null);
        }

        public static string AsEnumName(this string text)
        {
            var retVal = text.CleanOfNonChars(true).ToTitleCase().Replace(" ", "");
            if (retVal.IsInteger()) retVal = "v_" + retVal;
            if (char.IsNumber(retVal[0])) retVal = "_" + retVal;
            return retVal;
        }

    }

    public class DataRowChainableContext<T>
    {
        public T Model { get; set; }
        public DataRow Row { get; set; }
        
        public DataRowChainableContext(T model, DataRow row)
        {
            Model = model;
            Row = row;
        }
    }
}
