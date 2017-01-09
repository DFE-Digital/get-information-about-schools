using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using Edubase.Services;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Edubase.Web.UI.Helpers
{
    public static class HtmlHelperExtensions
    {

        public static MvcHtmlString ValidationCssClassFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            if (state == null) return MvcHtmlString.Empty;
            else if (state.Errors.Count == 0) return MvcHtmlString.Empty;
            else return new MvcHtmlString("error");
        }

        [Obsolete]
        private static Lazy<EstablishmentPermission[]> _permissions = new Lazy<EstablishmentPermission[]>(() =>
        {
            var permissions = MemoryCache.Default.Get("permissions") as EstablishmentPermission[];
            if (permissions == null)
            {
                using (var dc = new ApplicationDbContext()) permissions = dc.Permissions.ToArray();
                MemoryCache.Default.Set("permissions", permissions, DateTimeOffset.UtcNow.AddMinutes(10));
            }
            return permissions;
        });

        [Obsolete]
        public static bool CanUpdateFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var role = AuthHelper.GetRole();
            var permission = _permissions.Value.FirstOrDefault(x => x.PropertyName == name && x.RoleName == role);
            return permission?.AllowUpdate ?? true;
        }

        [Obsolete]
        public static bool CanUpdate<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            string name)
        {
            var role = AuthHelper.GetRole();
            var permission = _permissions.Value.FirstOrDefault(x => x.PropertyName == name && x.RoleName == role);
            return permission?.AllowUpdate ?? true;
        }

        [Obsolete]
        public static bool CanApproveFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var role = AuthHelper.GetRole();
            var permission = _permissions.Value.FirstOrDefault(x => x.PropertyName == name && x.RoleName == role);
            return permission?.AllowApproval ?? true;
        }

        [Obsolete]
        public static MvcHtmlString EduTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object attributes, string fieldName = null)
        {
            var canUpdate = fieldName == null ? CanUpdateFor(htmlHelper, expression) : CanUpdate(htmlHelper, fieldName);
            var result = htmlHelper.TextBoxFor(expression, SetAttributes(!canUpdate, attributes));
            var valMsg = htmlHelper.ValidationMessageFor(expression);
            if (valMsg != null) result = new MvcHtmlString(valMsg.ToHtmlString() + result.ToHtmlString());
            if (!canUpdate)
            {
                result = new MvcHtmlString(result.ToHtmlString() + htmlHelper.HiddenFor(expression).ToHtmlString());
            }
            return result;
        }

        [Obsolete]
        public static MvcHtmlString EduDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList, object attributes = null, string fieldName = null, string label = null)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var canUpdate = fieldName == null ? CanUpdateFor(htmlHelper, expression) : CanUpdate(htmlHelper, fieldName);
            var result = htmlHelper.DropDownListFor(expression, selectList, label ?? string.Empty, SetAttributes(!canUpdate, attributes, expressionText));
            var valMsg = htmlHelper.ValidationMessageFor(expression);
            if (valMsg != null) result = new MvcHtmlString(valMsg.ToHtmlString() + result.ToHtmlString());
            if (!canUpdate)
            {
                result = new MvcHtmlString(result.ToHtmlString() + htmlHelper.HiddenFor(expression).ToHtmlString());
            }
            return result;
        }

        [Obsolete]
        public static MvcHtmlString PendingUpdateFor<TProperty>(this HtmlHelper<EstablishmentDetailViewModel> htmlHelper,
            Expression<Func<Establishment, TProperty>> expression)
        {
            var model = htmlHelper.ViewData.Model;
            if (model.IsUserLoggedOn && model.PendingChanges.Any(x => x.DataField.Equals(ExpressionHelper.GetExpressionText(expression))))
            {
                return new MvcHtmlString("<div class=\"pending-update-icon\" title=\"Pending update\">!</div>");
            }
            else return null;
        }

        [Obsolete]
        public static MvcHtmlString PendingUpdatesFor(this HtmlHelper<EstablishmentDetailViewModel> htmlHelper,
            params Expression<Func<Establishment, object>>[] expressions)
        {
            var model = htmlHelper.ViewData.Model;
            if (model.IsUserLoggedOn)
                if (expressions.Any(x => model.PendingChanges.Any(p => p.DataField.Equals(ExpressionHelper.GetExpressionText(x)))))
                    return new MvcHtmlString("<div class=\"pending-update-icon\" title=\"Pending update\">!</div>");
            return null;
        }

        [Obsolete]
        public static MvcHtmlString PendingUpdatesFor(this HtmlHelper<EstablishmentDetailViewModel> htmlHelper, params string[] fields)
        {
            var model = htmlHelper.ViewData.Model;
            if (model.IsUserLoggedOn)
                if (fields.Any(x => model.PendingChanges.Any(p => p.DataField.Equals(x))))
                    return new MvcHtmlString("<div class=\"pending-update-icon\" title=\"Pending update\">!</div>");
            return null;
        }


        private static Dictionary<string, object> SetAttributes(bool isDisabled, object otherAttributes = null, string id = null)
        {
            var d = new Dictionary<string, object>();
            d["class"] = "form-control";
            if (isDisabled) d["disabled"] = "disabled";
            if (id != null) d["id"] = id.Replace(".", "_").ToLower();

            if (otherAttributes != null)
            {
                var items = ReflectionHelper.GetProperties(otherAttributes);
                foreach (var item in items)
                {
                    d[item] = ReflectionHelper.GetProperty(otherAttributes, item);
                }
            }

            if (isDisabled) d["style"] = d.ContainsKey("style") ? (d["style"].ToString() + ";background-color:#eee;") : "background-color:#eee";
            return d;
        }

        [Obsolete]
        public static MvcHtmlString EduLocalAuthorityDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.EduDropDownFor(expression,  new CachedLookupService().LocalAuthorityGetAll().Select(x => new SelectListItem
            {
                Text = string.Concat(x.Name, "(", x.Id, ")"),
                Value = x.Id.ToString()
            }));
        }

        [Obsolete]
        public static MvcHtmlString EduHeadTitlesDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression) => htmlHelper.EduLookupDropDownFor(expression,  new CachedLookupService().HeadTitlesGetAll());

        [Obsolete]
        public static MvcHtmlString EduGendersDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                    Expression<Func<TModel, TProperty>> expression) => htmlHelper.EduLookupDropDownFor(expression,  new CachedLookupService().GendersGetAll());

        [Obsolete]
        public static MvcHtmlString EduEducationPhasesDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                    Expression<Func<TModel, TProperty>> expression) => htmlHelper.EduLookupDropDownFor(expression,  new CachedLookupService().EducationPhasesGetAll());

        public static MvcHtmlString EduAdmissionsPoliciesDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                            Expression<Func<TModel, TProperty>> expression) => htmlHelper.EduLookupDropDownFor(expression,  new CachedLookupService().AdmissionsPoliciesGetAll());

        [Obsolete]
        public static MvcHtmlString EduStatusesPoliciesDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                    Expression<Func<TModel, TProperty>> expression) => htmlHelper.EduLookupDropDownFor(expression,  new CachedLookupService().EstablishmentStatusesGetAll());

        [Obsolete]
        public static MvcHtmlString EduEstablishmentTypesDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression) => htmlHelper.EduLookupDropDownFor(expression,  new CachedLookupService().EstablishmentTypesGetAll());

        [Obsolete]
        public static MvcHtmlString EduGroupTypesDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression) => htmlHelper.EduLookupDropDownFor(expression,  new CachedLookupService().GroupTypesGetAll());

        /// <summary>
        /// Group Types drop down but limited to just single and multi-academy trusts
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        [Obsolete]
        public static MvcHtmlString EduGroupTypesForCreateDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                    Expression<Func<TModel, TProperty>> expression) => htmlHelper.EduLookupDropDownFor(
                        expression,  new CachedLookupService().GroupTypesGetAll().Where(x=>x.Name.Contains("Multi") 
                        || x.Name.Contains("Single")));

        [Obsolete]
        public static MvcHtmlString EduLookupDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, 
            TProperty>> expression, IEnumerable<LookupBase> items) => 
            htmlHelper.EduDropDownFor(expression, items.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }));

        [Obsolete]
        public static MvcHtmlString EduLookupDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, IEnumerable<LookupDto> items) =>
            htmlHelper.EduDropDownFor(expression, items.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }));

        [Obsolete]
        public static MvcHtmlString EduLookupDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> modelExpression,
            Expression<Func<CachedLookupService, IEnumerable<LookupDto>>> lookupSourceExpression)
        {
            var items = lookupSourceExpression.Compile()( new CachedLookupService());
            return htmlHelper.EduDropDownFor(modelExpression, items.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }));
        }

        /// <summary>
        /// Gets the name of the item associated with a Lookup item id
        /// </summary>
        /// <typeparam name="EstablishmentDetailViewModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        [Obsolete]
        public static IHtmlString NameFor<EstablishmentDetailViewModel>(this HtmlHelper<EstablishmentDetailViewModel> htmlHelper,
            Expression<Func<EstablishmentDetailViewModel, int?>> expression)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression).Split('.').Last();
            var id = expression.Compile()(htmlHelper.ViewData.Model);
            if ( new CachedLookupService().IsLookupField(expressionText))
            {
                if (id.HasValue) return new MvcHtmlString( new CachedLookupService().GetName(expressionText, id.Value));
                else return MvcHtmlString.Empty;
            }
            else return new MvcHtmlString("_ new CachedLookupService()_NOT_RECOGNISED__");
        }


        public static IHtmlString Json<TModel>(this HtmlHelper<TModel> htmlHelper, object data) => htmlHelper.Raw(JsonConvert.SerializeObject(data, Formatting.None, 
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));


        public static IHtmlString Conditional<TModel>(this HtmlHelper<TModel> htmlHelper, bool condition, string text)
            => condition ? htmlHelper.Raw(text) : MvcHtmlString.Empty;


        public static IHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, bool condition, Expression<Func<TModel, TProperty>> expression)
         => condition ? htmlHelper.HiddenFor(expression) : MvcHtmlString.Empty;

        public static IDisposable BeginFormRetainQuery(this HtmlHelper html, string action, string controller, FormMethod method)
        {
            var routeValues = new RouteValueDictionary();
            var query = html.ViewContext.HttpContext.Request.QueryString;
            foreach (string key in query)
            {
                routeValues[key] = query[key];
            }
            return html.BeginForm(action, controller, routeValues, FormMethod.Get);
        }

        public static IHtmlString HiddenFieldsFromQueryString(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            var query = html.ViewContext.HttpContext.Request.QueryString;
            foreach (var item in query.AllKeys)
            {
                foreach (var item2 in query.GetValues(item))
                {
                    sb.AppendLine(html.Hidden(item, item2).ToString());
                }
            }
            return new MvcHtmlString(sb.ToString());

        }

    }
}