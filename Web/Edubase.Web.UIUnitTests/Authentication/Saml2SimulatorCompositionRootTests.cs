using System.Collections.Generic;
using System.Linq;
using Edubase.Web.UI.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.WebSso;
using Xunit;

namespace Edubase.Web.UIUnitTests.Authentication;

public class Saml2SimulatorCompositionRootTests
{
    private static Dictionary<string, string> CreateBaseConfig()
    {
        return new Dictionary<string, string>
        {
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:SASimulatorGuid", "abc123" },
            { "AppSettings:SASimulatorUri", "https://simulator/" }
        };
    }

    private static IConfiguration BuildConfig(Dictionary<string, string> values)
        => new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private static AuthenticationBuilder CreateBuilder(out ServiceCollection services)
    {
        services = new ServiceCollection();

        services.AddLogging();
        services.AddOptions();
        services.AddAuthentication();
        services.AddHttpContextAccessor();

        return new AuthenticationBuilder(services);
    }

    private static Saml2Options BuildOptions(
        AuthenticationBuilder builder, ServiceCollection services, IConfiguration config)
    {
        builder.AddSaml2SimulatorAuthenticationFlow(config);

        ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<Saml2Options> monitor = provider.GetRequiredService<IOptionsMonitor<Saml2Options>>();

        return monitor.Get("Saml2");
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_ConfiguresServiceProviderCorrectly()
    {
        // Arrange
        Dictionary<string, string> configValues = CreateBaseConfig();
        IConfiguration config = BuildConfig(configValues);
        ServiceCollection services;
        AuthenticationBuilder builder = CreateBuilder(out services);

        // Act
        Saml2Options options = BuildOptions(builder, services, config);

        // Assert
        Assert.Equal("http://edubase.gov", options.SPOptions.EntityId.Id);
        Assert.Equal("https://app/callback", options.SPOptions.ReturnUrl.ToString());
        Assert.Equal("http://www.w3.org/2000/09/xmldsig#rsa-sha1", options.SPOptions.MinIncomingSigningAlgorithm);
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_ConfiguresIdentityProviderCorrectly()
    {
        // Arrange
        Dictionary<string, string> configValues = CreateBaseConfig();
        IConfiguration config = BuildConfig(configValues);
        ServiceCollection services;
        AuthenticationBuilder builder = CreateBuilder(out services);

        // Act
        Saml2Options options = BuildOptions(builder, services, config);
        IdentityProvider idp = options.IdentityProviders.KnownIdentityProviders.First();

        // Assert
        Assert.Equal("https://simulator/abc123/Metadata", idp.EntityId.Id);
        Assert.Equal(Saml2BindingType.HttpRedirect, idp.Binding);
        Assert.Equal("https://simulator/abc123", idp.SingleSignOnServiceUrl.ToString());
        Assert.True(idp.AllowUnsolicitedAuthnResponse);
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_RegistersIdentityProvider()
    {
        // Arrange
        Dictionary<string, string> configValues = CreateBaseConfig();
        IConfiguration config = BuildConfig(configValues);
        ServiceCollection services;
        AuthenticationBuilder builder = CreateBuilder(out services);

        // Act
        Saml2Options options = BuildOptions(builder, services, config);

        // Assert
        Assert.Single(options.IdentityProviders.KnownIdentityProviders);
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_SetsPublicOrigin_WhenProvided()
    {
        // Arrange
        Dictionary<string, string> configValues = CreateBaseConfig();
        configValues["AppSettings:PublicOrigin"] = "https://public.example";
        IConfiguration config = BuildConfig(configValues);

        ServiceCollection services;
        AuthenticationBuilder builder = CreateBuilder(out services);

        // Act
        Saml2Options options = BuildOptions(builder, services, config);

        // Assert
        Assert.Equal("https://public.example/", options.SPOptions.PublicOrigin.ToString());
    }

    [Fact]
    public void AddSaml2SimulatorAuthenticationFlow_DoesNotSetPublicOrigin_WhenMissing()
    {
        // Arrange
        Dictionary<string, string> configValues = CreateBaseConfig();
        IConfiguration config = BuildConfig(configValues);

        ServiceCollection services;
        AuthenticationBuilder builder = CreateBuilder(out services);

        // Act
        Saml2Options options = BuildOptions(builder, services, config);

        // Assert
        Assert.Null(options.SPOptions.PublicOrigin);
    }
}
