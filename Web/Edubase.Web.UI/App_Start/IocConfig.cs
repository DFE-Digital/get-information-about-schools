using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AutoMapper;
using AzureTableLogger;
using AzureTableLogger.Services;
using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Approvals;
using Edubase.Services.Core;
using Edubase.Services.DataQuality;
using Edubase.Services.Downloads;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Geo;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.IntegrationEndPoints.AzureMaps;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.IntegrationEndPoints.OSPlaces;
using Edubase.Services.IntegrationEndPoints.Smtp;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Services.Security;
using Edubase.Services.Texuna.Approvals;
using Edubase.Services.Texuna.ChangeHistory;
using Edubase.Services.Texuna.Core;
using Edubase.Services.Texuna.Downloads;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Governors;
using Edubase.Services.Texuna.Groups;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using Edubase.Services.Texuna.Serialization;
using Edubase.Web.Resources;
using Edubase.Web.UI.Areas;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Validation;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace Edubase.Web.UI
{
    public static class IocConfig
    {
        public static AutofacDependencyResolver AutofacDependencyResolver { get; private set; }

        private const string Basic = "Basic";

        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            builder.RegisterModule<AutofacWebTypesModule>();
            builder.RegisterModule<ValidationModule>();

            builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterFilterProvider();

            RegisterTypes(builder);

            var container = builder.Build();
            AutofacDependencyResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(AutofacDependencyResolver);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<MockSmtpEndPoint>()
                .As<ISmtpEndPoint>(); // use mock for now, we don't need to email error reports at the moment.

            var cloudStorageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["DataConnectionString"]
                    .ConnectionString);
            builder.RegisterInstance(cloudStorageAccount);

            builder.Register(context => new LoggingServicePolicy
            {
                FlushInterval = TimeSpan.FromSeconds(30),
                RetentionCheckInterval = TimeSpan.FromDays(1),
                RetentionCutoffAge = 90,
                UsagePolicy = UsagePolicy.SCHEDULED
            }).SingleInstance();
            builder.Register(context =>
            {
                var loggingServicePolicy = context.Resolve<LoggingServicePolicy>();
                return new LoggingService(loggingServicePolicy, cloudStorageAccount, "AZTLoggerMessages");
            }).As<ILoggingService>().SingleInstance();
            builder.RegisterType<AzLogger>().As<IAzLogger>().SingleInstance();

            builder.RegisterType<ExceptionHandler>().AsSelf().SingleInstance();

            var dbGeographyConverter = new DbGeographyConverter();
            var jsonConverterCollection = new JsonConverterCollection { dbGeographyConverter };
            builder.RegisterInstance(jsonConverterCollection);

            builder.RegisterType<CacheAccessor>()
                .SingleInstance().As<ICacheAccessor>()
                .UsingConstructor(typeof(JsonConverterCollection));

            builder.RegisterType<CachedLookupService>().As<ICachedLookupService>();

            builder.RegisterInstance(CreatAzureMapsClient()).SingleInstance().Named<HttpClient>("AzureMapsClient");
            builder.Register(c => new AzureMapsService(c.ResolveNamed<HttpClient>("AzureMapsClient"))).As<IAzureMapsService>();

            builder.RegisterInstance(CreatOSPlacesClient()).SingleInstance().Named<HttpClient>("OSPlacesClient");
            builder.Register(c => new OSPlacesApiService(c.ResolveNamed<HttpClient>("OSPlacesClient"))).As<IOSPlacesApiService>();

            builder.RegisterType<PlacesLookupService>().As<IPlacesLookupService>();

            builder.RegisterInstance(CreateFscpdClient()).SingleInstance().Named<HttpClient>("FscpdClient");
            builder.Register(c => new FSCPDService(c.ResolveNamed<HttpClient>("FscpdClient"))).As<IFSCPDService>();

            builder.RegisterInstance(CreateSfbClient()).SingleInstance().Named<HttpClient>("SfbClient");
            builder.Register(c => new FBService(c.ResolveNamed<HttpClient>("SfbClient"))).As<IFBService>();

            builder.RegisterInstance(CreateOfstedClient()).SingleInstance().Named<HttpClient>("OfstedClient");
            builder.Register(c => new OfstedService(c.ResolveNamed<HttpClient>("OfstedClient"))).As<IOfstedService>();

            builder.RegisterType<ExternalLookupService>().As<IExternalLookupService>().SingleInstance().AutoActivate();

            builder.RegisterInstance(AutoMapperWebConfiguration.CreateMapper()).As<IMapper>();

            builder.RegisterType<EstablishmentReadApiService>().As<IEstablishmentReadService>();
            builder.RegisterType<EstablishmentDownloadApiService>().As<IEstablishmentDownloadService>();
            builder.RegisterType<GroupReadApiService>().As<IGroupReadService>();
            builder.RegisterType<GroupDownloadApiService>().As<IGroupDownloadService>();
            builder.RegisterInstance(CreateJsonMediaTypeFormatter()).SingleInstance().AsSelf();

            // HttpClient and HttpClientWrapper injected by default
            builder.RegisterInstance(CreateHttpClient()).SingleInstance().AsSelf();
            builder.RegisterType<HttpClientWrapper>()
                .AsSelf()
                .As<IHttpClientWrapper>();

            // named HttpClient and HttpClientWrapper used by lookup service
            builder.RegisterInstance(CreateLookupClient(
                ConfigurationManager.AppSettings["LookupApiBaseAddress"],
                ConfigurationManager.AppSettings["LookupApiUsername"],
                ConfigurationManager.AppSettings["LookupApiPassword"]
            )).SingleInstance().Named<HttpClient>("LookupApiClient");


            builder.Register(c => new HttpClientWrapper(
                    c.ResolveNamed<HttpClient>("LookupApiClient"),
                    c.Resolve<JsonMediaTypeFormatter>(),
                    c.Resolve<IClientStorage>(),
                    c.Resolve<ApiRecorderSessionItemRepository>()
                ))
                .Named<HttpClientWrapper>("LookupHttpClientWrapper");
            builder.Register(c => new LookupApiService(
                    c.ResolveNamed<HttpClientWrapper>("LookupHttpClientWrapper"),
                    c.Resolve<ISecurityService>()))
                .As<ILookupService>()
                .As<IUserDependentLookupService>();

            // named HttpClient and HttpClientWrapper used specifically for searching Governors
            builder.RegisterInstance(CreateGovernorSearchClient(
                ConfigurationManager.AppSettings["GovernorSearchApiBaseAddress"],
                ConfigurationManager.AppSettings["GovernorSearchApiUsername"],
                ConfigurationManager.AppSettings["GovernorSearchApiPassword"]
            )).SingleInstance().Named<HttpClient>("GovernorSearchApiClient");

            builder.Register(c => new HttpClientWrapper(
                    c.ResolveNamed<HttpClient>("GovernorSearchApiClient"),
                    c.Resolve<JsonMediaTypeFormatter>(),
                    c.Resolve<IClientStorage>(),
                    c.Resolve<ApiRecorderSessionItemRepository>()
                ))
                .Named<HttpClientWrapper>("GovernorSearchHttpClientWrapper");

            builder.Register(c => new GovernorsReadApiService(
                    c.Resolve<HttpClientWrapper>(),
                    c.ResolveNamed<HttpClientWrapper>("GovernorSearchHttpClientWrapper"),
                    c.Resolve<IEstablishmentReadService>()))
                .As<IGovernorsReadService>();

            builder.RegisterType<GovernorDownloadApiService>().As<IGovernorDownloadService>();
            builder.RegisterType<EstablishmentWriteApiService>().As<IEstablishmentWriteService>();
            builder.RegisterType<GovernorsWriteApiService>().As<IGovernorsWriteService>();
            builder.RegisterType<SecurityApiService>().As<ISecurityService>();
            builder.RegisterType<GroupsWriteApiService>().As<IGroupsWriteService>();
            builder.RegisterType<DownloadsApiService>().As<IDownloadsService>();
            builder.RegisterType<ApprovalService>().As<IApprovalService>();
            builder.RegisterType<ChangeHistoryService>().As<IChangeHistoryService>();

            builder.RegisterType<ResourcesHelper>().As<IResourcesHelper>();
            builder.RegisterType<CompaniesHouseService>().As<ICompaniesHouseService>();

            builder.RegisterType<DataQualityWriteService>().As<IDataQualityWriteService>();
            builder.RegisterType<DataQualityReadService>().As<IDataQualityReadService>();
            builder.RegisterType<DataQualityStatusRepository>().As<IDataQualityStatusRepository>();
            builder.RegisterType<LocalAuthoritySetRepository>().As<ILocalAuthoritySetRepository>();
            builder.RegisterType<TokenRepository>().As<ITokenRepository>().SingleInstance();
            builder.RegisterType<UserPreferenceRepository>().As<IUserPreferenceRepository>().SingleInstance();

            builder.RegisterType<BlobService>().As<IBlobService>();

            builder.RegisterType<LayoutHelper>().AsImplementedInterfaces().AsSelf();

            builder.Register(c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>()
                .InstancePerRequest();
            builder.RegisterType<BrowserClientStorage>().As<IClientStorage>().InstancePerRequest();

            builder.RegisterType<ApiRecorderSessionItemRepository>().AsSelf().SingleInstance();
            builder.RegisterType<WebLogItemRepository>().AsSelf().SingleInstance();
            builder.RegisterType<GlossaryRepository>().AsSelf().SingleInstance();
            builder.RegisterType<FaqItemRepository>().AsSelf().SingleInstance();
            builder.RegisterType<FaqGroupRepository>().AsSelf().SingleInstance();
            builder.RegisterType<NotificationBannerRepository>().AsSelf().SingleInstance();
            builder.RegisterType<NotificationTemplateRepository>().AsSelf().SingleInstance();
            builder.RegisterType<NewsArticleRepository>().AsSelf().SingleInstance();
            builder.RegisterType<GovernorsGridViewModelFactory>().As<IGovernorsGridViewModelFactory>();
        }

        public static JsonMediaTypeFormatter CreateJsonMediaTypeFormatter()
        {
            return new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new TexunaCamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
                }
            };
        }

        private static HttpClient CreateHttpClient()
        {
            var timeoutSettings = int.Parse(ConfigurationManager.AppSettings["HttpClient_Timeout"]);

            var client = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["TexunaApiBaseAddress"]),
                Timeout = TimeSpan.FromSeconds(timeoutSettings)
            };

            var apiUsername = ConfigurationManager.AppSettings["api:Username"];
            var apiPassword = ConfigurationManager.AppSettings["api:Password"];

            if (apiUsername != null && apiPassword != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic,
                    new BasicAuthCredentials(apiUsername, apiPassword).ToString());
            }

            return client;
        }

        public static HttpClient CreateFscpdClient()
        {
            var timeoutSettings = int.Parse(ConfigurationManager.AppSettings["FscpdClient_Timeout"]);

            var client = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["FscpdURL"]),
                Timeout = TimeSpan.FromSeconds(timeoutSettings)
            };

            var apiUsername = ConfigurationManager.AppSettings["FscpdUsername"];
            var apiPassword = ConfigurationManager.AppSettings["FscpdPassword"];

            if (!apiUsername.IsNullOrEmpty() && !apiPassword.IsNullOrEmpty())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic,
                    new BasicAuthCredentials(apiUsername, apiPassword).ToString());
            }

            return client;
        }

        private static HttpClient CreateSfbClient()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["FBService_Timeout"], out var timeoutsettings))
            {
                timeoutsettings = 10;
            }

            var client = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["FinancialBenchmarkingApiURL"]),
                Timeout = TimeSpan.FromSeconds(timeoutsettings)
            };

            var apiUsername = ConfigurationManager.AppSettings["FinancialBenchmarkingUsername"];
            var apiPassword = ConfigurationManager.AppSettings["FinancialBenchmarkingPassword"];

            if (!apiUsername.IsNullOrEmpty() && !apiPassword.IsNullOrEmpty())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic,
                    new BasicAuthCredentials(apiUsername, apiPassword).ToString());
            }

            var header = new ProductHeaderValue("GIAS", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var userAgent = new ProductInfoHeaderValue(header);
            client.DefaultRequestHeaders.UserAgent.Add(userAgent);

            return client;
        }

        private static HttpClient CreateOfstedClient()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["OfstedService_TimeoutSeconds"], out var timeoutsettings))
            {
                timeoutsettings = 10;
            }

            var client = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["OfstedService_BaseAddress"]),
                Timeout = TimeSpan.FromSeconds(timeoutsettings)
            };

            var apiUsername = ConfigurationManager.AppSettings["OfstedService_Username"];
            var apiPassword = ConfigurationManager.AppSettings["OfstedService_Password"];

            if (!apiUsername.IsNullOrEmpty() && !apiPassword.IsNullOrEmpty())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic,
                    new BasicAuthCredentials(apiUsername, apiPassword).ToString());
            }

            var productValue = new ProductInfoHeaderValue("GIAS", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var commentValue = new ProductInfoHeaderValue("(Chrome; Edge; Mozilla; +https://www.get-information-schools.service.gov.uk)");

            client.DefaultRequestHeaders.UserAgent.Add(productValue);
            client.DefaultRequestHeaders.UserAgent.Add(commentValue);

            return client;
        }

        public static HttpClient CreateLookupClient(string lookupApiAddress, string lookupApiUsername, string lookupApiPassword)
        {
            // If the given values are empty, default to using the generic/standard API address and credentials
            if (string.IsNullOrWhiteSpace(lookupApiAddress))
            {
                lookupApiAddress = ConfigurationManager.AppSettings["TexunaApiBaseAddress"];
            }
            if (string.IsNullOrEmpty(lookupApiUsername))
            {
                lookupApiUsername = ConfigurationManager.AppSettings["api:Username"];
            }
            if (string.IsNullOrEmpty(lookupApiPassword))
            {
                lookupApiPassword = ConfigurationManager.AppSettings["api:Password"];
            }

            var lookupUri = new Uri(lookupApiAddress);

            if (!int.TryParse(ConfigurationManager.AppSettings["LookupClient_Timeout"], out var timeoutsettings))
            {
                timeoutsettings = 10;
            }

            var client = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = lookupUri,
                Timeout = TimeSpan.FromSeconds(timeoutsettings)
            };

            if (!string.IsNullOrEmpty(lookupApiUsername) && !string.IsNullOrEmpty(lookupApiPassword))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic,
                    new BasicAuthCredentials(lookupApiUsername, lookupApiPassword).ToString());
            }

            return client;
        }

        public static HttpClient CreateGovernorSearchClient(string governorSearchApiAddress, string governorSearchApiUsername, string governorSearchApiPassword)
        {
            // If the given values are empty, default to using the generic/standard API address and credentials
            if (string.IsNullOrWhiteSpace(governorSearchApiAddress))
            {
                governorSearchApiAddress = ConfigurationManager.AppSettings["TexunaApiBaseAddress"];
            }
            if (string.IsNullOrEmpty(governorSearchApiUsername))
            {
                governorSearchApiUsername = ConfigurationManager.AppSettings["api:Username"];
            }
            if (string.IsNullOrEmpty(governorSearchApiPassword))
            {
                governorSearchApiPassword = ConfigurationManager.AppSettings["api:Password"];
            }

            var governorSearchUri = new Uri(governorSearchApiAddress);

            if (!int.TryParse(ConfigurationManager.AppSettings["GovernorSearchClient_Timeout"], out var timeoutsettings))
            {
                timeoutsettings = 30;
            }

            var client = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = governorSearchUri,
                Timeout = TimeSpan.FromSeconds(timeoutsettings)
            };

            if (!string.IsNullOrEmpty(governorSearchApiUsername) && !string.IsNullOrEmpty(governorSearchApiPassword))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic,
                    new BasicAuthCredentials(governorSearchApiUsername, governorSearchApiPassword).ToString());
            }

            return client;
        }

        private static HttpClient CreatOSPlacesClient()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["OSPlacesApiServices_Timeout"], out var timeoutsettings))
            {
                timeoutsettings = 10;
            }

            var client = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["OSPlacesUrl"]),
                Timeout = TimeSpan.FromSeconds(timeoutsettings)
            };

            return client;
        }

        private static HttpClient CreatAzureMapsClient()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["AzureMapService_Timeout"], out var timeoutsettings))
            {
                timeoutsettings = 10;
            }

            var client = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["AzureMapsUrl"]),
                Timeout = TimeSpan.FromSeconds(timeoutsettings)
            };

            return client;
        }
    }
}
