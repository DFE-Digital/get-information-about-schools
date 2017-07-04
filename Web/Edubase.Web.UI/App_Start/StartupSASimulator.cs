using Edubase.Data.Entity;
using Kentor.AuthServices;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Owin;
using Kentor.AuthServices.WebSso;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using System.Web.Hosting;

[assembly: OwinStartup("SASimulatorConfiguration", typeof(Edubase.Web.UI.StartupSASimulator))]

namespace Edubase.Web.UI
{
    public class StartupSASimulator
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
        public void ConfigureAuth(IAppBuilder app)
        {
            
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    //// Enables the application to validate the security stamp when the user logs in.
                    //// This is a security feature which is used when you change a password or add an external login to your account.  
                    //OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                    //    validateInterval: ConfiguredExpireTimeSpan,
                    //    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                ExpireTimeSpan = ConfiguredExpireTimeSpan
            });


            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseKentorAuthServicesAuthentication(CreateAuthServicesOptions());


            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

        }

        private static KentorAuthServicesAuthenticationOptions CreateAuthServicesOptions()
        {
            var spOptions = CreateSPOptions();
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

        private static SPOptions CreateSPOptions()
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