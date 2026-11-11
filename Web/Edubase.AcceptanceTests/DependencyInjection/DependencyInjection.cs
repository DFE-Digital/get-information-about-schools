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
            
            services.AddAcceptanceTestLogging();
            services.AddSingleton(settings);

            services.AddHardCodedUsers();
            services.AddEstablishmentsFromGiasFrontEnd();
            services.AddGiasFrontEnd(settings);
            services.AddUserPreferencesInPersistentStorage(settings);


            return services;
        }

        private static void AddAcceptanceTestLogging(this ServiceCollection services)
        {
            services.AddScoped<IAcceptanceTestLogger, ReqnrollLogger>();
        }

        private static void AddHardCodedUsers(this ServiceCollection services)
        {
            services.AddScoped<IUsers>(sp => new UsersFromHardCodedValues(sp.GetRequiredService<Settings>().Environment));
        }

        private static void AddEstablishmentsFromGiasFrontEnd(this ServiceCollection services)
        {
            services.AddScoped<IEstablishments, EstablishmentsFromGiasFrontEnd>();
            services.AddScoped<IEstablishmentSearchResultsScreen, EstablishmentSearchResultsScreenFromGiasFrontEnd>();
        }

        private static void AddUserPreferencesInPersistentStorage(this ServiceCollection services, Settings settings)
        {
            services.AddScoped<IUserPreferencesInMigrationSource>(sp => new UserPreferencesInAzureTableStorage(settings.DataConnectionString));
            services.AddDbContext<UserPreferencesDbContext>(options => options.UseSqlServer(settings.FrontEndDatabase));
            services.AddScoped<IUserPreferencesInMigrationTarget, UserPreferencesInSQLServer>();

            if (settings.Feature_UserPreferencesMigration == false)
            {
                var x = services.BuildServiceProvider().GetRequiredService<IUserPreferencesInMigrationSource>();

                services.AddScoped<IUserPreferencesInPersistentStorage>(sp => x);
            }
            else
            {
                var x = services.BuildServiceProvider().GetRequiredService<IUserPreferencesInMigrationTarget>();

                services.AddScoped<IUserPreferencesInPersistentStorage>(sp => x);
            }
        }

        private static void AddGiasFrontEnd(this ServiceCollection services, Settings settings)
        {
            // Factory-pooled handlers share cookies across scenario scopes.
            // Give each scenario its own handler and authenticated session.
            services.AddScoped<HttpClientHandler>(_ =>
            {
                var credentials = settings.HttpAuthModuleCredentials.Split(':');

                var networkCredentials = new NetworkCredential(credentials[0], credentials[1]);

                return new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    Credentials = networkCredentials,
                    CookieContainer = new CookieContainer()
                };
            });

            services.AddScoped<HttpClient>(sp => new HttpClient(
                sp.GetRequiredService<HttpClientHandler>(), disposeHandler: false)
            {
                BaseAddress = new Uri(settings.GiasFrontEndBaseAddress)
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
                if (settings.UseLocalSignIn)
                {
                    return new SigninSimulatorOnLocalMachine(
                        serviceProvider.GetRequiredService<HttpClient>());
                }

                return new SigninSimulatorInAzure(
                    serviceProvider.GetRequiredService<HttpClient>(),
                    serviceProvider.GetRequiredService<IApi>(), settings.Environment);
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
