using Edubase.Common.Reflection;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Edubase.Services.Governors.Models;

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
            return state.Errors.Count == 0 ? MvcHtmlString.Empty : new MvcHtmlString("error");
        }

        public static MvcHtmlString DuplicateCssClassFor(this HtmlHelper htmlHelper, int? governorId)
        {
            if (htmlHelper.ViewContext.ViewData.ContainsKey("DuplicateGovernor") && governorId.HasValue)
            {
                var duplicate = (GovernorModel)htmlHelper.ViewContext.ViewData["DuplicateGovernor"];
                if (governorId == duplicate.Id)
                {
                    return new MvcHtmlString("error");
                }
            }

            return MvcHtmlString.Empty;
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

            if (isDisabled) d["style"] = d.ContainsKey("style") ? (d["style"] + ";background-color:#eee;") : "background-color:#eee";
            return d;
        }
        
        
        public static MvcHtmlString EduDayDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, object attributes)
        {
            var v = expression.Compile()(htmlHelper.ViewData.Model);
            var items = Enumerable.Range(1, 31)
                .Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString(), Selected = (x.ToString() == v?.ToString()) });
            return htmlHelper.DropDownListFor(expression, items, "", attributes);
        }

        public static MvcHtmlString EduMonthDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, object attributes)
        {
            var v = expression.Compile()(htmlHelper.ViewData.Model);
            var items = Enumerable.Range(1, 12)
                .Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString(),
                    Selected = (x.ToString() == v?.ToString())});
            return htmlHelper.DropDownListFor(expression, items, "", attributes);
        }

        public static MvcHtmlString EduYearDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, object attributes)
        {
            var v = expression.Compile()(htmlHelper.ViewData.Model);
            var start = 1900;
            var count = (DateTime.UtcNow.Year + 5) - start;
            var items = Enumerable.Range(start, count).Reverse()
                .Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString(),
                    Selected = (x.ToString() == v?.ToString()) });
            return htmlHelper.DropDownListFor(expression, items, "", attributes);
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

        /// <summary>
        /// Puts all the stuff that's current in the querystring into hidden form fields.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString HiddenFieldsFromQueryString(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            var query = html.ViewContext.HttpContext.Request.QueryString;
            foreach (var item in query.AllKeys)
            {
                var vals = query.GetValues(item);
                foreach (var item2 in vals)
                {
                    sb.AppendLine("\r\n\t\t\t\t\t\t\t\t\t" + $@"<input type=""hidden"" name=""{item}"" value=""{item2}"" />");
                }
            }
            return new MvcHtmlString(sb.ToString());

        }

        public static HtmlHelper<TModel> For<TModel>(this HtmlHelper helper) where TModel : class, new()
        {
            return For<TModel>(helper.ViewContext, helper.ViewDataContainer.ViewData, helper.RouteCollection);
        }

        public static HtmlHelper<TModel> For<TModel>(this HtmlHelper helper, TModel model)
        {
            return For<TModel>(helper.ViewContext, helper.ViewDataContainer.ViewData, helper.RouteCollection, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, RouteCollection routeCollection) where TModel : class, new()
        {
            TModel model = new TModel();
            return For<TModel>(viewContext, viewData, routeCollection, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, RouteCollection routeCollection, TModel model)
        {
            var newViewData = new ViewDataDictionary(viewData) { Model = model };
            ViewContext newViewContext = new ViewContext(
                viewContext.Controller.ControllerContext,
                viewContext.View,
                newViewData,
                viewContext.TempData,
                viewContext.Writer);
            var viewDataContainer = new ViewDataContainer(newViewContext.ViewData);
            return new HtmlHelper<TModel>(newViewContext, viewDataContainer, routeCollection);
        }

        private class ViewDataContainer : System.Web.Mvc.IViewDataContainer
        {
            public System.Web.Mvc.ViewDataDictionary ViewData { get; set; }

            public ViewDataContainer(System.Web.Mvc.ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }
        }

    }
}