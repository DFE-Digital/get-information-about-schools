using System;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Edubase.Web.UI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class EdubaseAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                // Authenticated but not authorized
                context.Result = new StatusCodeResult((int) HttpStatusCode.Forbidden);
            }
            else
            {
                var urlHelper = new UrlHelper(context);
                var redirectUrl = urlHelper.Action("ExternalLoginCallback", "Account", new
                {
                    ReturnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString
                });

                context.Result = new ChallengeResult("Saml2", new AuthenticationProperties
                {
                    RedirectUri = redirectUrl
                });
            }
        }
    }
}
