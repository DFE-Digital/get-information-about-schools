using Edubase.Common;
using Edubase.Services.Domain;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Edubase.Web.UI
{
    using Edubase.Web.Resources;
    using Services.Texuna.ChangeHistory.Models;

    public static class LangExt
    {
        public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<LookupDto> items, int? currentId = null) 
            => items.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = currentId.HasValue && currentId.Value == x.Id });

        public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<UserGroupModel> items, string currentId)
            => items.Select(x => new SelectListItem { Text = x.Name, Value = x.Code, Selected = currentId == x.Code });

        public static List<int> AddUnique(this List<int> list, int item)
        {
            var retVal = new List<int>(list);
            if (!retVal.Contains(item))
            {
                retVal.Add(item);
            }

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

        /// <summary>
        /// Adds one or more items under the same key and returns the new collection (incoming ref is treated as immutable).
        /// Only distinct values will be added. If the key/value combination exists, it's ignored.
        /// </summary>
        /// <param name="nvc"></param>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static NameValueCollection AddIfNonExistent(this NameValueCollection nvc, string key, params object[] values)
        {
            nvc = HttpUtility.ParseQueryString(nvc.ToString());
            Guard.IsNotNull(key.Clean(), () => new ArgumentNullException(nameof(key)));
            Guard.IsNotNull(values, () => new ArgumentNullException(nameof(values)));
            var items = values.Select(x => x?.ToString().Clean()).Distinct();
            foreach (var value in items)
            {
                var data = nvc.GetValues(key);
                if (data == null || (data != null && !data.Contains(value))) nvc.Add(key, value);
            }
            return nvc;
        }

        public static NameValueCollection RemoveKey(this NameValueCollection nvc, string key)
        {
            nvc = HttpUtility.ParseQueryString(nvc.ToString());
            Guard.IsNotNull(key.Clean(), () => new ArgumentNullException(nameof(key)));
            nvc.Remove(key);
            return nvc;
        }

        public static NameValueCollection RemoveKeys(this NameValueCollection nvc, params string[] keys)
        {
            foreach (var item in keys)
            {
                nvc = nvc.RemoveKey(item);
            }
            return nvc;
        }

        /// <summary>
        /// Adds a message that will appear in the validation summary, in addition to the field-level message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="rule"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> WithSummaryMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string message) => rule.WithState(x => message);

        public static IRuleBuilderOptions<T, TProperty> WithSummaryMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, object> messageProvider) => rule.WithState(messageProvider);

        public static List<T> Append<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        public static string GetMessage(this ApiError error)
        {
            return ApiMessagesHelper.Get(error.Code).Clean() ?? ApiMessagesHelper.Get(string.Concat(error.Code, "_", error.Fields), error.Message);
        }

        public static string GetMessage(this ApiWarning warning)
        {
            return ApiMessagesHelper.Get(warning.Code, warning.Message);
        }

    }
}
