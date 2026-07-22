using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Edubase.Services.Texuna.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Edubase.Web.UI.Filters
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration configuration;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IConfiguration configuration) : base(options, logger, encoder)
        {
            this.configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]!);

                if (!string.Equals(authHeader.Scheme, "Basic", StringComparison.OrdinalIgnoreCase))
                {
                    return AuthenticateResult.NoResult();
                }

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                var basicAuthCredentials = configuration["AppSettings:BasicAuthCredentials"].Split(':', 2);

                var username = basicAuthCredentials[0];
                var password = basicAuthCredentials[1];

                if (credentials[0] == username && credentials[1] == password)
                {
                    var claims = new[] {
                        new Claim(ClaimTypes.NameIdentifier, credentials[0]),
                        new Claim(ClaimTypes.Name, credentials[0]),
                    };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }

                return AuthenticateResult.Fail("Invalid Username or Password");
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header Format");
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized; // Unauthorized

            // This specific header forces web browsers to show the native login prompt
            Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Secure Website\"");

            await Response.WriteAsync("Unauthorized Access.");
        }
    }

}
