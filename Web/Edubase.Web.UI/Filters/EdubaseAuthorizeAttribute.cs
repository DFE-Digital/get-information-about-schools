using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Edubase.Web.UI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class EdubaseAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var user = context.HttpContext.User;
            if (!(user?.Identity?.IsAuthenticated ?? false))
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                var redirectUrl = $"/Account/ExternalLoginCallback?ReturnUrl={Uri.EscapeDataString(returnUrl.ToString())}";
                context.Result = new ChallengeResult(new AuthenticationProperties { RedirectUri = redirectUrl });
                return;
            }

            if (!string.IsNullOrWhiteSpace(Roles))
            {
                var allowedRoles = Roles.Split(',').Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).ToHashSet();
                if (allowedRoles.Count > 0 && !allowedRoles.Any(user.IsInRole))
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
