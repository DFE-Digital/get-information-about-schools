using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Services.Governors.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using HtmlHelper = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper;
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using ModelError = Microsoft.AspNetCore.Mvc.ModelBinding.ModelError;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;

namespace Edubase.Web.UI.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string WebRootPath { get; set; }

        public static HtmlString ValidationCssClassFor<TModel, TProperty>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression)
        {
            var fullHtmlFieldName = htmlHelper.NameFor(expression).ToString();
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
            var fullHtmlFieldName = htmlHelper.NameFor(expression).ToString();
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            return state == null || state.Errors.Count == 0
                ? HtmlString.Empty
                : new HtmlString("govuk-input--error");
        }

        public static HtmlString ValidationMessageNested(this IHtmlHelper htmlHelper, string modelName)
        {
            var fullFieldName = htmlHelper.Name(modelName).ToString();

            if (!htmlHelper.ViewData.ModelState.ContainsKey(fullFieldName))
            {
                var parentProperty = htmlHelper.ViewData.ModelMetadata?.PropertyName;

                if (!string.IsNullOrEmpty(parentProperty) &&
                    htmlHelper.ViewData.ModelState.ContainsKey(parentProperty))
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
        /// Splits combined words in an error message where an uppercase letter follows a lowercase,
        /// then capitalizes the first letter and converts the rest to lowercase.
        /// </summary>
        /// <param name="error">Error message (ModelError)</param>
        /// <returns>Formatted string</returns>
        public static string SplitNameAndCapitaliseFirstLetter(ModelError error)
        {
            var message = error?.ErrorMessage;
            if (string.IsNullOrWhiteSpace(message))
                return string.Empty;

            var regex = new Regex("([a-z])([A-Z])", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
            var split = regex.Replace(message, "$1 $2");

            return string.Concat(
                char.ToUpper(split[0]),
                split.Substring(1).ToLower()
            );
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

        public static HtmlString Json<TModel>(this IHtmlHelper<TModel> htmlHelper, object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            return (HtmlString) htmlHelper.Raw(json);
        }

        public static HtmlString Conditional<TModel>(this IHtmlHelper<TModel> htmlHelper, bool condition, string text)
        {
            return (HtmlString) (condition ? htmlHelper.Raw(text) : new HtmlString(string.Empty));
        }

        public static HtmlString Conditional<TModel>(this IHtmlHelper<TModel> htmlHelper, bool condition, HtmlString html)
        {
            return condition ? html : new HtmlString(string.Empty);
        }

        public static IHtmlContent HiddenFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            bool condition,
            Expression<Func<TModel, TProperty>> expression) => condition ? htmlHelper.HiddenFor(expression) : HtmlString.Empty;

        public static HtmlString HiddenFieldsFromQueryString(this IHtmlHelper html, string[] keysToExclude = null)
        {
            var sb = new StringBuilder();
            var query = html.ViewContext.HttpContext.Request.Query;

            foreach (var key in query.Keys)
            {
                if (keysToExclude != null && keysToExclude.Contains(key))
                    continue;

                foreach (var value in query[key])
                {
                    sb.AppendLine($@"<input type=""hidden"" name=""{WebUtility.HtmlEncode(key)}"" value=""{WebUtility.HtmlEncode(value)}"" />");
                }
            }

            return new HtmlString(sb.ToString());
        }

        public static Task<IHtmlContent> For<TModel>(this IHtmlHelper htmlHelper, string partialViewName) where TModel : class, new()
        {
            var model = new TModel();
            return htmlHelper.PartialAsync(partialViewName, model);
        }

        public static Task<IHtmlContent> For<TModel>(this IHtmlHelper htmlHelper, string partialViewName, TModel model)
        {
            return htmlHelper.PartialAsync(partialViewName, model);
        }

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }

            public ViewDataContainer(ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }
        }

        /// <summary>
        /// Outputs the supplied file size in megabytes and appends 'MB', or returns an empty string if null.
        /// </summary>
        /// <param name="html">The HTML helper instance.</param>
        /// <param name="fileSizeInBytes">File size in bytes.</param>
        /// <param name="decimalPlaces">Number of decimal places to round to.</param>
        /// <param name="minimumValue">Minimum MB value to display.</param>
        /// <returns>Formatted file size string as HtmlString.</returns>
        public static HtmlString FileSizeInMegabytes(this IHtmlHelper html, long? fileSizeInBytes, int decimalPlaces = 2, double minimumValue = 0)
        {
            if (!fileSizeInBytes.HasValue)
                return new HtmlString(string.Empty);

            var mb = Math.Round(fileSizeInBytes.Value / 1024d / 1024d, decimalPlaces);
            var result = mb > minimumValue ? mb : minimumValue;

            return new HtmlString($"{result} MB");
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

        // Constant that defines the relative path (under wwwroot) where Webpack emits JS bundles.
        // In this setup, Webpack outputs to wwwroot/assets/scripts/
        private const string AssetsPath = "assets/scripts";

        /// <summary>
        /// Finds the Webpack‑built JS file by name (ignoring the content hash) and returns its URL.
        /// </summary>
        /// <param name="helper">The Razor HTML helper instance.</param>
        /// <param name="expression">
        ///   A search pattern (e.g. "main.*.js") used to match the bundle filename.
        ///   The wildcard allows you to ignore the hash part of the filename.
        /// </param>
        /// <param name="path">
        ///   Optional override for the root path. Defaults to WebRootPath (wwwroot).
        /// </param>
        /// <returns>
        ///   The URL to the matching JS file (e.g. "/assets/scripts/main.abc123.js"),
        ///   or an empty string if no single match is found.
        /// </returns>
        public static string GetWebpackScriptUrl(this IHtmlHelper helper, string expression, string path = null)
        {
            // If no path is provided, default to the web root (wwwroot).
            path ??= WebRootPath;

            // Search the assets/scripts folder for files matching the given expression.
            // Directory.GetFiles returns full paths; Select(Path.GetFileName) strips them down to just the filename.
            List<string> files =
                [.. Directory.GetFiles(Path.Combine(path, AssetsPath), expression)
                     .Select(Path.GetFileName)];

            // If exactly one file matches, return its URL relative to the site root.
            // Otherwise (no match or multiple matches), return an empty string.
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
        IEnumerable<SelectListItem> selectListItems,
        object htmlAttributes)
        {
            var expressionText = htmlHelper.NameFor(expression);

            var dropdown = new TagBuilder("select");
            dropdown.Attributes["name"] = expressionText;
            dropdown.Attributes["id"] = expressionText;
            dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            foreach (var item in selectListItems)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = item.Value;
                option.Attributes["id"] = $"{expressionText}-option-{item.Text}";
                if (item.Selected)
                {
                    option.Attributes["selected"] = "selected";
                }
                option.InnerHtml.Append(item.Text);
                dropdown.InnerHtml.AppendHtml(option);
            }

            return dropdown.Render();
        }

        private static string NameFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return ExpressionHelper.GetExpressionText(expression);
        }

        private static HtmlString Render(this TagBuilder tagBuilder)
        {
            var writer = new System.IO.StringWriter();
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
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
