using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.MvcResult
{
    internal class ChallengeResult : IActionResult
    {
        private const string XsrfKey = "XsrfId";

        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }

        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }
        public string UserId { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Items[XsrfKey] = UserId;
            }

            if (string.IsNullOrWhiteSpace(LoginProvider))
            {
                await context.HttpContext.ChallengeAsync(properties);
                return;
            }

            await context.HttpContext.ChallengeAsync(LoginProvider, properties);
        }
    }
}
