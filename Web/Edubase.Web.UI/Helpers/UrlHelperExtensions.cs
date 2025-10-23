using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Edubase.Web.UI.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;

namespace Edubase.Web.UI.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string CookieDomain(this IUrlHelper helper, IConfiguration configuration)
        {
            return "." + GetForwardedHeaderAwareUrl(helper, configuration).Host;
        }

        public static Uri GetForwardedHeaderAwareUrl(this IUrlHelper helper, IConfiguration configuration)
        {
            var request = helper.ActionContext.HttpContext.Request;
            var originalUrl = new Uri(request.GetDisplayUrl());
            var uriBuilder = new UriBuilder(originalUrl);

            var requestHeaderHost = request.Headers["X-Forwarded-Host"].FirstOrDefault();
            if (!string.IsNullOrEmpty(requestHeaderHost))
            {
                var allowedForwardedHostNames = (configuration["AllowedForwardedHostNames"] ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s));

                if (allowedForwardedHostNames.Contains(requestHeaderHost, StringComparer.OrdinalIgnoreCase))
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

        public static HtmlString Current(this IUrlHelper helper, object substitutes, IConfiguration configuration, string fragment = null)
        {
            var url = GetForwardedHeaderAwareUrl(helper, configuration);
            var uriBuilder = new UriBuilder(url);

            var queryDict = QueryHelpers.ParseQuery(uriBuilder.Query)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            if (substitutes != null)
            {
                foreach (var prop in substitutes.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var value = prop.GetValue(substitutes)?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(value))
                    {
                        queryDict.Remove(prop.Name);
                    }
                    else
                    {
                        queryDict[prop.Name] = value;
                    }
                }
            }

            uriBuilder.Query = string.Join("&", queryDict.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            var finalUrl = uriBuilder.Uri.ToString();
            if (!string.IsNullOrEmpty(fragment))
            {
                finalUrl += $"#{fragment}";
            }

            return new HtmlString(finalUrl);
        }

        public static HtmlString SortUrl(this IUrlHelper helper, string sortKey, string fragment = null)
        {
            var httpContext = helper.ActionContext.HttpContext;
            var query = httpContext.Request.Query;
            var currentSort = query["sortby"].ToString();
            var modifier = currentSort.Contains($"{sortKey}-asc") ? $"{sortKey}-desc" : $"{sortKey}-asc";

            var routeValues = new RouteValueDictionary(helper.ActionContext.RouteData.Values);
            foreach (var kvp in query)
            {
                routeValues[kvp.Key] = kvp.Value.ToString();
            }
            routeValues["sortby"] = modifier;

            var url = helper.RouteUrl(routeValues);
            if (!string.IsNullOrEmpty(fragment))
            {
                url += $"#{fragment}";
            }

            return new HtmlString(url);
        }

        public static HtmlString CurrentQueryString(this IUrlHelper helper, IConfiguration configuration, object substitutes = null)
        {
            var url = GetForwardedHeaderAwareUrl(helper, configuration);
            var uriBuilder = new UriBuilder(url);

            var queryDict = QueryHelpers.ParseQuery(uriBuilder.Query)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            if (substitutes != null)
            {
                foreach (var prop in substitutes.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var value = prop.GetValue(substitutes)?.ToString()?.Trim(); // Replace Clean() with Trim() or your own logic
                    if (string.IsNullOrEmpty(value))
                    {
                        queryDict.Remove(prop.Name);
                    }
                    else
                    {
                        queryDict[prop.Name] = value;
                    }
                }
            }

            var queryString = string.Join("&", queryDict.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            return new HtmlString(queryString);
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
