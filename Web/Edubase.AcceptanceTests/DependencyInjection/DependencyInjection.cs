using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Edubase.AcceptanceTests.Api;
using Edubase.AcceptanceTests.Establishments;
using Edubase.AcceptanceTests.SigninAuthorities;
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
            var baseAddress = "https://localhost:44309";
            var environment = "dev";

            var services = new ServiceCollection();

            services.AddHardCodedUsers();
            services.AddEstablishmentsFromFrontEnd();
            services.AddGiasFrontEnd(baseAddress, environment);

            return services;
        }

        private static void AddHardCodedUsers(this ServiceCollection services)
        {
            services.AddScoped<IUsers, UsersFromHardCodedValues>();
        }

        private static void AddEstablishmentsFromFrontEnd(this ServiceCollection services)
        {
            services.AddScoped<IEstablishments, EstablishmentsFromFrontEndApi>();
        }

        private static void AddGiasFrontEnd(this ServiceCollection services, string baseAddress, string environment)
        {
            services.AddHttpClient("GiasFrontEnd", client =>
            {
                client.BaseAddress = new Uri(baseAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    UseDefaultCredentials = true,
                    CookieContainer = new CookieContainer()
                });

            services.AddScoped<HttpClient>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();

                return factory.CreateClient("GiasFrontEnd");
            });

            services.AddScoped<ISignInAuthority>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();

                return new SignInSimulator(httpClient, environment);
            });

            services.AddScoped<IApiClient>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                var signinAuthority = sp.GetRequiredService<ISignInAuthority>();

                return new ApiForGiasFrontEnd(httpClient, signinAuthority);
            });
        }
    }
}
