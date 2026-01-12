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
    public static class Saml2SimulatorCompositionRoot
    {
        /// <summary>
        /// Configuration key for the default callback URL used after external authentication.
        /// </summary>
        private const string ExternalAuthCallbackUrlKey = "AppSettings:ExternalAuthDefaultCallbackUrl";

        /// <summary>
        /// Configuration key for the simulator GUID used to construct the IdP endpoint.
        /// </summary>
        private const string SimulatorGuidKey = "AppSettings:SASimulatorGuid";

        /// <summary>
        /// Configuration key for the base simulator URI.
        /// </summary>
        private const string SimulatorUriKey = "AppSettings:SASimulatorUri";

        /// <summary>
        /// Configuration key for the public origin override used by the SP.
        /// </summary>
        private const string PublicOriginKey = "AppSettings:PublicOrigin";

        /// <summary>
        /// Constant Service Provider (SP) entity ID used by the simulator.
        /// </summary>
        private const string SimulatorEntityId = "http://edubase.gov";

        /// <summary>
        /// Minimum accepted signing algorithm for incoming SAML messages.
        /// </summary>
        private const string RsaSha1AlgorithmUri = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

        /// <summary>
        /// Configures the simulator SAML2 authentication scheme using application settings.
        /// Sets up the Service Provider (SP), Identity Provider (IdP), and federation metadata.
        /// </summary>
        /// <param name="builder">The authentication builder to extend.</param>
        /// <param name="config">The application configuration containing simulator settings.</param>
        /// <returns>The updated <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddSaml2SimulatorAuthenticationFlow(
            this AuthenticationBuilder builder, IConfiguration config)
        {
            return builder.AddSaml2(options =>
            {
                // Configure the SP entity ID for the simulator environment.
                options.SPOptions.EntityId = new EntityId(SimulatorEntityId);

                // Configure the default callback URL after external authentication.
                options.SPOptions.ReturnUrl = new Uri(config[ExternalAuthCallbackUrlKey]);

                // Enforce minimum signing algorithm for incoming SAML assertions.
                options.SPOptions.MinIncomingSigningAlgorithm = RsaSha1AlgorithmUri;

                // Build the IdP base URL using the simulator URI and GUID.
                string? stubGuid = config[SimulatorGuidKey];
                string? idpUrl = config[SimulatorUriKey] + stubGuid;

                // Configure the Identity Provider (IdP) for the simulator.
                IdentityProvider idp =
                    new(new EntityId(idpUrl + "/Metadata"), options.SPOptions)
                    {
                        AllowUnsolicitedAuthnResponse = true,       // Allow IdP-initiated SSO.
                        Binding = Saml2BindingType.HttpRedirect,    // Use Redirect binding for requests.
                        SingleSignOnServiceUrl = new Uri(idpUrl)    // Set the SSO endpoint.
                    };

                // Register the IdP with the SAML2 options.
                options.IdentityProviders.Add(idp);

                // Configure federation metadata for the simulator environment.
                Federation federation = new Federation(idpUrl + "/Federation", true, options);

                // Optionally configure the public origin (useful behind proxies/load balancers).
                string? publicOrigin = config[PublicOriginKey];

                if (!string.IsNullOrWhiteSpace(publicOrigin))
                {
                    options.SPOptions.PublicOrigin = new Uri(publicOrigin);
                }
            });
        }
    }
}
