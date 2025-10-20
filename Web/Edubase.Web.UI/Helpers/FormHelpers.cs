using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using HtmlHelper = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;

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
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var name = ExpressionHelper.GetExpressionText(expression);
            var id = helper.ViewData.TemplateInfo.GetFullHtmlFieldName(name + "_" + value).ToLower();

            if (attributes.ContainsKey("id"))
            {
                id = attributes["id"].ToString();
            }

            var viewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<TModel>(helper.ViewData) { { "id", id } };
            foreach (var kvp in attributes)
            {
                viewData[kvp.Key] = kvp.Value;
            }

            var radioButton = helper.RadioButtonFor(expression, value, viewData);
            var radioHtml = RenderHtmlContent(radioButton);

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", id);
            labelBuilder.AddCssClass("govuk-label govuk-radios__label");
            if (!string.IsNullOrEmpty(additionalLabelClasses))
                labelBuilder.AddCssClass(additionalLabelClasses);
            labelBuilder.InnerHtml.Append(labelText);

            var labelHtml = RenderTagBuilder(labelBuilder);

            return new HtmlString($"<div class=\"govuk-radios__item\">{radioHtml}{labelHtml}</div>");
        }

        public static HtmlString GiasRadio(
            string inputValue,
            string inputName,
            string labelText,
            string additionalLabelClasses = "",
            object htmlAttributes = null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var id = attributes.ContainsKey("id") ? attributes["id"].ToString() : $"{inputName}_{inputValue}";

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", id);
            labelBuilder.AddCssClass("govuk-label govuk-radios__label");
            if (!string.IsNullOrEmpty(additionalLabelClasses))
                labelBuilder.AddCssClass(additionalLabelClasses);
            labelBuilder.InnerHtml.Append(labelText);

            var inputBuilder = new TagBuilder("input");
            inputBuilder.Attributes["type"] = "radio";
            inputBuilder.Attributes["name"] = inputName;
            inputBuilder.Attributes["value"] = inputValue;
            inputBuilder.Attributes["id"] = id;
            inputBuilder.AddCssClass("govuk-radios__input");

            foreach (var kvp in attributes)
            {
                inputBuilder.Attributes[kvp.Key] = kvp.Value?.ToString();
            }

            var inputHtml = RenderTagBuilder(inputBuilder);
            var labelHtml = RenderTagBuilder(labelBuilder);

            return new HtmlString($"<div class=\"govuk-radios__item\">{inputHtml}{labelHtml}</div>");
        }

        public static HtmlString GiasCheckboxFor<TModel>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            string labelText = "",
            string additionalLabelClasses = "",
            object htmlAttributes = null)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var id = attributes.ContainsKey("id") ? attributes["id"].ToString() : helper.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            var viewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<TModel>(helper.ViewData) { { "id", id } };
            foreach (var kvp in attributes)
            {
                viewData[kvp.Key] = kvp.Value;
            }

            var checkbox = helper.CheckBoxFor(expression, viewData);
            var checkboxHtml = RenderHtmlContent(checkbox);

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", id);
            labelBuilder.AddCssClass("govuk-label govuk-checkboxes__label");
            if (!string.IsNullOrEmpty(additionalLabelClasses))
                labelBuilder.AddCssClass(additionalLabelClasses);
            labelBuilder.InnerHtml.Append(labelText);

            var labelHtml = RenderTagBuilder(labelBuilder);

            return new HtmlString($"<div class=\"govuk-checkboxes__item\">{checkboxHtml}{labelHtml}</div>");
        }

        public static HtmlString GiasCheckbox(
            string inputValue,
            string inputName,
            string labelText,
            string additionalLabelClasses = "",
            object htmlAttributes = null,
            bool isChecked = false)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var id = attributes.ContainsKey("id") ? attributes["id"].ToString() : $"{inputName}_{inputValue}";

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", id);
            labelBuilder.AddCssClass("govuk-label govuk-checkboxes__label");
            if (!string.IsNullOrEmpty(additionalLabelClasses))
                labelBuilder.AddCssClass(additionalLabelClasses);
            labelBuilder.InnerHtml.Append(labelText);

            var inputBuilder = new TagBuilder("input");
            inputBuilder.Attributes["type"] = "checkbox";
            inputBuilder.Attributes["name"] = inputName;
            inputBuilder.Attributes["value"] = inputValue;
            inputBuilder.Attributes["id"] = id;
            inputBuilder.AddCssClass("govuk-checkboxes__input");

            if (isChecked)
            {
                inputBuilder.Attributes["checked"] = "checked";
            }

            foreach (var kvp in attributes)
            {
                inputBuilder.Attributes[kvp.Key] = kvp.Value?.ToString();
            }

            var inputHtml = RenderTagBuilder(inputBuilder);
            var labelHtml = RenderTagBuilder(labelBuilder);

            return new HtmlString($"<div class=\"govuk-checkboxes__item\">{inputHtml}{labelHtml}</div>");
        }

        private static string RenderHtmlContent(IHtmlContent content)
        {
            using var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        private static string RenderTagBuilder(TagBuilder tagBuilder)
        {
            using var writer = new StringWriter();
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
