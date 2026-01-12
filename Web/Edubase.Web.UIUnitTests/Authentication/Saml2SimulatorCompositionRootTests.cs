using System.Collections.Generic;
using System.Linq;
using Edubase.Web.UI.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.WebSso;
using Xunit;

namespace Edubase.Web.UIUnitTests.Authentication;

public class Saml2SimulatorCompositionRootTests
{
    private static IConfiguration CreateConfig(Dictionary<string, string> values)
        => new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private static AuthenticationBuilder CreateBuilder(out ServiceCollection services)
    {
        services = new ServiceCollection();

        services.AddLogging();               // required by Sustainsys
        services.AddOptions();               // required for IOptionsMonitor
        services.AddAuthentication();        // required for AddSaml2()
        services.AddHttpContextAccessor();   // required by Sustainsys internals

        return new AuthenticationBuilder(services);
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_ConfiguresServiceProviderCorrectly()
    {
        var configValues = new Dictionary<string, string>
        {
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:SASimulatorGuid", "abc123" },
            { "AppSettings:SASimulatorUri", "https://simulator/" }
        };

        IConfiguration config = CreateConfig(configValues);
        AuthenticationBuilder builder = CreateBuilder(out var services);

        builder.AddSaml2SimulatorAuthenticationFlow(config);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<Saml2Options>>().Get("Saml2");

        Assert.Equal("http://edubase.gov", options.SPOptions.EntityId.Id);
        Assert.Equal("https://app/callback", options.SPOptions.ReturnUrl.ToString());
        Assert.Equal("http://www.w3.org/2000/09/xmldsig#rsa-sha1", options.SPOptions.MinIncomingSigningAlgorithm);
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_ConfiguresIdentityProviderCorrectly()
    {
        var configValues = new Dictionary<string, string>
        {
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:SASimulatorGuid", "abc123" },
            { "AppSettings:SASimulatorUri", "https://simulator/" }
        };

        IConfiguration config = CreateConfig(configValues);
        AuthenticationBuilder builder = CreateBuilder(out var services);

        builder.AddSaml2SimulatorAuthenticationFlow(config);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<Saml2Options>>().Get("Saml2");

        var idp = options.IdentityProviders.KnownIdentityProviders.First();

        Assert.Equal("https://simulator/abc123/Metadata", idp.EntityId.Id);
        Assert.Equal(Saml2BindingType.HttpRedirect, idp.Binding);
        Assert.Equal("https://simulator/abc123", idp.SingleSignOnServiceUrl.ToString());
        Assert.True(idp.AllowUnsolicitedAuthnResponse);
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_RegistersIdentityProvider()
    {
        var configValues = new Dictionary<string, string>
        {
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:SASimulatorGuid", "abc123" },
            { "AppSettings:SASimulatorUri", "https://simulator/" }
        };

        IConfiguration config = CreateConfig(configValues);
        AuthenticationBuilder builder = CreateBuilder(out var services);

        builder.AddSaml2SimulatorAuthenticationFlow(config);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<Saml2Options>>().Get("Saml2");

        Assert.Single(options.IdentityProviders.KnownIdentityProviders);
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_SetsPublicOrigin_WhenProvided()
    {
        var configValues = new Dictionary<string, string>
        {
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:SASimulatorGuid", "abc123" },
            { "AppSettings:SASimulatorUri", "https://simulator/" },
            { "AppSettings:PublicOrigin", "https://public.example" }
        };

        IConfiguration config = CreateConfig(configValues);
        AuthenticationBuilder builder = CreateBuilder(out var services);

        builder.AddSaml2SimulatorAuthenticationFlow(config);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<Saml2Options>>().Get("Saml2");

        Assert.Equal("https://public.example/", options.SPOptions.PublicOrigin.ToString());
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_DoesNotSetPublicOrigin_WhenMissing()
    {
        var configValues = new Dictionary<string, string>
        {
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:SASimulatorGuid", "abc123" },
            { "AppSettings:SASimulatorUri", "https://simulator/" }
        };

        IConfiguration config = CreateConfig(configValues);
        AuthenticationBuilder builder = CreateBuilder(out var services);

        builder.AddSaml2SimulatorAuthenticationFlow(config);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<Saml2Options>>().Get("Saml2");

        Assert.Null(options.SPOptions.PublicOrigin);
    }
}
