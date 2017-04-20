using System;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Edubase.Web.UI.Models;
using Edubase.Data.Entity;
using Edubase.Data.Identity;
using Kentor.AuthServices.Owin;
using System.IdentityModel.Metadata;
using Kentor.AuthServices;
using System.Globalization;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Metadata;
using System.Security.Cryptography.X509Certificates;
using Kentor.AuthServices.WebSso;
using System.Web.Hosting;
using System.Web.Helpers;
using System.Security.Claims;
using System.Linq;
using MoreLinq;
using Edubase.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Edubase.Data.DbContext;

namespace Edubase.Web.UI
{
    public partial class Startup
    {
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
                ReturnUrl = AuthConfig.ExternalAuthDefaultCallbackUrl
            };

            var cert = new X509Certificate2(HostingEnvironment.MapPath("~/app_data/edubase3.pfx"), "testtest");
            spOptions.ServiceCertificates.Add(new ServiceCertificate
            {
                Use = CertificateUse.Signing,
                Certificate = cert
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
    }
}
