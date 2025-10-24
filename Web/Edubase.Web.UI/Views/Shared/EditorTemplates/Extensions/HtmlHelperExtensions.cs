using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Views.Shared.EditorTemplates.Extensions;

public static class HtmlHelperExtensions
{
    public static IHtmlContent ProprietorTextBoxFor<TModel>(
        this IHtmlHelper htmlHelper,
        Expression<Func<TModel, string>> expression,
        object htmlAttributes)
    {
        var typedHelper = htmlHelper as IHtmlHelper<TModel>;
        return typedHelper.TextBoxFor(expression, htmlAttributes);
    }

    public static IHtmlContent ProprietorLabelFor<TModel>(
        this IHtmlHelper htmlHelper,
        Expression<Func<TModel, string>> expression,
        object htmlAttributes)
    {
        var typedHelper = htmlHelper as IHtmlHelper<TModel>;
        return typedHelper.LabelFor(expression, htmlAttributes);
    }

    public static IHtmlContent ProprietorValidationMessageFor<TModel>(
        this IHtmlHelper htmlHelper,
        Expression<Func<TModel, string>> expression,
        string message,
        object htmlAttributes)
    {
        var typedHelper = htmlHelper as IHtmlHelper<TModel>;
        return typedHelper.ValidationMessageFor(expression, message, htmlAttributes);
    }

    public static string ValidationCssClassFor<TModel, TValue>(
        this IHtmlHelper htmlHelper,
        Expression<Func<TModel, TValue>> expression)
    {
        var typedHelper = htmlHelper as IHtmlHelper<TModel>;
        var fieldName = ExpressionHelper.GetExpressionText(expression);
        var fullName = typedHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

        var modelState = typedHelper.ViewData.ModelState;
        if (modelState.ContainsKey(fullName) && modelState[fullName].Errors.Count > 0)
        {
            return "govuk-form-group--error";
        }

        return string.Empty;
    }

    public static string ProprietorsValidationCssClass(this IHtmlHelper htmlHelper, string validationId)
    {
        var modelState = htmlHelper.ViewData.ModelState;
        if (modelState.ContainsKey(validationId) && modelState[validationId].Errors.Count > 0)
        {
            return "govuk-form-group--error";
        }

        return string.Empty;
    }
}


