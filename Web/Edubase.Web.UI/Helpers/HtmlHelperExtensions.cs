using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Services.Governors.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using HtmlHelper = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper;
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using ModelError = Microsoft.AspNetCore.Mvc.ModelBinding.ModelError;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;
using TagRenderMode = Microsoft.AspNetCore.Mvc.Rendering.TagRenderMode;

namespace Edubase.Web.UI.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString ValidationCssClassFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            return state == null || state.Errors.Count == 0
                ? HtmlString.Empty
                : new HtmlString("govuk-form-group--error");
        }

        public static HtmlString ValidationCssClass(this IHtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null || state.Errors.Count == 0
                ? HtmlString.Empty
                : new HtmlString("govuk-error-message");
        }

        public static HtmlString ValidationGroupCssClass(this IHtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null || state.Errors.Count == 0
                ? HtmlString.Empty
                : new HtmlString("govuk-form-group--error");
        }

        public static HtmlString ValidationSelectCssClass(this IHtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null || state.Errors.Count == 0
                ? HtmlString.Empty
                : new HtmlString("govuk-select--error");
        }

        public static HtmlString TextBoxValidationClass<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            return state == null || state.Errors.Count == 0
                ? HtmlString.Empty
                : new HtmlString("govuk-input--error");
        }

        public static HtmlString ValidationMessageNested(this IHtmlHelper htmlHelper, string modelName)
        {
            var fullFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(modelName);
            if (!htmlHelper.ViewData.ModelState.ContainsKey(fullFieldName))
            {
                var metadata = htmlHelper.ViewData.ModelMetadata;
                if (metadata != null && !string.IsNullOrEmpty(metadata.PropertyName) &&
                    htmlHelper.ViewData.ModelState.ContainsKey(metadata.PropertyName))
                {
                    var modelState = htmlHelper.ViewData.ModelState[modelName];
                    if (modelState != null && modelState.Errors.Any())
                    {
                        foreach (var error in modelState.Errors)
                        {
                            var splitName = SplitNameAndCapitaliseFirstLetter(error);
                            htmlHelper.ViewData.ModelState.AddModelError(fullFieldName, splitName);
                        }
                    }
                }
            }

            return (HtmlString) htmlHelper.ValidationMessage(modelName, null, new { @class = "govuk-error-message" });
        }

        /// <summary>
        /// Splits the combined words in an error message where an uppercase letter follows a lowercase
        /// then capitalises the first letter of the string, and converts the rest to lower
        /// </summary>
        /// <param name="error">Error message (ModelError)</param>
        /// <returns>String with first letter capitalized and the subsequent letters in lowercase</returns>
        public static string SplitNameAndCapitaliseFirstLetter(ModelError error)
        {
            if (string.IsNullOrEmpty(error.ErrorMessage))
                return string.Empty;

            var matchTimeout = TimeSpan.FromMilliseconds(100);
            var regex = new Regex("([a-z])([A-Z])", RegexOptions.Compiled, matchTimeout);
            var splitName = regex.Replace(error.ErrorMessage, "$1 $2");

            if (string.IsNullOrEmpty(splitName))
                return string.Empty;

            splitName = char.ToUpper(splitName[0]) + splitName.Substring(1).ToLower();

            return splitName;
        }

        public static HtmlString DuplicateCssClassFor(this IHtmlHelper htmlHelper, int? governorId)
        {
            if (htmlHelper.ViewContext.ViewData.ContainsKey("DuplicateGovernor") && governorId.HasValue)
            {
                var duplicate = (GovernorModel) htmlHelper.ViewContext.ViewData["DuplicateGovernor"];
                if (governorId == duplicate.Id)
                {
                    return new HtmlString("error");
                }
            }

            return HtmlString.Empty;
        }


        public static HtmlString Json<TModel>(this IHtmlHelper<TModel> htmlHelper, object data) => (HtmlString) htmlHelper.Raw(JsonConvert.SerializeObject(data, Formatting.None,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

        public static HtmlString Conditional<TModel>(this IHtmlHelper<TModel> htmlHelper, bool condition, string text)
            => (HtmlString) (condition ? htmlHelper.Raw(text) : HtmlString.Empty);

        public static HtmlString Conditional<TModel>(this IHtmlHelper<TModel> htmlHelper, bool condition, HtmlString html)
            => condition ? html : HtmlString.Empty;

        public static HtmlString HiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, bool condition, Expression<Func<TModel, TProperty>> expression)
         => (HtmlString) (condition ? htmlHelper.HiddenFor(expression) : HtmlString.Empty);

        /// <summary>
        /// Puts all the stuff that's current in the querystring into hidden form fields.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static HtmlString HiddenFieldsFromQueryString(this IHtmlHelper html, string[] keysToExclude = null)
        {
            var sb = new StringBuilder();
            var query = html.ViewContext.HttpContext.Request.Query;
            IEnumerable<KeyValuePair<string, StringValues>> keys = query;

            if (keysToExclude != null)
            {
                // Filter the keys, but do not try to assign back to IQueryCollection
                keys = keys.Where(kvp => !keysToExclude.Contains(kvp.Key));
            }

            foreach (var item in keys)
            {
                var vals = query[item.Key];
                foreach (var item2 in vals)
                {
                    sb.AppendLine("\r\n\t\t\t\t\t\t\t\t\t" + $@"<input type=""hidden"" name=""{HttpUtility.HtmlEncode(item.Key)}"" value=""{HttpUtility.HtmlEncode(item2)}"" />");
                }
            }
            return new HtmlString(sb.ToString());

        }

        public static IHtmlHelper<TModel> For<TModel>(this IHtmlHelper helper) where TModel : class, new()
        {
            // Use the ASP.NET Core types for ViewContext and ViewDataDictionary
            return For<TModel>(helper.ViewContext, helper.ViewData);
        }

        public static IHtmlHelper<TModel> For<TModel>(Microsoft.AspNetCore.Mvc.Rendering.ViewContext viewContext, Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary viewData) where TModel : class, new()
        {
            var model = new TModel();
            var newViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<TModel>(viewData, model);
            var newViewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext(
                viewContext,
                viewContext.View,
                newViewData,
                viewContext.TempData,
                viewContext.Writer,
                default
            );
            return new HtmlHelper<TModel>(newViewContext, newViewContext.ViewData);
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
        public static HtmlString FileSizeInMegabytes(this IHtmlHelper html, long? fileSizeInBytes, int decimalPlaces = 2, double minimumValue = 0)
        {
            if (fileSizeInBytes.HasValue)
            {
                var mb = Math.Round((double) fileSizeInBytes.Value / 1024 / 1024, decimalPlaces);
                var result = mb > minimumValue ? mb : minimumValue;
                return new HtmlString(result.ToString() + " MB");
            }
            else
            {
                return new HtmlString(string.Empty);
            }
        }

        /// <summary>
        /// Outputs the ToString value of an object; if the value is empty or null, it'll output "Not recorded"
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Field<TModel>(this IHtmlHelper<TModel> htmlHelper, object obj, string dateFormat = null)
            => (obj is DateTime? ? ((DateTime?) obj)?.ToString(dateFormat ?? "d MMMM yyyy").Clean() : obj?.ToString().Clean()) ?? "Not recorded";


        private const string AssetsPath = "public/assets/scripts/build";
        /// <summary>
        /// Outputs the path to the requested js by name (ignoring file hash)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="path"></param>
        /// <returns>path to js file</returns>
        public static string GetWebpackScriptUrl(this IHtmlHelper helper, string expression, string path = null)
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
            this IHtmlHelper helper,
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
        public static HtmlString DropDownListWithIds<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
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
                options = options.Append(
                    "<option " +
                    " value='" + item.Value + "'" +
                    " id='" + expressionText + "-option-" + item.Text + "'" +
                    (item.Selected ? " selected='selected'" : "") +
                    ">" +
                    item.Text +
                    "</option>"
                );
            }

            dropdown.InnerHtml.AppendHtml(options.ToString());
            dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return new HtmlString(dropdown.ToString());
        }

        /// <summary>
        /// Where the input contains a raw newline character (<c>\r\n</c>, <c>\r</c>, or <c>\n</c>),
        /// replace it with an HTML newline tag <c>&lt;br/&gt;</c>.
        /// <br/>
        /// See also <see cref="HtmlNewlines(System.Web.Mvc.HtmlHelper,MvcHtmlString)"/> if wanting
        /// to retain e.g. MVC-generated HTML during the encoding process.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="input">The input string to have raw newlines replaced with HTML newline tags.</param>
        /// <returns>
        ///     The original string as an <see cref="MvcHtmlString"/>.
        ///     <list type="bullet">
        ///         <item>If no newlines in the input string, it is an as-is copy.</item>
        ///         <item>If one or more raw newlines, these will be HTML newlines.</item>
        ///     </list>
        /// </returns>
        public static HtmlString HtmlNewlines(this IHtmlHelper helper, string input)
        {
            // Restrict/prevent raw HTML coming from API in error message.
            // The string is not trusted (e.g., not generated/assembled via MVC utilities),
            // therefore encode/escape it wholly.
            var htmlEncodedString = helper.Encode(input);
            return ConvertRawNewlineToHtmlBr(htmlEncodedString);
        }

        /// <summary>
        /// Where the input contains a raw newline character (<c>\r\n</c>, <c>\r</c>, or <c>\n</c>),
        /// replace it with an HTML newline tag <c>&lt;br/&gt;</c>.
        /// <br/>
        /// See also <see cref="HtmlNewlines(System.Web.Mvc.HtmlHelper,string)"/> if wanting
        /// to treat the whole input as untrusted, therefore to be wholly encoded.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="input">The input string to have raw newlines replaced with HTML newline tags.</param>
        /// <returns>
        ///     The original string as an <see cref="MvcHtmlString"/>.
        ///     <list type="bullet">
        ///         <item>If no newlines in the input string, it is an as-is copy.</item>
        ///         <item>If one or more raw newlines, these will be HTML newlines.</item>
        ///     </list>
        /// </returns>
        public static HtmlString HtmlNewlines(this IHtmlHelper helper, HtmlString input)
        {
            // Retain context of this being an `MvcHtmlString` when doing the `helper.Encode`,
            // to avoid double-encoding of "trusted" HTML in this (e.g., assembled within the view).
            var htmlEncodedString = helper.Encode(input);
            return ConvertRawNewlineToHtmlBr(htmlEncodedString);
        }

        private static HtmlString ConvertRawNewlineToHtmlBr(string input)
        {
            var newlinePattern = "\r|\n|\r\n";
            var htmlNewlineString = "<br/>";

            var replace = Regex.Replace(input, newlinePattern, htmlNewlineString);
            var inputWithBrNewlines = new HtmlString(replace);

            return inputWithBrNewlines;
        }
    }
}
