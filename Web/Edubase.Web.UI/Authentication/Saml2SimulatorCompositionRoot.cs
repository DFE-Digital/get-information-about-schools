using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;

namespace Edubase.Web.UI.Authentication;

/// <summary>
/// Provides extension methods for configuring SAML2 authentication schemes.
/// Supports both real IdP configuration and simulator configuration.
/// </summary>
public static class Saml2SimulatorCompositionRoot
{
    // Centralized configuration keys.
    private const string ExternalAuthCallbackUrlKey = "AppSettings:ExternalAuthDefaultCallbackUrl";
    private const string SimulatorGuidKey = "AppSettings:SASimulatorGuid";
    private const string SimulatorUriKey = "AppSettings:SASimulatorUri";
    private const string PublicOriginKey = "AppSettings:PublicOrigin";

    // Constant SP entity ID for simulator.
    private const string SimulatorEntityId = "http://edubase.gov";

    // Constant algorithm URI for minimum signing requirement.
    private const string RsaSha1AlgorithmUri = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

    /// <summary>
    /// Configures the simulator SAML2 authentication scheme using application settings.
    /// </summary>
    /// <param name="builder">The authentication builder to extend.</param>
    /// <param name="config">The application configuration containing simulator settings.</param>
    /// <returns>The updated <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddSaml2SimulatorAuthenticationFlow(
        this AuthenticationBuilder builder, IConfiguration config)
    {
        return builder.AddSaml2(options =>
        {
            // Set the SP entity ID to the simulator constant.
            options.SPOptions.EntityId = new EntityId(SimulatorEntityId);

            // Set the default return URL for external authentication callbacks.
            options.SPOptions.ReturnUrl = new Uri(config[ExternalAuthCallbackUrlKey]);

            // Require at least RSA-SHA1 for incoming signatures.
            options.SPOptions.MinIncomingSigningAlgorithm = RsaSha1AlgorithmUri;

            // Build the IdP URL using simulator URI and GUID.
            string? stubGuid = config[SimulatorGuidKey];
            string? idpUrl = config[SimulatorUriKey] + stubGuid;

            // Configure the Identity Provider (IdP) for the simulator.
            IdentityProvider idp =
                new(new EntityId(idpUrl + "/Metadata"), options.SPOptions)
                {
                    AllowUnsolicitedAuthnResponse = true,       // Allow IdP-initiated logins.
                    Binding = Saml2BindingType.HttpRedirect,    // Use HTTP Redirect binding.
                    SingleSignOnServiceUrl = new Uri(idpUrl)    // Set the SSO service URL.
                };

            // Register the IdP with the options.
            options.IdentityProviders.Add(idp);

            // Configure federation metadata for simulator.
            Federation federation = new Federation(idpUrl + "/Federation", true, options);

            // Optionally set the public origin if configured.
            string? publicOrigin = config[PublicOriginKey];

            if (!string.IsNullOrWhiteSpace(publicOrigin))
            {
                options.SPOptions.PublicOrigin = new Uri(publicOrigin);
            }
        });
    }
}
