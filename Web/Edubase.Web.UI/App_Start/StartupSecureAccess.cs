﻿using Edubase.Common;
using Kentor.AuthServices;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Owin;
using Kentor.AuthServices.WebSso;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using System.Web.Hosting;

[assembly: OwinStartup("SecureAccessConfiguration", typeof(Edubase.Web.UI.StartupSecureAccess))]

namespace Edubase.Web.UI
{
    public partial class StartupSecureAccess
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        private static TimeSpan ConfiguredExpireTimeSpan
        {
            get
            {
                var configuredValue = ConfigurationManager.AppSettings["SessionExpireTimeSpan"];
                return configuredValue != null
                    ? TimeSpan.Parse(configuredValue)
                    : TimeSpan.FromMinutes(60);
            }
        }

        private static string SaCertificate
        {
            get
            {
                var configuredValue = ConfigurationManager.AppSettings["SaCertificate"];
                return configuredValue ?? "wildcard-dfe.pfx";
            }
        }

        private static string SaCertificatePassword
        {
            get
            {
                var configuredValue = ConfigurationManager.AppSettings["SaCertificatePassword"];
                return configuredValue ?? "test";
            }
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider(),
                ExpireTimeSpan = ConfiguredExpireTimeSpan
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseKentorAuthServicesAuthentication(CreateAuthServicesOptions());
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

        }
        private static KentorAuthServicesAuthenticationOptions CreateAuthServicesOptions()
        {
            var spOptions = new SPOptions
            {
                EntityId = new EntityId(AuthConfig.ApplicationIdpEntityId),
                ReturnUrl = AuthConfig.ExternalAuthDefaultCallbackUrl,
                AuthenticateRequestSigningBehavior = SigningBehavior.Always
            };

            spOptions.ServiceCertificates.Add(new ServiceCertificate
            {
                Use = CertificateUse.Signing,
                Certificate = GetSPCertificateFromAppData()
            });

            var authServicesOptions = new KentorAuthServicesAuthenticationOptions(false) { SPOptions = spOptions };

            var idp = new IdentityProvider(new EntityId(AuthConfig.ExternalIdpEntityId.AbsoluteUri), spOptions)
            {
                AllowUnsolicitedAuthnResponse = true,
                Binding = Saml2BindingType.HttpRedirect,
                MetadataLocation = AuthConfig.ExternalIdpMetadataPath,
                WantAuthnRequestsSigned = true
            };

            idp.SigningKeys.AddConfiguredKey(new X509Certificate2(AuthConfig.ExternalIdpCertificatePath));
            authServicesOptions.IdentityProviders.Add(idp);
            return authServicesOptions;
        }

        private static X509Certificate2 GetSPCertificateFromAppData() => new X509Certificate2(HostingEnvironment.MapPath($"~/app_data/{SaCertificate}"), SaCertificatePassword, X509KeyStorageFlags.MachineKeySet);
    }
}
