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
            return string.Concat(".", GetForwardedHeaderAwareUrl(helper).Host);
        }

        public static Uri GetForwardedHeaderAwareUrl(this UrlHelper helper)
        {
            var request = helper.RequestContext.HttpContext.Request;
            var originalUrl = request.Url;

            if (originalUrl is null)
            {
                return null;
            }

            var uriBuilder = new UriBuilder(originalUrl)
            {
                // TODO: Setup allow-list of permitted x-forwarded-host values
                Host = request.Headers["X-Forwarded-Host"] ?? originalUrl.Host
            };

            return uriBuilder.Uri;
        }

        public static MvcHtmlString Current(this UrlHelper helper, object substitutes, string fragment = null)
        {
            var url = GetForwardedHeaderAwareUrl(helper);
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
            var modifier = (request.QueryString["sortby"] ?? "").Contains($"{sortKey}-asc") ? $"{sortKey}-desc" : $"{sortKey}-asc";
            return Current(helper, new { sortby = modifier }, fragment);
        }

        public static MvcHtmlString CurrentQueryString(this UrlHelper helper, object substitutes = null)
        {
            // Making this "forwarded-header-aware" is not strictly required,
            // but it's easier and safer to be consistent and just do it everywhere.
            var url = GetForwardedHeaderAwareUrl(helper);
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
    }
}
