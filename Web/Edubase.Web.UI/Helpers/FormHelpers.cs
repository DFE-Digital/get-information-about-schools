﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

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
        public static MvcHtmlString GiasRadioFor<TModel, TProperty>(
            this HtmlHelper<TModel> helper,
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
            var viewData = new ViewDataDictionary() { { "id", id } };

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
            tagBuilder.MergeAttribute("class", string.Concat("edubase-button-radio ", additionalLabelClasses));
            tagBuilder.InnerHtml = labelText;

            return new MvcHtmlString("<span class=\"radio-button-wrap\">" + radioButton.ToHtmlString() + tagBuilder + "</span>");
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
        public static MvcHtmlString GiasRadio(
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
            labelBuilder.MergeAttribute("class", string.Concat("edubase-button-radio ", additionalLabelClasses));
            labelBuilder.InnerHtml = labelText;

            var checkbox = new TagBuilder("input");
            checkbox.Attributes["type"] = "radio";
            checkbox.Attributes["name"] = inputName;
            checkbox.Attributes["value"] = inputValue;
            checkbox.Attributes["id"] = id;

            if (htmlAttributes != null)
            {
                var viewDataDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var keyValue in viewDataDictionary)
                {
                    checkbox.Attributes[keyValue.Key] = (string)keyValue.Value;
                }
            }

            return new MvcHtmlString("<span class=\"radio-button-wrap\">" + checkbox + labelBuilder + "</span>");
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
        public static MvcHtmlString GiasCheckboxFor<TModel>(
            this HtmlHelper<TModel> helper,
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
            tagBuilder.MergeAttribute("class", string.Concat("edubase-button-checkbox ", additionalLabelClasses));
            tagBuilder.InnerHtml = labelText;

            return new MvcHtmlString("<span class=\"checkbox-wrap\">" + checkbox.ToHtmlString() + tagBuilder + "</span>");
        }

        /// <summary>
        ///  Checkbox helper 
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="inputName"></param>
        /// <param name="labelText"></param>
        /// <param name="additionalLabelClasses"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString GiasCheckbox(
            string inputValue,
            string inputName,
            string labelText,
            string additionalLabelClasses = "",
            object htmlAttributes = null
        )
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var id = attributes["id"].ToString();
      
            var labelBuilder = new TagBuilder("label");

            labelBuilder.MergeAttribute("for", id);
            labelBuilder.MergeAttribute("class", string.Concat("edubase-button-checkbox ", additionalLabelClasses));
            labelBuilder.InnerHtml = labelText;

            var checkbox = new TagBuilder("input");
            checkbox.Attributes["type"] =  "checkbox";

            if (htmlAttributes != null)
            {
                var viewDataDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var keyValue in viewDataDictionary)
                {
                    checkbox.Attributes[keyValue.Key] = (string) keyValue.Value;
                }
            }

            return new MvcHtmlString("<span class=\"checkbox-wrap\">" + checkbox + labelBuilder + "</span>");

        }
    }
}


