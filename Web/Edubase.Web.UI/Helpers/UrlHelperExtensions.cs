using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using Edubase.Common;
using Edubase.Web.UI.Exceptions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Edubase.Web.UI.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string CookieDomain(this IUrlHelper helper)
        {
            return string.Concat(".", GetForwardedHeaderAwareUrl(helper).Host);
        }

        public static Uri GetForwardedHeaderAwareUrl(this IUrlHelper helper)
        {
            var request = helper.ActionContext.HttpContext.Request;
            var uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port ?? -1,
                Path = request.PathBase.Add(request.Path).ToString(),
                Query = request.QueryString.HasValue ? request.QueryString.Value : string.Empty
            };

            var requestHeaderHost = request.Headers["X-Forwarded-Host"].ToString();
            if (!string.IsNullOrWhiteSpace(requestHeaderHost))
            {
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
                    throw new InvalidForwardedHostException($"The `X-Forwarded-Host` header contains an invalid value: {requestHeaderHost}");
                }
            }

            return uriBuilder.Uri;
        }

        public static HtmlString Current(this IUrlHelper helper, object substitutes, string fragment = null)
        {
            var request = helper.ActionContext.HttpContext.Request;
            var query = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var kv in QueryHelpers.ParseQuery(request.QueryString.Value ?? string.Empty))
            {
                query[kv.Key] = kv.Value.ToString();
            }

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

            var queryString = query.Count == 0
                ? string.Empty
                : "?" + string.Join("&", query.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value ?? string.Empty)}"));

            return new HtmlString($"{request.Path}{queryString}{fragment}");
        }

        public static HtmlString SortUrl(this IUrlHelper helper, string sortKey, string fragment = null)
        {
            var request = helper.ActionContext.HttpContext.Request;
            var sortBy = request.Query["sortby"].ToString();
            var modifier = sortBy.Contains($"{sortKey}-asc", StringComparison.OrdinalIgnoreCase) ? $"{sortKey}-desc" : $"{sortKey}-asc";
            return Current(helper, new { sortby = modifier }, fragment);
        }

        public static HtmlString CurrentQueryString(this IUrlHelper helper, object substitutes = null)
        {
            var request = helper.ActionContext.HttpContext.Request;
            var query = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var kv in QueryHelpers.ParseQuery(request.QueryString.Value ?? string.Empty))
            {
                query[kv.Key] = kv.Value.ToString();
            }

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

            var raw = string.Join("&", query.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value ?? string.Empty)}"));
            return new HtmlString(raw);
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
