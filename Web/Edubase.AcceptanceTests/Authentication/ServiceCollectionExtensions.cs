using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Edubase.AcceptanceTests.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton((provider) =>
            {
                ApiOptions apiConfig = provider.GetRequiredService<AppSettings>().ApiConfig();
                IMessageSink sink = provider.GetRequiredService<IMessageSink>();
                TestOptions testOptions = provider.GetRequiredService<AppSettings>().TestConfig();

                var rateLimitingHandler = new RateLimitingHandler(TimeSpan.FromSeconds(testOptions.RateLimiting))
                {
                    InnerHandler = new HttpClientHandler()
                };

                var httpClient = new HttpClient(rateLimitingHandler)
                {
                    BaseAddress = new Uri(apiConfig.Url)
                };

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        scheme: "Basic",
                        parameter: Convert.ToBase64String(
                            Encoding.UTF8.GetBytes($"{apiConfig.AuthUsername}:{apiConfig.AuthPassword}")
                        )
                );

                httpClient.DefaultRequestHeaders.Add("sa_user_id", "3600154");

                return new ApiClient(httpClient, sink);
            });
            //services.AddSingleton<ApprovalsPage>();
            services.AddSingleton<AuthenticatedHttpClient>();
            services.AddSingleton<AppSettings>();
            //services.AddSingleton<BulkCreateNewAcademiesPage>();
            //services.AddSingleton<BulkUpdateEstablishmentsPage>();
            services.AddTransient<ClientLoginHelper>();
            //services.AddSingleton<CreateChildrensCentreGroupsCollaborationsPage>();
            //services.AddSingleton<CreateNewEstablishmentsPage>();
            //services.AddSingleton<DownloadsPage>();
            //services.AddSingleton<EstablishmentPage>();
            //services.AddSingleton<EstablishmentEditPage>();
            //services.AddSingleton<FaqPage>();
            //services.AddSingleton<GovernancePage>();
            //services.AddSingleton<GovernanceEditPage>();
            //services.AddSingleton<GroupPage>();
            //services.AddSingleton<GroupEditPage>();
            services.AddSingleton<HttpClient>(sp =>
            {
                var testOptions = sp.GetRequiredService<AppSettings>().TestConfig();

                var httpClientHandler = new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    UseDefaultCredentials = true,
                    CookieContainer = new CookieContainer()
                };

                var rateLimitingHandler = new RateLimitingHandler(TimeSpan.FromSeconds(testOptions.RateLimiting))
                {
                    InnerHandler = httpClientHandler
                };

                var authenticatedClient = sp.GetRequiredService<AuthenticatedHttpClient>();
                return authenticatedClient.AuthenicatedClient(rateLimitingHandler);
            });
            //services.AddSingleton<HomePage>();
            //services.AddSingleton<IEstablishmentService, EstablishmentService>();
            //services.AddSingleton<IGroupService, EstablishmentGroupService>();
            //services.AddSingleton<IGovernorService, GovernorService>();
            //services.AddSingleton<ILookupService, LookupService>();
            services.AddTransient<LoginSignInSimulator>(provider =>
            {
                var httpClient = provider.GetRequiredService<HttpClient>();
                var appSettings = provider.GetRequiredService<AppSettings>();
                var userOptions = appSettings.UserConfig(); // Loads from signinSimUsers.{env}.json

                return new LoginSignInSimulator(httpClient, userOptions);
            });
            //services.AddTransient<ManageAcademyOpeningsPage>();
            //services.AddSingleton<SiteHeaderPage>();
            //services.AddSingleton<SearchResultsPage>();
            //services.AddSingleton<SearchLocationsMatchingPage>();

            return services;
        }
    }
}
