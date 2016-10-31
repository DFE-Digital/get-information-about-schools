using System.ComponentModel;
using System.Web.Mvc;
using System.Web.Routing;

namespace Edubase.Web.UI.Helpers
{
    public static class UrlHelperExtensions
    {
        public static MvcHtmlString Current(this UrlHelper helper, object substitutes)
        {
            var routeDataValues = new RouteValueDictionary(helper.RequestContext.RouteData.Values);

            var queryString = helper.RequestContext.HttpContext.Request.QueryString;

            foreach (string param in queryString)
                if (!string.IsNullOrEmpty(queryString[param]))
                    routeDataValues[param] = queryString[param];

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
            {
                var value = property.GetValue(substitutes);
                if (string.IsNullOrEmpty(value?.ToString())) routeDataValues.Remove(property.Name); else routeDataValues[property.Name] = value;
            }

            var url = helper.RouteUrl(routeDataValues);
            return new MvcHtmlString(url);
        }
    }
}