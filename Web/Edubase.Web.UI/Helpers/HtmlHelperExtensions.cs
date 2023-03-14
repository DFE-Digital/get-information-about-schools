using Edubase.Common;
using Edubase.Services.Governors.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
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
            return state == null
                ? MvcHtmlString.Empty
                : state.Errors.Count == 0 ? MvcHtmlString.Empty : new MvcHtmlString("govuk-form-group--error");
        }

        public static MvcHtmlString ValidationCssClass(this HtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null ? MvcHtmlString.Empty : state.Errors.Count == 0 ? MvcHtmlString.Empty : new MvcHtmlString("govuk-error-message");
        }

        public static MvcHtmlString ValidationGroupCssClass(this HtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null
                ? MvcHtmlString.Empty
                : state.Errors.Count == 0 ? MvcHtmlString.Empty : new MvcHtmlString("govuk-form-group--error");
        }

        public static MvcHtmlString ValidationSelectCssClass(this HtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null ? MvcHtmlString.Empty : state.Errors.Count == 0 ? MvcHtmlString.Empty : new MvcHtmlString("govuk-select--error");
        }



        public static MvcHtmlString TextBoxValidationClass<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            return state == null ? MvcHtmlString.Empty : state.Errors.Count == 0 ? MvcHtmlString.Empty : new MvcHtmlString("govuk-input--error");
        }

        public static MvcHtmlString ValidationMessageNested(this HtmlHelper htmlHelper, string modelName)
        {
            var fullFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(modelName);
            if (!htmlHelper.ViewData.ModelState.ContainsKey(fullFieldName))
            {
                if (htmlHelper.ViewData.ModelState.ContainsKey(htmlHelper.ViewData.ModelMetadata.PropertyName))
                {
                    // add the errors from the modelName to the parent FullHtmlFieldName
                    if (htmlHelper.ViewData.ModelState[modelName].Errors.Any())
                    {
                        foreach (var error in htmlHelper.ViewData.ModelState[modelName].Errors)
                        {
                            var splitName = SplitNameAndCapitaliseFirstLetter(error);
                            htmlHelper.ViewData.ModelState.AddModelError(fullFieldName, splitName);
                        }
                    }
                }
            }

            return htmlHelper.ValidationMessage(modelName, (string) null, new { @class = "govuk-error-message"});
        }

        private static string SplitNameAndCapitaliseFirstLetter(ModelError error)
        {
            var splitName = Regex.Replace(error.ErrorMessage, "([a-z])([A-Z])", "$1 $2");
            splitName = splitName.Substring(0, 1) + splitName.Substring(1).ToLower();
            return splitName;
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


        public static IHtmlString Json<TModel>(this HtmlHelper<TModel> htmlHelper, object data) => htmlHelper.Raw(JsonConvert.SerializeObject(data, Formatting.None,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

        public static IHtmlString Conditional<TModel>(this HtmlHelper<TModel> htmlHelper, bool condition, string text)
            => condition ? htmlHelper.Raw(text) : MvcHtmlString.Empty;

        public static IHtmlString Conditional<TModel>(this HtmlHelper<TModel> htmlHelper, bool condition, IHtmlString html)
            => condition ? html : MvcHtmlString.Empty;

        public static IHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, bool condition, Expression<Func<TModel, TProperty>> expression)
         => condition ? htmlHelper.HiddenFor(expression) : MvcHtmlString.Empty;

        /// <summary>
        /// Puts all the stuff that's current in the querystring into hidden form fields.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString HiddenFieldsFromQueryString(this HtmlHelper html, string[] keysToExclude = null)
        {
            var sb = new StringBuilder();
            var query = html.ViewContext.HttpContext.Request.QueryString;
            var keys = query.AllKeys;

            if (keysToExclude != null)
            {
                keys = keys.Where(k => !keysToExclude.Contains(k)).ToArray();
            }

            foreach (var item in keys)
            {
                var vals = query.GetValues(item);
                foreach (var item2 in vals)
                {
                    sb.AppendLine("\r\n\t\t\t\t\t\t\t\t\t" + $@"<input type=""hidden"" name=""{HttpUtility.HtmlEncode(item)}"" value=""{HttpUtility.HtmlEncode(item2)}"" />");
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
            var model = new TModel();
            return For<TModel>(viewContext, viewData, routeCollection, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, RouteCollection routeCollection, TModel model)
        {
            var newViewData = new ViewDataDictionary(viewData) { Model = model };
            var newViewContext = new ViewContext(
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

        /// <summary>
        /// Outputs the supplied file size in megabytes and appends 'MB', or if the supplied bytes value is null, a zero length string is returned.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="fileSizeInBytes"></param>
        /// <param name="decimalPlaces"></param>
        /// <param name="minimumValue"></param>
        /// <returns></returns>
        public static IHtmlString FileSizeInMegabytes(this HtmlHelper html, long? fileSizeInBytes, int decimalPlaces = 2, double minimumValue = 0)
        {
            if (fileSizeInBytes.HasValue)
            {
                var mb = Math.Round((double)fileSizeInBytes.Value / 1024 / 1024, decimalPlaces);
                var result = mb > minimumValue ? mb : minimumValue;
                return new MvcHtmlString(result.ToString() + " MB");
            }
            else
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        /// <summary>
        /// Outputs the ToString value of an object; if the value is empty or null, it'll output "Not recorded"
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Field<TModel>(this HtmlHelper<TModel> htmlHelper, object obj, string dateFormat = null)
            => (obj is DateTime? ? ((DateTime?)obj)?.ToString(dateFormat ?? "d MMMM yyyy").Clean() : obj?.ToString().Clean()) ?? "Not recorded";


        private const string AssetsPath = "public/assets/scripts/build";
        /// <summary>
        /// Outputs the path to the requested js by name (ignoring file hash)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="path"></param>
        /// <returns>path to js file</returns>
        public static string GetWebpackScriptUrl(this HtmlHelper helper, string expression, string path = null)
        {
            if (path == null)
            {
                path = HttpRuntime.AppDomainAppPath;
            }

            var files = Directory.GetFiles(Path.Combine(path, AssetsPath), expression).Select(Path.GetFileName).ToList();

            return files.Count == 1 ? $"/{AssetsPath}/{files[0]}" : "";
        }

        /// <summary>
        /// Gets an object containing a htmlAttributes collection for any Razor HTML helper component,
        /// supporting a static set (anonymous object) and/or a dynamic set (Dictionary)
        /// </summary>
        /// <param name="fixedHtmlAttributes">A fixed set of htmlAttributes (anonymous object)</param>
        /// <param name="dynamicHtmlAttributes">A dynamic set of htmlAttributes (Dictionary)</param>
        /// <returns>A collection of htmlAttributes including a merge of the given set(s)</returns>
        public static IDictionary<string, object> GetHtmlAttributes(
            this HtmlHelper helper,
            object fixedHtmlAttributes = null,
            IDictionary<string, object> dynamicHtmlAttributes = null
        )
        {
            var rvd = (fixedHtmlAttributes == null)
                ? new RouteValueDictionary()
                : HtmlHelper.AnonymousObjectToHtmlAttributes(fixedHtmlAttributes);
            if (dynamicHtmlAttributes != null)
            {
                foreach (var kvp in dynamicHtmlAttributes)
                {
                    rvd[kvp.Key] = kvp.Value;
                }
            }
            return rvd;
        }

        /// <summary>
        /// Gets details for an Select drop down list and creates a new select with
        /// a unique ID for each option
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression">Data model to be used against</param>
        /// <param name="selectListItem">Items of the dropdown list. Usually a blank class to populate</param>
        /// <param name="htmlAttributes">Containing additonal attributes for the element, expected class usually</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListWithIds<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TProperty>> expression,
           IEnumerable<SelectListItem> selectListItem,
           object htmlAttributes)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);

            var dropdown = new TagBuilder("select");

            dropdown.Attributes.Add("name", expressionText);
            dropdown.Attributes.Add("id", expressionText);

            var options = new StringBuilder();

            foreach (var item in selectListItem)
            {
                options = options.Append("<option value='" + item.Value + "'id='" + expressionText + "-option-" + item.Text + "'>" + item.Text + "</option>");
            }

            dropdown.InnerHtml = options.ToString();
            dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
        }
    }
}
