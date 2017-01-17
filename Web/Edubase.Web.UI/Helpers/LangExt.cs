using Autofac;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Helpers
{
    public static class LangExt
    {
        public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<LookupDto> items, int? currentId) 
            => items.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = currentId.HasValue && currentId.Value == x.Id });

        /// <summary>
        /// Adds an item to the list if it's not already in there.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<int> AddUniqueMutable(this List<int> list, int item)
        {
            if (!list.Contains(item)) list.Add(item);
            return list;
        }

        public static List<int> AddUnique(this List<int> list, int item)
        {
            var retVal = new List<int>(list);
            if (!retVal.Contains(item)) retVal.Add(item);
            return retVal;
        }

        public static void AddModelError<TModel, TProperty>(this TModel source,
                                                    Expression<Func<TModel, TProperty>> ex,
                                                    string message,
                                                    ModelStateDictionary modelState)
        {
            var key = ExpressionHelper.GetExpressionText(ex);
            modelState.AddModelError(key, message);
        }


    }
}