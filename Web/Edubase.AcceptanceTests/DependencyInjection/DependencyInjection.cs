using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Edubase.AcceptanceTests.Api;
using Edubase.AcceptanceTests.Establishments;
using Edubase.AcceptanceTests.Users;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace Edubase.AcceptanceTests.DependencyInjection
{
    internal static class DependencyInjection
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            services.AddScoped<IUsers, UsersHardCoded>();

            services.AddHttpClient("Edubase", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://localhost:44309");

                var basicAuthorizationUserName = string.Empty;
                var basicAuthorizationUserPassword = string.Empty;

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes($"{basicAuthorizationUserName}:{basicAuthorizationUserPassword}")
                        )
                    );
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseDefaultCredentials = true,
                CookieContainer = new CookieContainer()
            });

            services.AddScoped<IApiClient>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();

                return new ApiClientForEnvironment(
                    factory.CreateClient("Edubase"),
                    "dev");
            });

            services.AddScoped<IEstablishments, EstablishmentsFromFrontEndApi>();

            return services;
        }
    }
}
