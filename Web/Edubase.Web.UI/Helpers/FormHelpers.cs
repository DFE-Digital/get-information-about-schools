using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace Edubase.Web.UI.Helpers
{
    public static class FormHelpers
    {
        public static HtmlString GiasRadioFor<TModel, TProperty>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression,
            object value,
            string labelText = "",
            string additionalLabelClasses = "",
            object htmlAttributes = null)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var baseId = helper.IdFor(expression);
            var id = string.Concat(baseId, "_", value).ToLowerInvariant();

            if (attributes.ContainsKey("id"))
            {
                id = attributes["id"]?.ToString();
            }

            var radioButton = helper.RadioButtonFor(expression, value, attributes);

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", id);
            labelBuilder.MergeAttribute("class", string.Concat("govuk-label govuk-radios__label", additionalLabelClasses));
            labelBuilder.InnerHtml.Append(labelText);

            return new HtmlString(ToHtml(radioButton) + ToHtml(labelBuilder));
        }

        public static HtmlString GiasRadio(
            string inputValue,
            string inputName,
            string labelText,
            string additionalLabelClasses = "",
            object htmlAttributes = null
        )
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var id = string.Concat(inputName, "_", inputValue);
            if (attributes.ContainsKey("id"))
            {
                id = attributes["id"]?.ToString();
            }

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", id);
            labelBuilder.MergeAttribute("class", string.Concat("govuk-label govuk-radios__label", additionalLabelClasses));
            labelBuilder.InnerHtml.Append(labelText);

            var checkbox = new TagBuilder("input");
            checkbox.Attributes["type"] = "radio";
            checkbox.Attributes["name"] = inputName;
            checkbox.Attributes["value"] = inputValue;
            checkbox.Attributes["id"] = id;
            checkbox.Attributes["class"] = "govuk-radios__input";

            foreach (var keyValue in attributes)
            {
                checkbox.Attributes[keyValue.Key] = keyValue.Value?.ToString();
            }

            return new HtmlString("<div class=\"govuk-radios__item\">" + ToHtml(checkbox) + ToHtml(labelBuilder) + "</div>");
        }

        public static HtmlString GiasCheckboxFor<TModel>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            string labelText = "",
            string additionalLabelClasses = "",
            object htmlAttributes = null)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var id = helper.IdFor(expression);
            if (attributes.ContainsKey("id"))
            {
                id = attributes["id"]?.ToString();
            }

            var checkbox = helper.CheckBoxFor(expression, attributes);

            var tagBuilder = new TagBuilder("label");
            tagBuilder.MergeAttribute("for", id);
            tagBuilder.MergeAttribute("class", string.Concat("govuk-label govuk-checkboxes__label", additionalLabelClasses));
            tagBuilder.InnerHtml.Append(labelText);

            return new HtmlString("<div class=\"govuk-checkboxes__item\">" + ToHtml(checkbox) + ToHtml(tagBuilder) + "</div>");
        }

        public static HtmlString GiasCheckbox(
            string inputValue,
            string inputName,
            string labelText,
            string additionalLabelClasses = "",
            object htmlAttributes = null,
            bool isChecked = false
        )
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var id = attributes.ContainsKey("id") ? attributes["id"]?.ToString() : string.Concat(inputName, "_", inputValue);

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", id);
            labelBuilder.MergeAttribute("class", string.Concat("govuk-label govuk-checkboxes__label", additionalLabelClasses));
            labelBuilder.InnerHtml.Append(labelText);

            var checkbox = new TagBuilder("input");
            checkbox.Attributes["type"] = "checkbox";
            checkbox.Attributes["name"] = inputName;
            checkbox.Attributes["value"] = inputValue;
            checkbox.Attributes["class"] = "govuk-checkboxes__input";

            if (isChecked)
            {
                checkbox.Attributes["checked"] = "checked";
            }

            foreach (var keyValue in attributes)
            {
                checkbox.Attributes[keyValue.Key] = keyValue.Value?.ToString();
            }

            return new HtmlString("<div class=\"govuk-checkboxes__item\">" + ToHtml(checkbox) + ToHtml(labelBuilder) + "</div>");
        }

        private static string ToHtml(IHtmlContent content)
        {
            using var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
