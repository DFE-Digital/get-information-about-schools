using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Edubase.Common;
using Edubase.Web.UI.Exceptions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Edubase.Web.UI.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string CookieDomain(this IUrlHelper helper)
        {
            return "." + GetForwardedHeaderAwareUrl(helper).Host;
        }

        public static Uri GetForwardedHeaderAwareUrl(this IUrlHelper helper)
        {
            var request = helper.ActionContext.HttpContext.Request;
            var originalUrl = new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}");

            var uriBuilder = new UriBuilder(originalUrl);

            var requestHeaderHost = request.Headers["X-Forwarded-Host"].FirstOrDefault();
            if (!string.IsNullOrEmpty(requestHeaderHost))
            {
                var config = helper.ActionContext.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
                var allowedForwardedHostNames = (config?["AllowedForwardedHostNames"] ?? string.Empty)
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
            var url = GetForwardedHeaderAwareUrl(helper);
            var uriBuilder = new UriBuilder(url);
            var query = QueryHelpers.ParseQuery(uriBuilder.Query);
            var queryDict = query.ToDictionary(k => k.Key, v => v.Value.ToString());

            if (substitutes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
                {
                    var value = property.GetValue(substitutes)?.ToString()?.Clean();
                    if (value == null)
                    {
                        queryDict.Remove(property.Name);
                    }
                    else
                    {
                        queryDict[property.Name] = value;
                    }
                }
            }

            uriBuilder.Query = string.Join("&", queryDict.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            return new HtmlString(uriBuilder.Uri.PathAndQuery + fragment);
        }

        public static HtmlString SortUrl(this IUrlHelper helper, string sortKey, string fragment = null)
        {
            var request = helper.ActionContext.HttpContext.Request;
            var query = request.Query["sortby"].ToString();
            var modifier = query.Contains($"{sortKey}-asc") ? $"{sortKey}-desc" : $"{sortKey}-asc";
            return Current(helper, new { sortby = modifier }, fragment);
        }

        public static HtmlString CurrentQueryString(this IUrlHelper helper, object substitutes = null)
        {
            var url = GetForwardedHeaderAwareUrl(helper);
            var uriBuilder = new UriBuilder(url);
            var query = QueryHelpers.ParseQuery(uriBuilder.Query);
            var queryDict = query.ToDictionary(k => k.Key, v => v.Value.ToString());

            if (substitutes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
                {
                    var value = property.GetValue(substitutes)?.ToString()?.Clean();
                    if (value == null)
                    {
                        queryDict.Remove(property.Name);
                    }
                    else
                    {
                        queryDict[property.Name] = value;
                    }
                }
            }

            var queryString = string.Join("&", queryDict.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            return new HtmlString(queryString);
        }

        public static string ToQueryString(this IDictionary<string, string[]> nvc)
        {
            if (nvc == null || !nvc.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var kvp in nvc)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key) || kvp.Value == null) continue;

                foreach (var value in kvp.Value)
                {
                    sb.Append(sb.Length == 0 ? "" : "&");
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(value));
                }
            }

            return sb.ToString();
        }
    }
}
