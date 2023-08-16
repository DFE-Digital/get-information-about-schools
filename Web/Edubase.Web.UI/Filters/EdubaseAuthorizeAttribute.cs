using Edubase.Web.UI.MvcResult;
using System;
using System.Net;
using System.Web.Mvc;

namespace Edubase.Web.UI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class EdubaseAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            else
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                var redirectUrl = urlHelper.Action("ExternalLoginCallback", "Account", new
                {
                    // TODO: Check to see if we can make this call to `Request.Url` relative
                    ReturnUrl = filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery
                });

                filterContext.Result = new ChallengeResult("Saml2", redirectUrl);
            }
        }
    }
}
