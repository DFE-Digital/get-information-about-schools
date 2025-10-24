//using System;
//using System.Configuration;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Security.Cryptography.X509Certificates;
//using System.Web.Hosting;
//using Sustainsys.Saml2;
//using Sustainsys.Saml2.Configuration;
//using Sustainsys.Saml2.Metadata;
//using Sustainsys.Saml2.Owin;
//using Sustainsys.Saml2.WebSso;

//[assembly: OwinStartup("SecureAccessConfiguration", typeof(Edubase.Web.UI.StartupSecureAccess))]

//namespace Edubase.Web.UI
//{
//    using static ConfigurationManager;

//    public partial class StartupSecureAccess
//    {
//        public void Configuration(IAppBuilder app) => ConfigureAuth(app);

//        public static TimeSpan ConfiguredExpireTimeSpan => TimeSpan.Parse(AppSettings["SessionExpireTimeSpan"]);
//        public static string ApplicationIdpEntityId => AppSettings[nameof(ApplicationIdpEntityId)];
//        public static Uri ExternalAuthDefaultCallbackUrl => new Uri(AppSettings[nameof(ExternalAuthDefaultCallbackUrl)]);
//        public static Uri MetadataLocation => new Uri(AppSettings[nameof(MetadataLocation)]);
//        public static Uri PublicOrigin => string.IsNullOrWhiteSpace(AppSettings[nameof(PublicOrigin)]) ? null : new Uri(AppSettings[nameof(PublicOrigin)]);

//        public void ConfigureAuth(IAppBuilder app)
//        {
//            app.UseCookieAuthentication(new CookieAuthenticationOptions
//            {
//                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
//                LoginPath = new PathString("/Account/Login"),
//                Provider = new CookieAuthenticationProvider(),
//                ExpireTimeSpan = ConfiguredExpireTimeSpan,
//                CookieSecure = CookieSecureOption.Always,
//                CookieSameSite = SameSiteMode.Lax
//            });

//            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
//            // -- Replaced with internal logic of UseExternalSignInCookie as the default CookieAuthenticationOptions do not include CookieSameSite value
//            app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ExternalCookie);
//            app.UseCookieAuthentication(new CookieAuthenticationOptions
//            {
//                AuthenticationMode = AuthenticationMode.Passive,
//                AuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
//                ExpireTimeSpan = TimeSpan.FromMinutes(5.0),
//                CookieSecure = CookieSecureOption.Always,
//                CookieSameSite = SameSiteMode.Lax
//            });

//            app.UseSaml2Authentication(CreateAuthServicesOptions());
//            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

//            app.Use((context, next) =>
//            {
//                var rng = new RNGCryptoServiceProvider();
//                var nonceBytes = new byte[32];
//                rng.GetBytes(nonceBytes);
//                var nonce = Convert.ToBase64String(nonceBytes);
//                context.Set("ScriptNonce", nonce);

//                var securityPolicy = AppSettings["Content-Security-Policy"];
//                securityPolicy = securityPolicy.Replace("nonce-inject", $"nonce-{nonce}");

//                context.Response.Headers.Append("Content-Security-Policy", securityPolicy);
//                context.Response.Headers.Append("X-Content-Security-Policy", securityPolicy);
//                return next();
//            });

//        }
//        private static Saml2AuthenticationOptions CreateAuthServicesOptions()
//        {
//            var spOptions = new SPOptions
//            {
//                EntityId = new EntityId(ApplicationIdpEntityId),
//                ReturnUrl = ExternalAuthDefaultCallbackUrl,
//                MinIncomingSigningAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1"
//            };

//            if (PublicOrigin != null)
//            {
//                // Only set if provided via configuration
//                spOptions.PublicOrigin = PublicOrigin;
//            }

//            var authServicesOptions = new Saml2AuthenticationOptions(false) { SPOptions = spOptions };

//            var idp = new IdentityProvider(new EntityId(MetadataLocation.AbsoluteUri), spOptions)
//            {
//                AllowUnsolicitedAuthnResponse = true,
//                Binding = Saml2BindingType.HttpRedirect,
//                MetadataLocation = MetadataLocation.AbsoluteUri,
//                DisableOutboundLogoutRequests = true
//            };

//            authServicesOptions.IdentityProviders.Add(idp);
//            return authServicesOptions;
//        }
//    }
//}
