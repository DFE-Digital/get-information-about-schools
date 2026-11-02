using System.Net;
using Edubase.AcceptanceTests.Apis;
using Edubase.AcceptanceTests.Establishments;
using Edubase.AcceptanceTests.GiasFrontEnd;
using Edubase.AcceptanceTests.SigninAuthorities;
using Edubase.AcceptanceTests.UserPreferences;
using Edubase.AcceptanceTests.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace Edubase.AcceptanceTests.DependencyInjection
{
    internal static class DependencyInjection
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var settings = Settings.Load();

            var services = new ServiceCollection();

            services.AddSingleton(settings);

            services.AddHardCodedUsers();
            services.AddEstablishmentsFromGiasFrontEnd();
            services.AddGiasFrontEnd(settings.GiasFrontEndBaseAddress, settings.Environment, settings.UseLocalSignIn);
            services.AddUserPreferencesInPersistentStorage();


            return services;
        }

        private static void AddHardCodedUsers(this ServiceCollection services)
        {
            services.AddScoped<IUsers, UsersFromHardCodedValues>();
        }

        private static void AddEstablishmentsFromGiasFrontEnd(this ServiceCollection services)
        {
            services.AddScoped<IEstablishments, EstablishmentsFromGiasFrontEnd>();
            services.AddScoped<IEstablishmentSearchResultsScreen, EstablishmentSearchResultsScreenFromGiasFrontEnd>();
        }

        private static void AddUserPreferencesInPersistentStorage(this ServiceCollection services)
        {
            var connectionString = UserPreferencesInSQLServer.LoadFrontEndDatabaseConnectionString();

            services.AddDbContext<UserPreferencesDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IUserPreferencesInPersistentStorage, UserPreferencesInSQLServer>();
        }

        private static void AddGiasFrontEnd(
            this ServiceCollection services,
            string baseAddress,
            string environment,
            bool useLocalSimulator)
        {
            // Factory-pooled handlers share cookies across scenario scopes.
            // Give each scenario its own handler and authenticated session.
            services.AddScoped<HttpClientHandler>(_ =>
                new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    UseDefaultCredentials = true,
                    CookieContainer = new CookieContainer()
                });

            services.AddScoped<HttpClient>(sp => new HttpClient(
                sp.GetRequiredService<HttpClientHandler>(), disposeHandler: false)
            {
                BaseAddress = new Uri(baseAddress)
            });

            services.AddHttpClient("AzureSignInSimulator")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AllowAutoRedirect = false
                });

            services.AddScoped<IApi>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var simulatorClient = factory.CreateClient("AzureSignInSimulator");

                return new HttpApi(simulatorClient);
            });

            services.AddScoped<ISignInAuthority>(serviceProvider =>
            {
                if (useLocalSimulator)
                {
                    return new SigninSimulatorOnLocalMachine(
                        serviceProvider.GetRequiredService<HttpClient>());
                }

                return new SigninSimulatorInAzure(
                    serviceProvider.GetRequiredService<HttpClient>(),
                    serviceProvider.GetRequiredService<IApi>(), environment);
            });

            services.AddScoped<IGiasFrontEnd>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                var signInAuthority = sp.GetRequiredService<ISignInAuthority>();

                return new GiasFrontEndViaHttp(httpClient, signInAuthority);
            });
        }
    }
}
