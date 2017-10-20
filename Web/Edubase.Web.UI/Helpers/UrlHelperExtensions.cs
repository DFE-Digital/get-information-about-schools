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
        public static MvcHtmlString Current(this UrlHelper helper, object substitutes, string fragment = null)
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
            
            uriBuilder.Query = query.ToString();
            
            return new MvcHtmlString(uriBuilder.Uri.MakeRelativeUri(uriBuilder.Uri).ToString() + fragment);
        }
        public static MvcHtmlString SortUrl(this UrlHelper helper, string sortKey, string fragment = null)
        {
            var request = helper.RequestContext.HttpContext.Request;
            var url = request.Url;
            var modifier = (request.QueryString["sortby"] ?? "").Contains($"{sortKey}-asc") ? $"{sortKey}-desc" : $"{sortKey}-asc";
            return Current(helper, new { sortby = modifier }, fragment);
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