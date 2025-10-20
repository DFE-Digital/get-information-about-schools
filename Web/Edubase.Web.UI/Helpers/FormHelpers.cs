using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Helpers
{
    public static class FormHelpers
    {
        /// <summary>
        ///  Radio button helper with model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="labelText"></param>
        /// <param name="additionalLabelClasses"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
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
            var id = helper.ViewData.TemplateInfo.GetFullHtmlFieldId(name + "_" + value).ToLower();

            if (attributes.ContainsKey("id"))
            { // radio button has a defined ID - be sure to use for the label for attr
                id = attributes["id"].ToString();
            }
            var viewData = new ViewDataDictionary() { { "id", id} };

            if (htmlAttributes != null)
            {
                var viewDataDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var keyValue in viewDataDictionary)
                {
                    viewData[keyValue.Key] = keyValue.Value;
                }
            }

            var radioButton = helper.RadioButtonFor(expression, value, viewData);

            var tagBuilder = new TagBuilder("label");
            tagBuilder.MergeAttribute("for", id);
            tagBuilder.MergeAttribute("class", string.Concat("govuk-label govuk-radios__label", additionalLabelClasses));
            tagBuilder.InnerHtml = labelText;

            return new HtmlString(radioButton.ToHtmlString() + tagBuilder);
        }


        /// <summary>
        /// Radio helper no model
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="inputName"></param>
        /// <param name="labelText"></param>
        /// <param name="additionalLabelClasses"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static HtmlString GiasRadio(
            string inputValue,
            string inputName,
            string labelText,
            string additionalLabelClasses = "",
            object htmlAttributes = null
        )
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var id = string.Concat(inputName, "_", inputValue);
            if (attributes.ContainsKey("id"))
            {
                id = attributes["id"].ToString();
            }

            var labelBuilder = new TagBuilder("label");

            labelBuilder.MergeAttribute("for", id);
            labelBuilder.MergeAttribute("class", string.Concat("govuk-label govuk-radios__label",additionalLabelClasses));
            labelBuilder.InnerHtml = labelText;

            var checkbox = new TagBuilder("input");
            checkbox.Attributes["type"] = "radio";
            checkbox.Attributes["name"] = inputName;
            checkbox.Attributes["value"] = inputValue;
            checkbox.Attributes["id"] = id;
            checkbox.Attributes["class"] = "govuk-radios__input";

            if (htmlAttributes != null)
            {
                var viewDataDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var keyValue in viewDataDictionary)
                {
                    checkbox.Attributes[keyValue.Key] = (string)keyValue.Value;
                }
            }

            return new HtmlString("<div class=\"govuk-radios__item\">" + checkbox + labelBuilder + "</div>");
        }

        /// <summary>
        ///  Checkbox Helper - with model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="labelText"></param>
        /// <param name="additionalLabelClasses"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static HtmlString GiasCheckboxFor<TModel>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            string labelText = "",
            string additionalLabelClasses = "",
            object htmlAttributes = null)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var id = helper.ViewData.TemplateInfo.GetFullHtmlFieldId(name);
            if (attributes.ContainsKey("id"))
            {
                id = attributes["id"].ToString();
            }

            var viewData = new ViewDataDictionary() { { "id", id } };

            if (htmlAttributes != null)
            {
                var viewDataDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var keyValue in viewDataDictionary)
                {
                    viewData[keyValue.Key] = keyValue.Value;
                }
            }

            var checkbox = helper.CheckBoxFor(expression, viewData);

            var tagBuilder = new TagBuilder("label");
            tagBuilder.MergeAttribute("for", id);
            tagBuilder.MergeAttribute("class", string.Concat("govuk-label govuk-checkboxes__label", additionalLabelClasses));
            tagBuilder.InnerHtml = labelText;

            return new HtmlString("<div class=\"govuk-checkboxes__item\">" + checkbox.ToHtmlString() + tagBuilder +"</div>" );
        }

        /// <summary>
        ///  Checkbox helper
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="inputName"></param>
        /// <param name="labelText"></param>
        /// <param name="additionalLabelClasses"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public static HtmlString GiasCheckbox(
            string inputValue,
            string inputName,
            string labelText,
            string additionalLabelClasses = "",
            object htmlAttributes = null,
            bool isChecked = false
        )
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var id = attributes["id"].ToString();

            var labelBuilder = new TagBuilder("label");

            labelBuilder.MergeAttribute("for", id);
            labelBuilder.MergeAttribute("class", string.Concat("govuk-label govuk-checkboxes__label", additionalLabelClasses));
            labelBuilder.InnerHtml = labelText;

            var checkbox = new TagBuilder("input");
            checkbox.Attributes["type"] =  "checkbox";
            checkbox.Attributes["name"] = inputName;
            checkbox.Attributes["value"] = inputValue;
            checkbox.Attributes["class"] = "govuk-checkboxes__input";

            if (isChecked)
            {
                checkbox.Attributes["checked"] = "checked";
            }

            if (htmlAttributes != null)
            {
                var viewDataDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var keyValue in viewDataDictionary)
                {
                    checkbox.Attributes[keyValue.Key] = (string) keyValue.Value;
                }
            }

            return new HtmlString("<div class=\"govuk-checkboxes__item\">" + checkbox + labelBuilder + "</div>");

        }
    }
}
