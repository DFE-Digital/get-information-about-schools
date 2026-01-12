using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;

namespace Edubase.Web.UI.Authentication
{
    /// <summary>
    /// Provides extension methods for configuring SAML2 authentication schemes.
    /// Supports both real IdP configuration and simulator configuration.
    /// </summary>
    public static class Saml2CompositionRoot
    {
        /// <summary>
        /// Configuration key for the application's SAML Service Provider (SP) entity ID.
        /// </summary>
        private const string ApplicationIdpEntityIdKey = "AppSettings:ApplicationIdpEntityId";

        /// <summary>
        /// Configuration key for the default callback URL used after external authentication.
        /// </summary>
        private const string ExternalAuthCallbackUrlKey = "AppSettings:ExternalAuthDefaultCallbackUrl";

        /// <summary>
        /// Configuration key for the SAML Identity Provider (IdP) metadata location.
        /// </summary>
        private const string MetadataLocationKey = "AppSettings:MetadataLocation";

        /// <summary>
        /// Configuration key for the public origin override used by the SP.
        /// </summary>
        private const string PublicOriginKey = "AppSettings:PublicOrigin";

        /// <summary>
        /// Minimum accepted signing algorithm for incoming SAML messages.
        /// </summary>
        private const string RsaSha1AlgorithmUri = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

        /// <summary>
        /// Configures the real SAML2 authentication scheme using application settings.
        /// Sets up the Service Provider (SP), Identity Provider (IdP), and required bindings.
        /// </summary>
        /// <param name="builder">The authentication builder to extend.</param>
        /// <param name="configuration">The application configuration containing IdP and SP settings.</param>
        /// <returns>The updated <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddSaml2AuthenticationFlow(
            this AuthenticationBuilder builder,
            IConfiguration configuration)
        {
            return builder.AddSaml2(options =>
            {
                // Configure the SP entity ID (unique identifier for this application).
                options.SPOptions.EntityId =
                    new EntityId(configuration[ApplicationIdpEntityIdKey]);

                // Configure the default callback URL after external authentication.
                options.SPOptions.ReturnUrl =
                    new Uri(configuration[ExternalAuthCallbackUrlKey]);

                // Enforce minimum signing algorithm for incoming SAML assertions.
                options.SPOptions.MinIncomingSigningAlgorithm = RsaSha1AlgorithmUri;

                // Create and configure the Identity Provider (IdP) using metadata.
                IdentityProvider idp =
                    new IdentityProvider(
                        new EntityId(configuration[MetadataLocationKey]), options.SPOptions)
                    {
                        AllowUnsolicitedAuthnResponse = true,                   // Allow IdP-initiated SSO.
                        Binding = Saml2BindingType.HttpRedirect,                // Use Redirect binding for requests.
                        MetadataLocation = configuration[MetadataLocationKey],  // URL to IdP metadata.
                        DisableOutboundLogoutRequests = true                    // Disable SP-initiated logout.
                    };

                // Register the IdP with the SAML2 options.
                options.IdentityProviders.Add(idp);

                // Optionally configure the public origin (useful behind proxies/load balancers).
                string? publicOrigin = configuration[PublicOriginKey];

                if (!string.IsNullOrWhiteSpace(publicOrigin))
                {
                    options.SPOptions.PublicOrigin = new Uri(publicOrigin);
                }
            });
        }
    }
}
