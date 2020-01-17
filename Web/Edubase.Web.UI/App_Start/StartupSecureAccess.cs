using System;
using System.Configuration;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using System.Web.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Owin;
using Sustainsys.Saml2.WebSso;

[assembly: OwinStartup("SecureAccessConfiguration", typeof(Edubase.Web.UI.StartupSecureAccess))]

namespace Edubase.Web.UI
{
    using static ConfigurationManager;

    public partial class StartupSecureAccess
    {
        public void Configuration(IAppBuilder app) => ConfigureAuth(app);

        public static TimeSpan ConfiguredExpireTimeSpan => TimeSpan.Parse(AppSettings["SessionExpireTimeSpan"]);
        public static string ApplicationIdpEntityId => AppSettings[nameof(ApplicationIdpEntityId)];
        public static Uri ExternalAuthDefaultCallbackUrl => new Uri(AppSettings[nameof(ExternalAuthDefaultCallbackUrl)]);
        public static Uri MetadataLocation => new Uri(AppSettings[nameof(MetadataLocation)]);

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider(),
                ExpireTimeSpan = ConfiguredExpireTimeSpan,
                CookieSecure = CookieSecureOption.Always
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseSaml2Authentication(CreateAuthServicesOptions());
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

        }
        private static Saml2AuthenticationOptions CreateAuthServicesOptions()
        {
            var spOptions = new SPOptions
            {
                EntityId = new EntityId(ApplicationIdpEntityId),
                ReturnUrl = ExternalAuthDefaultCallbackUrl,
                MinIncomingSigningAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1"
            };

            spOptions.ServiceCertificates.Add(new ServiceCertificate
            {
                Use = CertificateUse.Signing,
                Certificate = GetSPCertificateFromAppData()
            });

            var authServicesOptions = new Saml2AuthenticationOptions(false) { SPOptions = spOptions };

            var idp = new IdentityProvider(new EntityId(MetadataLocation.AbsoluteUri), spOptions)
            {
                AllowUnsolicitedAuthnResponse = true,
                Binding = Saml2BindingType.HttpRedirect,
                MetadataLocation = MetadataLocation.AbsoluteUri,
                DisableOutboundLogoutRequests = true
            };

            authServicesOptions.IdentityProviders.Add(idp);
            return authServicesOptions;
        }

        private static X509Certificate2 GetSPCertificateFromAppData() => new X509Certificate2(HostingEnvironment.MapPath($"~/app_data/wildcard-dfe.pfx"), "test", X509KeyStorageFlags.MachineKeySet);
    }
}
