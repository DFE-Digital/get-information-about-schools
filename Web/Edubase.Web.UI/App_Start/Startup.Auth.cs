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
        
        public void ConfigureSecureAccessAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider(),
                ExpireTimeSpan = ConfiguredExpireTimeSpan
            });
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseKentorAuthServicesAuthentication(CreateSecureAccessAuthServicesOptions());
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            
        }
        private static KentorAuthServicesAuthenticationOptions CreateSecureAccessAuthServicesOptions()
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
                Certificate = GetSecureAccessSPCertificate()
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

        private static X509Certificate2 GetSecureAccessSPCertificate()
        {
#if (DEBUG)
            return GetSecureAccessSPCertificateFromAppData();
#else
            return GetSPCertificateFromCertStore();
#endif
        }

        private static X509Certificate2 GetSecureAccessSPCertificateFromAppData() => new X509Certificate2(HostingEnvironment.MapPath("~/app_data/edubase3.pfx"), "testtest");

        private static X509Certificate2 GetSecureAccessSPCertificateFromCertStore()
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                var thumbprint = ConfigurationManager.AppSettings["ServiceProvider.Certificate.Thumbprint"];
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, ConfigurationManager.AppSettings["ServiceProvider.Certificate.Thumbprint"], false);
                var cert = certCollection.Cast<X509Certificate2>().FirstOrDefault();
                Guard.IsNotNull(cert, () => new Exception($"Service provider certificate could not be found by thumbprint [{thumbprint}]"));
                return cert;
            }
        }
        
        public void ConfigureSASimulatorAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: ConfiguredExpireTimeSpan,
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                ExpireTimeSpan = ConfiguredExpireTimeSpan
            });


            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseKentorAuthServicesAuthentication(CreateSASimulatorAuthServicesOptions());


            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

        }

        private static KentorAuthServicesAuthenticationOptions CreateSASimulatorAuthServicesOptions()
        {
            var spOptions = CreateSASimulatorSPOptions();
            var authServicesOptions = new KentorAuthServicesAuthenticationOptions(false)
            {
                SPOptions = spOptions
            };

            var idp = new IdentityProvider(new EntityId("http://secure-access-simulator.azurewebsites.net/Metadata"), spOptions)
            {
                AllowUnsolicitedAuthnResponse = true,
                Binding = Saml2BindingType.HttpRedirect,
                SingleSignOnServiceUrl = new Uri("http://secure-access-simulator.azurewebsites.net/")
            };

            idp.SigningKeys.AddConfiguredKey(
                new X509Certificate2(
                    HostingEnvironment.MapPath(
                        "~/App_Data/Kentor.AuthServices.StubIdp.cer")));

            authServicesOptions.IdentityProviders.Add(idp);

            // It's enough to just create the federation and associate it
            // with the options. The federation will load the metadata and
            // update the options with any identity providers found.
            new Federation("http://secure-access-simulator.azurewebsites.net/Federation", true, authServicesOptions);

            return authServicesOptions;
        }

        private static SPOptions CreateSASimulatorSPOptions()
        {
            var swedish = CultureInfo.GetCultureInfo("sv-se");

            var organization = new Organization();
            organization.Names.Add(new LocalizedName("Edubase", swedish));
            organization.DisplayNames.Add(new LocalizedName("Edubase", swedish));
            organization.Urls.Add(new LocalizedUri(new Uri("http://www.edubase.gov.uk"), swedish));

            var spOptions = new SPOptions
            {
                EntityId = new EntityId("http://edubase.gov"),
                Organization = organization
            };

            var techContact = new ContactPerson
            {
                Type = ContactType.Technical
            };
            techContact.EmailAddresses.Add("authservices@example.com");
            spOptions.Contacts.Add(techContact);

            var supportContact = new ContactPerson
            {
                Type = ContactType.Support
            };
            supportContact.EmailAddresses.Add("support@example.com");
            spOptions.Contacts.Add(supportContact);

            var attributeConsumingService = new AttributeConsumingService("AuthServices")
            {
                IsDefault = true,
            };

            attributeConsumingService.RequestedAttributes.Add(
                new RequestedAttribute("urn:someName")
                {
                    FriendlyName = "Some Name",
                    IsRequired = true,
                    NameFormat = RequestedAttribute.AttributeNameFormatUri
                });

            attributeConsumingService.RequestedAttributes.Add(
                new RequestedAttribute("Minimal"));

            spOptions.AttributeConsumingServices.Add(attributeConsumingService);
            
            return spOptions;
        }
        
    }
}
