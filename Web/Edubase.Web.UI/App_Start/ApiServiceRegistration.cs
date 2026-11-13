using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Core;
using Edubase.Services.Establishments;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.IntegrationEndPoints.AzureMaps;
using Edubase.Services.IntegrationEndPoints.OSPlaces;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Services.Texuna.Governors;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using Edubase.Services.Texuna.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Edubase.Web.UI.App_Start
{
    public static class ApiServiceRegistration
    {
        private const string TexunaApiClient = "TexunaApiClient";
        private const string LookupApiClient = "LookupApiClient";
        private const string GovernorSearchApiClient = "GovernorSearchApiClient";

        public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddClient(services, configuration, TexunaApiClient, "TexunaApiBaseAddress", "HttpClient_Timeout", "api:Username", "api:Password");
            AddClient(services, configuration, LookupApiClient, "LookupApiBaseAddress", "LookupClient_Timeout", "LookupApiUsername", "LookupApiPassword");
            AddClient(services, configuration, GovernorSearchApiClient, "GovernorSearchApiBaseAddress", "GovernorSearchClient_Timeout", "GovernorSearchApiUsername", "GovernorSearchApiPassword");

            services.AddSingleton(new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new TexunaCamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
                }
            });

            services.AddScoped<HttpClientWrapper>(provider => CreateWrapper(provider, TexunaApiClient));
            services.AddScoped<IHttpClientWrapper>(provider => provider.GetRequiredService<HttpClientWrapper>());
            services.AddScoped<ISecurityService, SecurityApiService>();
            services.AddScoped<LookupApiService>(provider => new LookupApiService(
                CreateWrapper(provider, LookupApiClient),
                provider.GetRequiredService<ISecurityService>()));
            services.AddScoped<ILookupService>(provider => provider.GetRequiredService<LookupApiService>());
            services.AddScoped<IUserDependentLookupService>(provider => provider.GetRequiredService<LookupApiService>());
            services.AddScoped<IGovernorsReadService>(provider => new GovernorsReadApiService(
                provider.GetRequiredService<HttpClientWrapper>(),
                CreateWrapper(provider, GovernorSearchApiClient),
                provider.GetRequiredService<IEstablishmentReadService>()));
            services.AddScoped<IGovernorsWriteService, GovernorsWriteApiService>();
            services.AddScoped<IGovernorDownloadService, GovernorDownloadApiService>();
        }

        public static void AddExternalApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddExternalClient(services, configuration, "OSPlacesClient", "OSPlacesApiBaseUrl")
                .ConfigureHttpClient(client =>
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                        configuration["AppSettings:OSPlacesApiKey"]);
                });
            AddExternalClient(services, configuration, "AzureMapsClient", "AzureMapsUrl");
            AddExternalClient(services, configuration, "FscpdClient", "FscpdURL");
            AddExternalClient(services, configuration, "SfbClient", "FinancialBenchmarkingApiURL");

            services.AddScoped<IOSPlacesApiService>(provider => new OSPlacesApiService(
                provider.GetRequiredService<IHttpClientFactory>().CreateClient("OSPlacesClient"), configuration));
            services.AddScoped<IAzureMapsService>(provider => new AzureMapsService(configuration,
                provider.GetRequiredService<IHttpClientFactory>().CreateClient("AzureMapsClient")));
            services.AddScoped<IFSCPDService>(provider => new FSCPDService(
                provider.GetRequiredService<IHttpClientFactory>().CreateClient("FscpdClient"), configuration));
            services.AddScoped<IFBService>(provider => new FBService(
                provider.GetRequiredService<IHttpClientFactory>().CreateClient("SfbClient"), configuration));
        }

        private static IHttpClientBuilder AddExternalClient(IServiceCollection services, IConfiguration configuration, string name, string baseAddressKey)
        {
            return services.AddHttpClient(name, client =>
            {
                client.BaseAddress = new Uri(configuration["AppSettings:" + baseAddressKey]);
                // Each external service applies its configured timeout through Polly.
                client.Timeout = Timeout.InfiniteTimeSpan;
            });
        }

        private static void AddClient(IServiceCollection services, IConfiguration configuration, string name,
            string baseAddressKey, string timeoutKey, string usernameKey, string passwordKey)
        {
            services.AddHttpClient(name, client =>
            {
                client.BaseAddress = new Uri(configuration["AppSettings:" + baseAddressKey]);
                if (!int.TryParse(configuration["AppSettings:" + timeoutKey], out var timeoutSeconds))
                {
                    timeoutSeconds = 30;
                }
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

                var username = configuration["AppSettings:" + usernameKey];
                var password = configuration["AppSettings:" + passwordKey];
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false });
        }

        private static HttpClientWrapper CreateWrapper(IServiceProvider provider, string name)
        {
            var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
            return new HttpClientWrapper(client,
                provider.GetRequiredService<JsonMediaTypeFormatter>(),
                provider.GetRequiredService<IClientStorage>(),
                provider.GetRequiredService<ApiRecorderSessionItemRepository>());
        }
    }
}
