using System.Web.Mvc;

namespace Edubase.Web.UI.Helpers
{
    using System;
    using System.Linq.Expressions;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    namespace Edubase.Web.UI.Helpers
    {
        public static class FormHelpers
        {
            public static IHtmlContent GiasRadioFor<TModel, TProperty>(
                this IHtmlHelper<TModel> helper,
                Expression<Func<TModel, TProperty>> expression,
                object value,
                string labelText = "",
                string additionalLabelClasses = "",
                object htmlAttributes = null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                var name = ExpressionHelper.GetExpressionText(expression);
                var id = helper.IdFor(expression).ToString();

                if (attributes.ContainsKey("id"))
                {
                    id = attributes["id"]?.ToString();
                }

                var viewData = new ViewDataDictionary<TModel>(
                    metadataProvider: new EmptyModelMetadataProvider(),
                    modelState: helper.ViewData.ModelState)
                    {
                        { "id", id }
                    };

                foreach (var kvp in attributes)
                {
                    viewData[kvp.Key] = kvp.Value;
                }

                var radioButton = helper.RadioButtonFor(expression, value, viewData);

                var label = new TagBuilder("label");
                label.Attributes["for"] = id;
                label.AddCssClass("govuk-label govuk-radios__label" + additionalLabelClasses);
                label.InnerHtml.AppendHtml(labelText);

                var container = new HtmlContentBuilder();
                container.AppendHtml(radioButton);
                container.AppendHtml(label);

                return container;
            }


            public static IHtmlContent GiasRadio(
                string inputValue,
                string inputName,
                string labelText,
                string additionalLabelClasses = "",
                object htmlAttributes = null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                var id = attributes.ContainsKey("id") ? attributes["id"].ToString() : $"{inputName}_{inputValue}";

                var labelBuilder = new TagBuilder("label");
                labelBuilder.Attributes["for"] = id;
                labelBuilder.AddCssClass("govuk-label govuk-radios__label" + additionalLabelClasses);
                labelBuilder.InnerHtml.Append(labelText); // Use Append for plain text

                TagBuilder radio = new("input")
                {
                    TagRenderMode = TagRenderMode.SelfClosing
                };
                radio.Attributes["type"] = "radio";
                radio.Attributes["name"] = inputName;
                radio.Attributes["value"] = inputValue;
                radio.Attributes["id"] = id;
                radio.AddCssClass("govuk-radios__input");

                foreach (var kvp in attributes)
                {
                    radio.Attributes[kvp.Key] = kvp.Value?.ToString();
                }

                TagBuilder container = new("div");
                container.AddCssClass("govuk-radios__item");
                container.InnerHtml.AppendHtml(radio);
                container.InnerHtml.AppendHtml(labelBuilder);

                return container;
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
                var id = helper.Id(name).ToString();

                if (attributes.ContainsKey("id"))
                {
                    id = attributes["id"].ToString();
                }

                var viewData = new ViewDataDictionary<TModel>(
                    metadataProvider: new EmptyModelMetadataProvider(),
                    modelState: helper.ViewData.ModelState)
            {
                { "id", id }
            };

                foreach (var kvp in attributes)
                {
                    viewData[kvp.Key] = kvp.Value;
                }

                var checkbox = helper.CheckBoxFor(expression, viewData);

                var labelBuilder = new TagBuilder("label");
                labelBuilder.Attributes["for"] = id;
                labelBuilder.AddCssClass("govuk-label govuk-checkboxes__label" + additionalLabelClasses);
                labelBuilder.InnerHtml.AppendHtml(labelText);

                return new HtmlString($"<div class=\"govuk-checkboxes__item\">{checkbox.ToString()}{labelBuilder.ToString()}</div>");
            }

            public static IHtmlContent GiasCheckbox(
                string inputValue,
                string inputName,
                string labelText,
                string additionalLabelClasses = "",
                object htmlAttributes = null,
                bool isChecked = false)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                var id = attributes.TryGetValue("id", out var value) ? value.ToString() : $"{inputName}_{inputValue}";

                TagBuilder labelBuilder = new("label");
                labelBuilder.Attributes["for"] = id;
                labelBuilder.AddCssClass("govuk-label govuk-checkboxes__label" + additionalLabelClasses);
                labelBuilder.InnerHtml.Append(labelText); // Use Append for plain text

                TagBuilder checkbox = new("input")
                {
                    TagRenderMode = TagRenderMode.SelfClosing
                };
                checkbox.Attributes["type"] = "checkbox";
                checkbox.Attributes["name"] = inputName;
                checkbox.Attributes["value"] = inputValue;
                checkbox.Attributes["id"] = id;
                checkbox.AddCssClass("govuk-checkboxes__input");

                if (isChecked)
                {
                    checkbox.Attributes["checked"] = "checked";
                }

                foreach (var kvp in attributes)
                {
                    checkbox.Attributes[kvp.Key] = kvp.Value?.ToString();
                }

                TagBuilder container = new("div");
                container.AddCssClass("govuk-checkboxes__item");
                container.InnerHtml.AppendHtml(checkbox);
                container.InnerHtml.AppendHtml(labelBuilder);

                return container;
            }
        }
    }
}
