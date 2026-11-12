using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace System.Web
{
    public abstract class HttpRequestBase
    {
        public virtual NameValueCollection QueryString => new NameValueCollection();
        public virtual bool IsAuthenticated => false;
    }

    public abstract class HttpContextBase
    {
        public virtual HttpRequestBase Request => null;
        public virtual IPrincipal User => null;
    }
}

namespace System.Web.Routing
{
    public class RouteData
    {
    }

    public class RouteValueDictionary : Dictionary<string, object>
    {
        public RouteValueDictionary()
        {
        }

        public RouteValueDictionary(object values)
        {
            if (values == null)
            {
                return;
            }

            foreach (var property in values.GetType().GetProperties())
            {
                this[property.Name] = property.GetValue(values);
            }
        }
    }
}

namespace System.Web.Mvc
{
    public class ControllerContext : Microsoft.AspNetCore.Mvc.ControllerContext
    {
        public ControllerContext(System.Web.HttpContextBase context, System.Web.Routing.RouteData routeData, Controller controller)
        {
            HttpContext = BuildHttpContext(context);
            RouteData = new Microsoft.AspNetCore.Routing.RouteData();
            ActionDescriptor = new ControllerActionDescriptor();
        }

        private static HttpContext BuildHttpContext(System.Web.HttpContextBase context)
        {
            var core = new DefaultHttpContext();

            var user = context?.User;
            if (user is ClaimsPrincipal claimsPrincipal)
            {
                core.User = claimsPrincipal;
            }
            else if (user != null)
            {
                core.User = new PrincipalAdapter(user);
            }

            var query = context?.Request?.QueryString;
            if (query != null && query.Count > 0)
            {
                var parts = new List<string>();
                foreach (var key in query.AllKeys.Where(k => !string.IsNullOrEmpty(k)))
                {
                    foreach (var value in query.GetValues(key) ?? Array.Empty<string>())
                    {
                        parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value ?? string.Empty)}");
                    }
                }

                core.Request.QueryString = parts.Count > 0 ? new QueryString("?" + string.Join("&", parts)) : QueryString.Empty;
            }

            return core;
        }

        private sealed class PrincipalAdapter : ClaimsPrincipal
        {
            private readonly IPrincipal _principal;

            public PrincipalAdapter(IPrincipal principal)
                : base(principal.Identity != null ? new ClaimsIdentity(principal.Identity) : new ClaimsIdentity("Compat"))
            {
                _principal = principal;
            }

            public override bool IsInRole(string role) => _principal.IsInRole(role);
        }
    }

    public class UrlHelper : IUrlHelper
    {
        public virtual ActionContext ActionContext { get; } = new ActionContext();

        public virtual string Action(UrlActionContext actionContext) => string.Empty;

        public virtual string Action(string actionName, string controllerName, object routeValues)
            => string.Empty;

        public virtual string Content(string contentPath) => contentPath;

        public virtual bool IsLocalUrl(string url) => true;

        public virtual string Link(string routeName, object values) => RouteUrl(new System.Web.Routing.RouteValueDictionary(values));

        public string RouteUrl(UrlRouteContext routeContext)
        {
            if (routeContext?.Values is System.Web.Routing.RouteValueDictionary legacyValues)
            {
                return RouteUrl(legacyValues);
            }

            if (routeContext?.Values is IEnumerable<KeyValuePair<string, object>> coreValues)
            {
                return string.Join("|", coreValues.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }

            return string.Empty;
        }

        public virtual string RouteUrl(System.Web.Routing.RouteValueDictionary values)
        {
            if (values == null || values.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("|", values.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }
    }
}
