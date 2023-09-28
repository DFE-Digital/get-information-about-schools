using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Common;
using Edubase.Web.UI.Exceptions;
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

            var uriBuilder = new UriBuilder(originalUrl);

            // When accessing the app service directly this should normally be null (not provided).
            //  - When accessing the app service via proxy (e.g., Azure Front Door), the request header will be
            //    that of the app service (+- hostname forwarding) and the original hostname (as seen in the browser)
            //    will be provided within the `X-Forwarded-Host` header.
            // - Equally, however, a user accessing the app service directly might manually inject this header
            //   (e.g., via browser or Postman-like software) with a custom (potentially nefarious) value,
            //   therefore we need to do some additional validation of the supplied value.
            var requestHeaderHost = request.Headers["X-Forwarded-Host"];
            if(requestHeaderHost != null)
            {
                // Override the request's host only if the `X-Forwarded-Host` header is present and contains an acceptable value.
                // Notes:
                // - Configuration Manager will return `null` if the key is not found.
                // - `string.Empty.Split(',')` will return an array of length 1, with a single empty string element.
                // - The host should be trimmed of leading/trailing whitespace, to account for the configuration
                //   being "comma-plus-space-separated" instead of just "comma-separated" (plus makes it easier
                //   to filter out empty/whitespace-only strings e.g., if a trailing comma on the list).
                var allowedForwardedHostNames = (ConfigurationManager.AppSettings["AllowedForwardedHostNames"] ?? string.Empty)
                    .Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s));

                if (allowedForwardedHostNames.Contains(requestHeaderHost))
                {
                    uriBuilder.Host = requestHeaderHost;
                }
                else
                {
                    // Fail loudly if the `X-Forwarded-Host` header is present but contains an invalid value.
                    // Could signal a misconfiguration or an attempted malicious attack.
                    throw new InvalidForwardedHostException($"The `X-Forwarded-Host` header contains an invalid value: {requestHeaderHost}");
                }
            }

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
