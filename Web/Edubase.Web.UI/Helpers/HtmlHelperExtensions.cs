using Edubase.Common;
using Edubase.Services.Governors.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace Edubase.Web.UI.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static readonly HtmlString EmptyHtml = new HtmlString(string.Empty);

        public static HtmlString ValidationCssClassFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var fullHtmlFieldName = htmlHelper.NameFor(expression);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            return state == null || state.Errors.Count == 0 ? EmptyHtml : new HtmlString("govuk-form-group--error");
        }

        public static HtmlString ValidationCssClass(this IHtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null || state.Errors.Count == 0 ? EmptyHtml : new HtmlString("govuk-error-message");
        }

        public static HtmlString ValidationGroupCssClass(this IHtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null || state.Errors.Count == 0 ? EmptyHtml : new HtmlString("govuk-form-group--error");
        }

        public static HtmlString ValidationSelectCssClass(this IHtmlHelper htmlHelper, string modelName)
        {
            var state = htmlHelper.ViewData.ModelState[modelName];
            return state == null || state.Errors.Count == 0 ? EmptyHtml : new HtmlString("govuk-select--error");
        }

        public static HtmlString TextBoxValidationClass<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var fullHtmlFieldName = htmlHelper.NameFor(expression);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            return state == null || state.Errors.Count == 0 ? EmptyHtml : new HtmlString("govuk-input--error");
        }

        public static IHtmlContent ValidationMessageNested(this IHtmlHelper htmlHelper, string modelName)
        {
            var fullFieldName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(modelName);
            if (!htmlHelper.ViewData.ModelState.ContainsKey(fullFieldName))
            {
                if (htmlHelper.ViewData.ModelMetadata.PropertyName != null && htmlHelper.ViewData.ModelState.ContainsKey(htmlHelper.ViewData.ModelMetadata.PropertyName))
                {
                    if (htmlHelper.ViewData.ModelState.TryGetValue(modelName, out var modelState) && modelState.Errors.Any())
                    {
                        foreach (var error in modelState.Errors)
                        {
                            var splitName = SplitNameAndCapitaliseFirstLetter(error);
                            htmlHelper.ViewData.ModelState.AddModelError(fullFieldName, splitName);
                        }
                    }
                }
            }

            return htmlHelper.ValidationMessage(modelName, message: null, htmlAttributes: new { @class = "govuk-error-message" });
        }

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
                var duplicate = (GovernorModel)htmlHelper.ViewContext.ViewData["DuplicateGovernor"];
                if (governorId == duplicate.Id)
                {
                    return new HtmlString("error");
                }
            }

            return EmptyHtml;
        }

        public static IHtmlContent Json<TModel>(this IHtmlHelper<TModel> htmlHelper, object data) => htmlHelper.Raw(JsonConvert.SerializeObject(data, Formatting.None,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

        public static IHtmlContent Conditional<TModel>(this IHtmlHelper<TModel> htmlHelper, bool condition, string text)
            => condition ? htmlHelper.Raw(text) : EmptyHtml;

        public static IHtmlContent Conditional<TModel>(this IHtmlHelper<TModel> htmlHelper, bool condition, IHtmlContent html)
            => condition ? html : EmptyHtml;

        public static IHtmlContent HiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, bool condition, Expression<Func<TModel, TProperty>> expression)
            => condition ? htmlHelper.HiddenFor(expression) : EmptyHtml;

        public static IHtmlContent HiddenFieldsFromQueryString(this IHtmlHelper html, string[] keysToExclude = null)
        {
            var sb = new StringBuilder();
            var query = html.ViewContext.HttpContext.Request.Query;
            var keys = query.Keys.AsEnumerable();

            if (keysToExclude != null)
            {
                keys = keys.Where(k => !keysToExclude.Contains(k));
            }

            foreach (var item in keys)
            {
                foreach (var item2 in query[item])
                {
                    sb.AppendLine($"<input type=\"hidden\" name=\"{HtmlEncoder.Default.Encode(item)}\" value=\"{HtmlEncoder.Default.Encode(item2)}\" />");
                }
            }
            return new HtmlString(sb.ToString());
        }

        public static IHtmlHelper<TModel> For<TModel>(this IHtmlHelper helper) where TModel : class, new()
        {
            throw new NotSupportedException("IHtmlHelper.For<TModel>() is not supported on ASP.NET Core.");
        }

        public static IHtmlHelper<TModel> For<TModel>(this IHtmlHelper helper, TModel model)
        {
            throw new NotSupportedException("IHtmlHelper.For<TModel>(model) is not supported on ASP.NET Core.");
        }

        public static IHtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, RouteValueDictionary routeCollection) where TModel : class, new()
        {
            throw new NotSupportedException("IHtmlHelper factory methods are not supported on ASP.NET Core.");
        }

        public static IHtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, RouteValueDictionary routeCollection, TModel model)
        {
            throw new NotSupportedException("IHtmlHelper factory methods are not supported on ASP.NET Core.");
        }

        public static IHtmlContent FileSizeInMegabytes(this IHtmlHelper html, long? fileSizeInBytes, int decimalPlaces = 2, double minimumValue = 0)
        {
            if (fileSizeInBytes.HasValue)
            {
                var mb = Math.Round((double)fileSizeInBytes.Value / 1024 / 1024, decimalPlaces);
                var result = mb > minimumValue ? mb : minimumValue;
                return new HtmlString(result + " MB");
            }

            return EmptyHtml;
        }

        public static string Field<TModel>(this IHtmlHelper<TModel> htmlHelper, object obj, string dateFormat = null)
            => (obj is DateTime? ? ((DateTime?)obj)?.ToString(dateFormat ?? "d MMMM yyyy").Clean() : obj?.ToString().Clean()) ?? "Not recorded";

        private const string AssetsPath = "public/assets/scripts/build";

        public static string GetWebpackScriptUrl(this IHtmlHelper helper, string expression, string path = null)
        {
            if (path == null)
            {
                path = AppContext.BaseDirectory;
            }

            var files = Directory.GetFiles(Path.Combine(path, AssetsPath), expression).Select(Path.GetFileName).ToList();

            return files.Count == 1 ? $"/{AssetsPath}/{files[0]}" : "";
        }

        public static IDictionary<string, object> GetHtmlAttributes(
            this IHtmlHelper helper,
            object fixedHtmlAttributes = null,
            IDictionary<string, object> dynamicHtmlAttributes = null
        )
        {
            var rvd = fixedHtmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(fixedHtmlAttributes);

            if (dynamicHtmlAttributes != null)
            {
                foreach (var kvp in dynamicHtmlAttributes)
                {
                    rvd[kvp.Key] = kvp.Value;
                }
            }

            return rvd;
        }

        public static HtmlString DropDownListWithIds<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectListItem,
            object htmlAttributes)
        {
            var expressionText = htmlHelper.NameFor(expression);

            var dropdown = new TagBuilder("select");

            dropdown.Attributes.Add("name", expressionText);
            dropdown.Attributes.Add("id", expressionText);

            foreach (var item in selectListItem)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = item.Value;
                option.Attributes["id"] = expressionText + "-option-" + item.Text;
                if (item.Selected)
                {
                    option.Attributes["selected"] = "selected";
                }

                option.InnerHtml.Append(item.Text);
                dropdown.InnerHtml.AppendHtml(option);
            }

            dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            using var writer = new StringWriter();
            dropdown.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }

        public static HtmlString HtmlNewlines(this IHtmlHelper helper, string input)
        {
            var htmlEncodedString = helper.Encode(input);
            return ConvertRawNewlineToHtmlBr(htmlEncodedString);
        }

        public static HtmlString HtmlNewlines(this IHtmlHelper helper, HtmlString input)
        {
            var htmlEncodedString = helper.Encode(input);
            return ConvertRawNewlineToHtmlBr(htmlEncodedString);
        }

        private static HtmlString ConvertRawNewlineToHtmlBr(string input)
        {
            var newlinePattern = "\r\n|\r|\n";
            var htmlNewlineString = "<br/>";

            var replace = Regex.Replace(input, newlinePattern, htmlNewlineString);
            return new HtmlString(replace);
        }
    }
}
