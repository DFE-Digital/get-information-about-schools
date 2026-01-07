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
public static class Saml2CompositionRoot
{
    // Centralized configuration keys.
    private const string ApplicationIdpEntityIdKey = "AppSettings:ApplicationIdpEntityId";
    private const string ExternalAuthCallbackUrlKey = "AppSettings:ExternalAuthDefaultCallbackUrl";
    private const string MetadataLocationKey = "AppSettings:MetadataLocation";
    private const string PublicOriginKey = "AppSettings:PublicOrigin";

    // Constant algorithm URI for minimum signing requirement.
    private const string RsaSha1AlgorithmUri = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

    /// <summary>
    /// Configures the real SAML2 authentication scheme using application settings.
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
            // Set the SP entity ID from configuration.
            options.SPOptions.EntityId =
                new EntityId(configuration[ApplicationIdpEntityIdKey]);

            // Set the default return URL for external authentication callbacks.
            options.SPOptions.ReturnUrl =
                new Uri(configuration[ExternalAuthCallbackUrlKey]);

            // Require at least RSA-SHA1 for incoming signatures.
            options.SPOptions.MinIncomingSigningAlgorithm = RsaSha1AlgorithmUri;

            // Configure the Identity Provider (IdP) using metadata location.
            IdentityProvider idp =
                new IdentityProvider(
                    new EntityId(configuration[MetadataLocationKey]), options.SPOptions)
                {
                    AllowUnsolicitedAuthnResponse = true,                   // Allow IdP-initiated logins.
                    Binding = Saml2BindingType.HttpRedirect,                // Use HTTP Redirect binding.
                    MetadataLocation = configuration[MetadataLocationKey],  // Metadata URL.
                    DisableOutboundLogoutRequests = true                    // Disable SP-initiated logout.
                };

            // Register the IdP with the options.
            options.IdentityProviders.Add(idp);

            // Optionally set the public origin if configured.
            string? publicOrigin = configuration[PublicOriginKey];

            if (!string.IsNullOrWhiteSpace(publicOrigin))
            {
                options.SPOptions.PublicOrigin = new Uri(publicOrigin);
            }
        });
    }
}
