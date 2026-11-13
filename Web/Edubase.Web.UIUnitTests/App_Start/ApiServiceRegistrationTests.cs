using System;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Core;
using Edubase.Services.Establishments;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Lookup;
using Edubase.Services.IntegrationEndPoints.OSPlaces;
using Edubase.Web.UI.App_Start;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Polly.Timeout;
using Xunit;

namespace Edubase.Web.UIUnitTests.App_Start
{
    public class ApiServiceRegistrationTests
    {
        [Theory]
        [InlineData("OSPlacesClient", "OSPlacesApiBaseUrl")]
        [InlineData("AzureMapsClient", "AzureMapsUrl")]
        [InlineData("FscpdClient", "FscpdURL")]
        [InlineData("SfbClient", "FinancialBenchmarkingApiURL")]
        public void AddExternalApiServices_ConfiguresNamedClient(string name, string addressKey)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            configuration["AppSettings:" + addressKey] = $"https://{name.ToLowerInvariant()}.example/";
            configuration["AppSettings:OSPlacesApiKey"] = "test-key";
            var services = new ServiceCollection();
            services.AddExternalApiServices(configuration);

            using var provider = services.BuildServiceProvider();
            using var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
            Assert.Equal(configuration["AppSettings:" + addressKey], client.BaseAddress.AbsoluteUri);
            Assert.Equal(Timeout.InfiniteTimeSpan, client.Timeout);
        }

        [Fact]
        public async Task AddExternalApiServices_OSPlacesUsesConfiguredKeyAndTimeout()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            configuration["AppSettings:OSPlacesApiBaseUrl"] = "https://osplaces.example/";
            configuration["AppSettings:OSPlacesApiKey"] = "test-key";
            configuration["AppSettings:OSPlacesApiServices_Timeout"] = "1";
            configuration["AppSettings:OSPlacesApiServices_RetryIntervals"] = "";
            var services = new ServiceCollection();
            services.AddExternalApiServices(configuration);
            var handler = new DelayedResponseHandler();
            services.AddHttpClient("OSPlacesClient").ConfigurePrimaryHttpMessageHandler(() => handler);

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var subject = scope.ServiceProvider.GetRequiredService<IOSPlacesApiService>();

            await Assert.ThrowsAsync<TimeoutRejectedException>(() => subject.SearchAsync("SW1A 1AA"));

            Assert.Equal("osplaces.example", handler.RequestUri.Host);
            Assert.Contains("key=test-key", handler.RequestUri.Query);
        }

        private class DelayedResponseHandler : HttpMessageHandler
        {
            public Uri RequestUri { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                RequestUri = request.RequestUri;
                await Task.Delay(2000, cancellationToken);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [Theory]
        [InlineData("TexunaApiClient", "TexunaApiBaseAddress", "HttpClient_Timeout", "api:Username", "api:Password", 11)]
        [InlineData("LookupApiClient", "LookupApiBaseAddress", "LookupClient_Timeout", "LookupApiUsername", "LookupApiPassword", 22)]
        [InlineData("GovernorSearchApiClient", "GovernorSearchApiBaseAddress", "GovernorSearchClient_Timeout", "GovernorSearchApiUsername", "GovernorSearchApiPassword", 33)]
        public void AddApiServices_ConfiguresNamedClient(string name, string addressKey, string timeoutKey,
            string usernameKey, string passwordKey, int timeoutSeconds)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            configuration["AppSettings:" + addressKey] = $"https://{name.ToLowerInvariant()}.example/";
            configuration["AppSettings:" + timeoutKey] = timeoutSeconds.ToString();
            configuration["AppSettings:" + usernameKey] = name;
            configuration["AppSettings:" + passwordKey] = "password";
            var services = new ServiceCollection();
            services.AddApiServices(configuration);

            using var provider = services.BuildServiceProvider();
            using var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
            Assert.Equal(configuration["AppSettings:" + addressKey], client.BaseAddress.AbsoluteUri);
            Assert.Equal(TimeSpan.FromSeconds(timeoutSeconds), client.Timeout);
            Assert.Equal("Basic", client.DefaultRequestHeaders.Authorization.Scheme);
            Assert.Equal(Convert.ToBase64String(Encoding.ASCII.GetBytes($"{name}:password")),
                client.DefaultRequestHeaders.Authorization.Parameter);
        }

        [Theory]
        [InlineData(typeof(IHttpClientWrapper), "TexunaApiClient")]
        [InlineData(typeof(ILookupService), "LookupApiClient", "TexunaApiClient")]
        [InlineData(typeof(IGovernorsReadService), "TexunaApiClient", "GovernorSearchApiClient")]
        [InlineData(typeof(IGovernorsWriteService), "TexunaApiClient")]
        [InlineData(typeof(IGovernorDownloadService), "TexunaApiClient")]
        public void AddApiServices_ResolvesServicesWithCorrectClients(Type serviceType, params string[] clientNames)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var services = new ServiceCollection();
            services.AddApiServices(configuration);
            services.AddScoped<IClientStorage>(_ => Mock.Of<IClientStorage>());
            services.AddScoped<IEstablishmentReadService>(_ => Mock.Of<IEstablishmentReadService>());
            services.AddSingleton(new ApiRecorderSessionItemRepository(new TableServiceClient("UseDevelopmentStorage=true")));
            var factory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            using var client = new HttpClient();
            foreach (var name in clientNames)
            {
                factory.Setup(x => x.CreateClient(name)).Returns(client);
            }
            services.AddSingleton(factory.Object);
            services.AddSingleton(new HttpClient());

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            Assert.NotNull(scope.ServiceProvider.GetRequiredService(serviceType));
            foreach (var name in clientNames)
            {
                factory.Verify(x => x.CreateClient(name), Times.Once);
            }
        }
    }
}
