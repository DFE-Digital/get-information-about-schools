using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Helpers
{
    internal static class FakeHttpContext
    {
        public static DefaultHttpContext Create(IPrincipal user = null, NameValueCollection qs = null)
        {
            var context = new DefaultHttpContext();

            if (user != null)
            {
                context.User = user is ClaimsPrincipal cp ? cp : new ClaimsPrincipal(user.Identity);
            }

            if (qs != null)
            {
                var queryString = qs.Count > 0
                    ? "?" + string.Join("&", qs.AllKeys.Where(k => !string.IsNullOrEmpty(k)).Select(k => Uri.EscapeDataString(k) + "=" + Uri.EscapeDataString(qs[k])))
                    : string.Empty;
                context.Request.QueryString = new QueryString(queryString);
            }

            return context;
        }
    }
}
