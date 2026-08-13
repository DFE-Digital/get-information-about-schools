using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sustainsys.Saml2.AspNetCore2;

namespace Edubase.Web.UI.Authentication
{
    public sealed class SamlAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();

                var returnUrl = context.Request.GetEncodedPathAndQuery();

                var redirectUrl = linkGenerator.GetPathByAction(context, action: "ExternalLoginCallback", controller: "Account", values: new { returnUrl });

                await context.ChallengeAsync(Saml2Defaults.Scheme, new AuthenticationProperties { RedirectUri = redirectUrl });

                return;
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
