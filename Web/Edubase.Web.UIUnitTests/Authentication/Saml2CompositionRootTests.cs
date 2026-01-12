using System.Collections.Generic;
using Edubase.Web.UI.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;
using Xunit;

namespace Edubase.Web.UIUnitTests.Authentication;

public class Saml2CompositionRootTests
{
    private static AuthenticationBuilder CreateBuilder(out ServiceCollection services)
    {
        services = new ServiceCollection();

        // Required by Sustainsys.Saml2
        services.AddLogging();                     // ILoggerFactory
        services.AddOptions();                     // IOptions<T>
        services.AddHttpContextAccessor();         // IHttpContextAccessor
        services.AddSingleton<IHttpContextFactory, DefaultHttpContextFactory>();

        AuthenticationBuilder builder = new AuthenticationBuilder(services);
        return builder;
    }

    private static IConfiguration CreateConfig(Dictionary<string, string> values)
    {
        ConfigurationBuilder builder = new();
        builder.AddInMemoryCollection(values);
        IConfiguration configuration = builder.Build();
        return configuration;
    }

    [Fact]
    public void AddSaml2AuthenticationFlow_ConfiguresServiceProviderOptions()
    {
        // Arrange
        Dictionary<string, string> configValues = new()
        {
            { "AppSettings:ApplicationIdpEntityId", "http://sp.example" },
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:MetadataLocation", "http://localhost/metadata" }
        };

        IConfiguration config = CreateConfig(configValues);
        ServiceCollection services;
        AuthenticationBuilder builder = CreateBuilder(out services);

        // Act
        builder.AddSaml2AuthenticationFlow(config);

        ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<Saml2Options> optionsMonitor =
            provider.GetRequiredService<IOptionsMonitor<Saml2Options>>();

        Saml2Options options = optionsMonitor.Get("Saml2");

        // Assert
        Assert.Equal("http://sp.example", options.SPOptions.EntityId.Id);
        Assert.Equal("https://app/callback", options.SPOptions.ReturnUrl.ToString());
        Assert.Equal("http://www.w3.org/2000/09/xmldsig#rsa-sha1",
                     options.SPOptions.MinIncomingSigningAlgorithm);
    }

    [Fact]
    public void AddSaml2AuthenticationFlow_SetsPublicOrigin_WhenProvided()
    {
        // Arrange
        Dictionary<string, string> configValues = new()
        {
            { "AppSettings:ApplicationIdpEntityId", "http://sp.example" },
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:MetadataLocation", "http://localhost/metadata" },
            { "AppSettings:PublicOrigin", "https://public-origin" }
        };

        IConfiguration config = CreateConfig(configValues);
        ServiceCollection services;
        AuthenticationBuilder builder = CreateBuilder(out services);

        // Act
        builder.AddSaml2AuthenticationFlow(config);

        ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<Saml2Options> optionsMonitor =
            provider.GetRequiredService<IOptionsMonitor<Saml2Options>>();

        Saml2Options options = optionsMonitor.Get("Saml2");

        // Assert
        Assert.Equal("https://public-origin/", options.SPOptions.PublicOrigin.ToString());
    }

    [Fact]
    public void AddSaml2AuthenticationFlow_DoesNotSetPublicOrigin_WhenMissing()
    {
        // Arrange
        Dictionary<string, string> configValues = new()
        {
            { "AppSettings:ApplicationIdpEntityId", "http://sp.example" },
            { "AppSettings:ExternalAuthDefaultCallbackUrl", "https://app/callback" },
            { "AppSettings:MetadataLocation", "http://localhost/metadata" }
        };

        IConfiguration config = CreateConfig(configValues);
        ServiceCollection services;
        AuthenticationBuilder builder = CreateBuilder(out services);

        // Act
        builder.AddSaml2AuthenticationFlow(config);

        ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<Saml2Options> optionsMonitor =
            provider.GetRequiredService<IOptionsMonitor<Saml2Options>>();

        Saml2Options options = optionsMonitor.Get("Saml2");

        // Assert
        Assert.Null(options.SPOptions.PublicOrigin);
    }
}
