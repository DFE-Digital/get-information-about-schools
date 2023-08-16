using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Common;
using Glimpse.AspNet.Tab;

namespace Edubase.Web.UI.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string CookieDomain(this UrlHelper helper)
        {
            return string.Concat(".", GetForwardedHostAwareHost(helper));
        }

        public static string GetForwardedHostAwareHost(this UrlHelper helper)
        {
            // TODO: Setup allow-list of permitted x-forwarded-host values
            var request = helper.RequestContext.HttpContext.Request;
            var host = request.Headers["X-Forwarded-Host"] ?? request.Url.Host;
            return host;
        }

        public static MvcHtmlString Current(this UrlHelper helper, object substitutes, string fragment = null)
        {
            // TODO: Make reference to `Request.Url` relative (or x-forwarded-host aware)
            var url = helper.RequestContext.HttpContext.Request.Url;
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (substitutes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
                {
                    var value = property.GetValue(substitutes)?.ToString()?.Clean();
                    if (value == null)
                    {
                        query.Remove(property.Name);
                    }
                    else
                    {
                        query[property.Name] = value;
                    }
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
                    if (value == null)
                    {
                        query.Remove(property.Name);
                    }
                    else
                    {
                        query[property.Name] = value;
                    }
                }
            }
            return new MvcHtmlString(query.ToString());
        }

        public static string ToQueryString(this NameValueCollection nvc)
        {
            if (nvc == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (string key in nvc.Keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                string[] values = nvc.GetValues(key);
                if (values == null)
                {
                    continue;
                }

                foreach (string value in values)
                {
                    sb.Append(sb.Length == 0 ? "" : "&");
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
                }
            }

            return sb.ToString();
        }

        public static string AbsoluteActionUrl(
            this UrlHelper urlHelper,
            string actionName,
            string controllerName,
            object routeValues = null)
        {
            var scheme = urlHelper.RequestContext.HttpContext.Request.Url.Scheme;
            return urlHelper.Action(actionName, controllerName, routeValues, scheme);
        }
    }
}
