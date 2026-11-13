using System;
using Edubase.Web.UI.Authentication;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sustainsys.Saml2.AspNetCore2;

namespace Edubase.Web.UI.App_Start
{
    public static class AuthenticationServiceRegistration
    {
        public static void AddApplicationAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy("CanManageAcademyOpeningsOrSecureAcademy16To19", policy =>
                    policy.RequireAuthenticatedUser().RequireRole(
                        string.Join(
                            ",", AuthorizedRoles.CanManageAcademyOpenings,
                            AuthorizedRoles.CanManageSecureAcademy16To19Openings).Split(",")))
                .AddPolicy("CanBulkAssociateEstabs2Groups", policy =>
                    policy.RequireAuthenticatedUser().RequireRole(AuthorizedRoles.CanBulkAssociateEstabs2Groups.Split(",")))
                .AddPolicy("CanBulkCreateFreeSchools", policy =>
                    policy.RequireAuthenticatedUser().RequireRole(AuthorizedRoles.CanBulkCreateFreeSchools.Split(",")))
                .AddPolicy("CanBulkUpdateEstablishments", policy =>
                    policy.RequireAuthenticatedUser().RequireRole(AuthorizedRoles.CanBulkUpdateEstablishments.Split(",")))
                .AddPolicy("EdubasePolicy", policy =>
                    policy.RequireAuthenticatedUser());

            var useSimulator = configuration.GetValue<bool>("AppSettings:UseSimulatorAuth");
            var useBasicAuth = configuration.GetValue<bool>("AppSettings:UseBasicAuth");

            /// <summary>
            /// Configures the authentication schemes for the application.
            /// Sets up the default cookie schemes and toggles between real and simulator SAML2 flows.
            /// </summary>
            var authBuilder = services.AddAuthentication(options =>
            {                
                options.DefaultScheme = "ApplicationCookie";                
                options.DefaultChallengeScheme = Saml2Defaults.Scheme;                
                options.DefaultForbidScheme = "ApplicationCookie";
            })
            .AddCookie("ApplicationCookie", options =>
            {
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);                
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddCookie("ExternalCookie", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddSingleton<IAuthorizationMiddlewareResultHandler, SamlAuthorizationResultHandler>();

            if (useBasicAuth)
            {
                authBuilder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", _ => { });
            }

            /// <summary>
            /// Registers either the simulator or real SAML2 authentication flow depending on the configuration flag.
            /// </summary>
            if (useSimulator)
            {
                authBuilder.AddSaml2SimulatorAuthenticationFlow(configuration);
            }
            else
            {
                authBuilder.AddSaml2AuthenticationFlow(configuration);
            }

        }
    }
}
