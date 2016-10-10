using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

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
            if (state == null) return MvcHtmlString.Empty;
            else if (state.Errors.Count == 0) return MvcHtmlString.Empty;
            else return new MvcHtmlString("error");
        }


        private static Lazy<EstablishmentPermission[]> _permissions = new Lazy<EstablishmentPermission[]>(() =>
        {
            var permissions = MemoryCache.Default.Get("permissions") as EstablishmentPermission[];
            if (permissions == null)
            {
                using (var dc = new ApplicationDbContext()) permissions = dc.Permissions.ToArray();
                MemoryCache.Default.Set("permissions", permissions, DateTimeOffset.UtcNow.AddMinutes(10));
            }
            return permissions;
        });

        public static bool CanUpdateFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var role = AuthHelper.GetRole(); 
            var permission = _permissions.Value.FirstOrDefault(x => x.PropertyName == name && x.RoleName == role);
            return permission?.AllowUpdate ?? true;
        }

        public static bool CanUpdate<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            string name)
        {
            var role = AuthHelper.GetRole();
            var permission = _permissions.Value.FirstOrDefault(x => x.PropertyName == name && x.RoleName == role);
            return permission?.AllowUpdate ?? true;
        }

        public static bool CanApproveFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var role = AuthHelper.GetRole();
            var permission = _permissions.Value.FirstOrDefault(x => x.PropertyName == name && x.RoleName == role);
            return permission?.AllowApproval ?? true;
        }

        public static MvcHtmlString EduTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object attributes, string fieldName = null)
        {
            var canUpdate = fieldName == null ? CanUpdateFor(htmlHelper, expression) : CanUpdate(htmlHelper, fieldName);
            var result = htmlHelper.TextBoxFor(expression, SetAttributes(!canUpdate, attributes));
            var valMsg = htmlHelper.ValidationMessageFor(expression);
            if (valMsg != null) result = new MvcHtmlString(result.ToHtmlString() + valMsg.ToHtmlString());
            if (!canUpdate)
            {
                result = new MvcHtmlString(result.ToHtmlString() + htmlHelper.HiddenFor(expression).ToHtmlString());
            }
            return result;
        }

        public static MvcHtmlString EduDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, 
            IEnumerable<SelectListItem> selectList, object attributes = null, string fieldName = null, string label = null)
        {
            var canUpdate = fieldName == null ? CanUpdateFor(htmlHelper, expression) : CanUpdate(htmlHelper, fieldName);
            var result = htmlHelper.DropDownListFor(expression, selectList, label ?? string.Empty, SetAttributes(!canUpdate, attributes));
            var valMsg = htmlHelper.ValidationMessageFor(expression);
            if (valMsg != null) result = new MvcHtmlString(result.ToHtmlString() + valMsg.ToHtmlString());
            if (!canUpdate)
            {
                result = new MvcHtmlString(result.ToHtmlString() + htmlHelper.HiddenFor(expression).ToHtmlString());
            }
            return result;
        }

        //public static MvcHtmlString EduDropDownFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, 
        //    IEnumerable<SelectListItem> selectList, object attributes = null, Expression<Func<TModel, TProperty>> permissionExpression = null, string label = null)
        //{
        //    string fieldName = null;
        //    if (permissionExpression != null) fieldName = ExpressionHelper.GetExpressionText(permissionExpression);
        //    return EduDropDownFor(htmlHelper, expression, selectList, attributes, fieldName, label);
        //}



        public static MvcHtmlString PendingUpdateFor<TProperty>(this HtmlHelper<EstablishmentDetailViewModel> htmlHelper, 
            Expression<Func<Establishment, TProperty>> expression)
        {
            var model = htmlHelper.ViewData.Model;
            if (model.IsUserLoggedOn && model.PendingChanges.Any(x => x.DataField.Equals(ExpressionHelper.GetExpressionText(expression))))
            {
                return new MvcHtmlString("<div class=\"pending-update-icon\" title=\"Pending update\">!</div>");
            }
            else return null;
        }

        public static MvcHtmlString PendingUpdatesFor(this HtmlHelper<EstablishmentDetailViewModel> htmlHelper,
            params Expression<Func<Establishment, object>>[] expressions)
        {
            var model = htmlHelper.ViewData.Model;
            if (model.IsUserLoggedOn)
                if (expressions.Any(x => model.PendingChanges.Any(p => p.DataField.Equals(ExpressionHelper.GetExpressionText(x)))))
                    return new MvcHtmlString("<div class=\"pending-update-icon\" title=\"Pending update\">!</div>");
            return null;
        }

        public static MvcHtmlString PendingUpdatesFor(this HtmlHelper<EstablishmentDetailViewModel> htmlHelper, params string[] fields)
        {
            var model = htmlHelper.ViewData.Model;
            if (model.IsUserLoggedOn)
                if (fields.Any(x => model.PendingChanges.Any(p => p.DataField.Equals(x))))
                    return new MvcHtmlString("<div class=\"pending-update-icon\" title=\"Pending update\">!</div>");
            return null;
        }


        private static Dictionary<string, object> SetAttributes(bool isDisabled, object otherAttributes = null)
        {
            var d = new Dictionary<string, object>();
            d["class"] = "form-control";
            if (isDisabled) d["disabled"] = "disabled";

            if (otherAttributes != null)
            {
                var items = ReflectionHelper.GetProperties(otherAttributes);
                foreach (var item in items)
                {
                    d[item] = ReflectionHelper.GetProperty(otherAttributes, item);
                }
            }

            if (isDisabled) d["style"] = d.ContainsKey("style") ? (d["style"].ToString() + ";background-color:#eee;") : "background-color:#eee";
            return d;
        }
        
    }
}