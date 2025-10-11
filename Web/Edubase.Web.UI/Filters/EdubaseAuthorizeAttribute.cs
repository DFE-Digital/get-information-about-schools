using Edubase.Web.UI.MvcResult;
using System;
using System.Net;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Edubase.Web.UI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class EdubaseAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            else
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                var redirectUrl = urlHelper.Action("ExternalLoginCallback", "Account", new
                {
                    // Making this "forwarded-header-aware" is not strictly required,
                    // but it's easier and safer to be consistent and just do it everywhere.
                    // - The `returnUrl` is currently ignored by the `AccountController.ExternalLoginCallback` method.
                    // - `PathAndQuery` is absolute, but relative to the host/domain part
                    //   (e.g., `new Uri("http://example.com/./abc/123/../567").PathAndQuery` returns /abc/567).
                    ReturnUrl = urlHelper.GetForwardedHeaderAwareUrl().PathAndQuery
                });

                filterContext.Result = new ChallengeResult("Saml2", redirectUrl);
            }
        }
    }
}
