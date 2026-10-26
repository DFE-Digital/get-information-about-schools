using System.Web.Mvc;
using System.Web.Routing;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Helpers
{
    internal sealed class FakeUrlHelper : UrlHelper
    {
        public FakeUrlHelper() : base(new RequestContext(new FakeHttpContext(null), new RouteData())) { }

        public override string RouteUrl(string routeName, object routeValues)
        {
            return "/fake-url";
        }
    }
}
