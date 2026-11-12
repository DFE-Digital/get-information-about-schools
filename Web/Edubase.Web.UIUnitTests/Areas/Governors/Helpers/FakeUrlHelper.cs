using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Helpers
{
    internal sealed class FakeUrlHelper : IUrlHelper
    {
        public ActionContext ActionContext { get; set; } = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        public string Action(UrlActionContext actionContext) => "/fake-url";

        public string Content(string contentPath) => contentPath;

        public bool IsLocalUrl(string url) => !string.IsNullOrEmpty(url) && url.StartsWith("/", StringComparison.Ordinal);

        public string Link(string routeName, object values) => "/fake-url";

        public string RouteUrl(UrlRouteContext routeContext) => "/fake-url";
    }
}
