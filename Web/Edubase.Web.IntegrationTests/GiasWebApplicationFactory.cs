using System.Security.Claims;
using System.Text.Encodings.Web;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Edubase.Web.IntegrationTests;

public sealed class GiasWebApplicationFactory : WebApplicationFactory<Program>
{
    public GiasWebApplicationFactory()
    {
        ClientOptions.AllowAutoRedirect = false;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // TODO WEBROOT CONTENT ISN'T BEING SET WITH BELOW FAILING TO RESOLVE CSS AND SCRIPTS, CURRENTLY RESOLVES TO TESTPROJECT/BIN/DEBUG... need to fix WebRoot to point at Web output dir OR copy assets on build

        /*var solutionRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\.."));
        var webProjectPath = Path.Combine(solutionRoot, "Web", "Edubase.Web.UI", "bin", "Debug", "net8.0", "public");
        builder.UseWebRoot(webProjectPath);
        
        var T = builder.GetSetting(WebHostDefaults.WebRootKey);*/

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Point at Edubase Mock API
            config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "AppSettings:LookupApiBaseAddress", "https://localhost:8443/edubase/rest/" },
                    { "AppSettings:AzureMapsApiKey", "MAPS_API_KEY" },
                    { "AppSettings:AzureMapsUrl", "https://localhost:8443/search/address/json/" },
                });
        });

        builder.ConfigureServices((services) =>
        {
            // Add fake authentication
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            // Replace real policy handler with a fake one
            services.AddSingleton<IAuthorizationHandler, AllowAllEdubaseRequirementHandler>();

            // Disable AntiForgery token validation
            services.PostConfigure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
            {
                options.Filters.Remove(new Microsoft.AspNetCore.Mvc.ValidateAntiForgeryTokenAttribute());
            });
        });
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Role, AuthorizedRoles.CanManageAcademyOpenings)
            ],
            "Test");

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class AllowAllEdubaseRequirementHandler : AuthorizationHandler<EdubaseRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EdubaseRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
