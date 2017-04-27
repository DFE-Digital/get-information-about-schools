using Autofac;
using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using Edubase.Services;
using Edubase.Services.Domain;
using Edubase.Services.Lookup;
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
using Edubase.Services.Governors.Models;

namespace Edubase.Web.UI.Helpers
{
    public static class HtmlHelperExtensions
    {
        private enum DatePart { Day, Month, Year};

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

        public static MvcHtmlString RenderDateSelector<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, DateTimeViewModel>> dateTimeViewModel, string label)
        {
            var dateTimeHtmlHelper = htmlHelper.For<DateTimeViewModel>();

            var id = label.Replace(" ", "").ToLower();
            
            var divTag = new TagBuilder("div");
            divTag.AddCssClass("form-group");
            divTag.AddCssClass("create-edit-form-group");
            divTag.AddCssClass("drop-down-date");
            divTag.AddCssClass("range-group");
            divTag.AddCssClass(htmlHelper.ValidationCssClassFor(dateTimeViewModel).ToHtmlString());

            var fieldsetTag = new TagBuilder("fieldset");
            
            var legendTag = new TagBuilder("legend");
            legendTag.AddCssClass("bold-xsmall");
            legendTag.MergeAttribute("style", "font-weight:bold;");
            legendTag.SetInnerText(label);

            var hintDivTag = new TagBuilder("div");
            hintDivTag.AddCssClass("form-hint");
            hintDivTag.SetInnerText("For example, 20 03 2003");

            fieldsetTag.InnerHtml = $@"{legendTag}
                                       {hintDivTag}
                                       {htmlHelper.ValidationMessageFor(dateTimeViewModel)}
                                       {BuildDatePart(dateTimeHtmlHelper, DatePart.Day, id)}
                                       {BuildDatePart(dateTimeHtmlHelper, DatePart.Month, id)}
                                       {BuildDatePart(dateTimeHtmlHelper, DatePart.Year, id)}";

            divTag.InnerHtml = fieldsetTag.ToString();

            return new MvcHtmlString(divTag.ToString());
        }

        private static string BuildDatePart(HtmlHelper<DateTimeViewModel> htmlHelper, DatePart part, string id)
        {
            var labelFor = (part == DatePart.Day) ? $"{id}-day" : $"opendate-{part.ToString().ToLower()}";

            var spanTag = new TagBuilder("span");
            spanTag.AddCssClass("inline-form-control");

            var labelTag = new TagBuilder("label");
            labelTag.MergeAttribute("for", labelFor);
            labelTag.InnerHtml = part.ToString();

            var dropDown = part == DatePart.Day ? htmlHelper.EduDayDropDownFor(x => x.Day, new { id = $"{id}-day", @class = "form-control", aria_describedby = $"help-{id}-day"}) 
                         : part == DatePart.Month ? htmlHelper.EduMonthDropDownFor(x => x.Month, new { id = $"{id}-month", @class = "form-control", aria_describedby = $"help-{id}-month" })
                         : htmlHelper.EduYearDropDownFor(x => x.Year, new { id = $"{id}-year", @class = "form-control", aria_describedby = $"help-{id}-year" });

            spanTag.InnerHtml = $"{labelTag}{dropDown}";

            return spanTag.ToString();
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

            if (isDisabled) d["style"] = d.ContainsKey("style") ? (d["style"].ToString() + ";background-color:#eee;") : "background-color:#eee";
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