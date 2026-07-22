using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Edubase.Web.UI.Filters
{
    public sealed class BasicAuthenticationGateMiddleware
    {
        private const string BasicAuthenticationScheme = "BasicAuthentication";

        private readonly RequestDelegate _next;

        public BasicAuthenticationGateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var result = await context.AuthenticateAsync(BasicAuthenticationScheme);

            if (!result.Succeeded)
            {
                await context.ChallengeAsync(BasicAuthenticationScheme);

                return;
            }

            await _next(context);
        }
    }
}
