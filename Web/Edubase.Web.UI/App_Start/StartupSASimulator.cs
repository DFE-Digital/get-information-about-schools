using System;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Security.Claims;
using System.Web.Helpers;
using Kentor.AuthServices;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Owin;
using Kentor.AuthServices.WebSso;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup("SASimulatorConfiguration", typeof(Edubase.Web.UI.StartupSASimulator))]

namespace Edubase.Web.UI
{
    public class StartupSASimulator
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider(),
                ExpireTimeSpan = StartupSecureAccess.ConfiguredExpireTimeSpan,
                CookieSecure = CookieSecureOption.Always
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

            var idp = new IdentityProvider(new EntityId("http://dfe-sign-in-simulator.azurewebsites.net/Metadata"), spOptions)
            {
                AllowUnsolicitedAuthnResponse = true,
                Binding = Saml2BindingType.HttpRedirect,
                SingleSignOnServiceUrl = new Uri("http://dfe-sign-in-simulator.azurewebsites.net/")
            };

            //idp.SigningKeys.AddConfiguredKey(new X509Certificate2(HostingEnvironment.MapPath("~/App_Data/Kentor.AuthServices.StubIdp.cer")));
            authServicesOptions.IdentityProviders.Add(idp);
            new Federation("http://dfe-sign-in-simulator.azurewebsites.net/Federation", true, authServicesOptions);
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
                Organization = organization,
                ReturnUrl = StartupSecureAccess.ExternalAuthDefaultCallbackUrl
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
