using Edubase.Common.Reflection;
using Edubase.Services.Governors.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public static MvcHtmlString EditorForGeneric<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var propertyType = typeof(TValue);
            if (!propertyType.IsGenericType)
                throw new ArgumentException();

            var genericType = propertyType.GetGenericArguments()[0];
            var templateName = $"{propertyType.Name.Split('`')[0]}_{genericType.Name}";
            return html.EditorFor<TModel, TValue>(expression, templateName);
        }

        public static MvcHtmlString DisplayForGeneric<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var propertyType = typeof(TValue);
            if (!propertyType.IsGenericType)
                throw new ArgumentException();

            var genericType = propertyType.GetGenericArguments()[0];
            var templateName = $"{propertyType.Name.Split('`')[0]}_{genericType.Name}";
            return html.DisplayFor<TModel, TValue>(expression, templateName);
        }

        //public static MvcHtmlString EdubaseValidationSummary(this HtmlHelper htmlHelper)
        //{
        //    return htmlHelper.EdubaseValidationSummary(false);
        //}

        //public static MvcHtmlString EdubaseValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors)
        //{

        //}
    }
}