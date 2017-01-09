using System;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Common;

namespace Edubase.Web.UI.Helpers
{
    public static class UrlHelperExtensions
    {
        public static MvcHtmlString Current(this UrlHelper helper, object substitutes)
        {
            var url = helper.RequestContext.HttpContext.Request.Url;
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
            {
                var value = property.GetValue(substitutes)?.ToString()?.Clean();
                if (value == null) query.Remove(property.Name);
                else query[property.Name] = value;
            }

            uriBuilder.Query = query.ToString();
            
            return new MvcHtmlString(uriBuilder.Uri.MakeRelativeUri(uriBuilder.Uri).ToString());
        }

        public static MvcHtmlString CurrentQueryString(this UrlHelper helper, object substitutes = null)
        {
            var url = helper.RequestContext.HttpContext.Request.Url;
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (substitutes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
                {
                    var value = property.GetValue(substitutes)?.ToString()?.Clean();
                    if (value == null) query.Remove(property.Name);
                    else query[property.Name] = value;
                }
            }
            return new MvcHtmlString(query.ToString());
        }
    }
}