using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Helpers
{
    internal sealed class FakeUrlHelper : UrlHelper
    {
        public FakeUrlHelper()
            : base(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor()))
        {
        }

        public override string RouteUrl(UrlRouteContext routeContext)
        {
            return "/fake-url";
        }
    }
}
