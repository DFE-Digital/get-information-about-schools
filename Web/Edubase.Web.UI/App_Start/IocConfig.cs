using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AutoMapper;
using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Services;
using Edubase.Services.Approvals;
using Edubase.Services.Downloads;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.IntegrationEndPoints.Google;
using Edubase.Services.IntegrationEndPoints.Smtp;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Services.Security;
using Edubase.Services.Texuna.Approvals;
using Edubase.Services.Texuna.ChangeHistory;
using Edubase.Services.Texuna.Downloads;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Governors;
using Edubase.Services.Texuna.Groups;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using Edubase.Web.Resources;
using Edubase.Web.UI.Validation;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Edubase.Data.Repositories;
using Edubase.Services.DataQuality;
using Edubase.Web.UI.Helpers;
using Edubase.Services.Core;
using System.Net.Http.Formatting;
using Edubase.Services.Texuna.Serialization;
using System.Net.Http.Headers;
using Edubase.Services.Texuna.Core;
using System.Web;

namespace Edubase.Web.UI
{
    public static class IocConfig
    {
        public static AutofacDependencyResolver AutofacDependencyResolver { get; private set; }

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
            builder.RegisterType<MockSmtpEndPoint>().As<ISmtpEndPoint>(); // use mock for now, we don't need to email error reports at the moment.

            builder.RegisterType<MessageLoggingService>()
                .As<IMessageLoggingService>()
                .As<IExceptionLogger>()
                .SingleInstance();

            var dbGeographyConverter = new DbGeographyConverter();
            var jsonConverterCollection = new JsonConverterCollection() { dbGeographyConverter };
            builder.RegisterInstance(jsonConverterCollection);

            builder.RegisterType<CacheAccessor>()
                .SingleInstance().As<ICacheAccessor>()
                .UsingConstructor(typeof(JsonConverterCollection));

            builder.RegisterInstance(Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString));

            builder.RegisterType<CachedLookupService>().As<ICachedLookupService>();
            builder.RegisterType<GooglePlacesService>().As<IGooglePlacesService>();
            builder.RegisterInstance(AutoMapperWebConfiguration.CreateMapper()).As<IMapper>();
            builder.RegisterInstance(new NomenclatureService()).AsSelf();

            builder.RegisterType<EstablishmentReadApiService>().As<IEstablishmentReadService>();
            builder.RegisterType<EstablishmentDownloadApiService>().As<IEstablishmentDownloadService>();
            builder.RegisterType<GroupReadApiService>().As<IGroupReadService>();
            builder.RegisterType<GroupDownloadApiService>().As<IGroupDownloadService>();
            builder.RegisterType<LookupApiService>().As<ILookupService>();

            builder.RegisterInstance(CreateJsonMediaTypeFormatter()).SingleInstance().AsSelf();

            builder.RegisterInstance(CreateHttpClient()).SingleInstance().AsSelf();
            builder.RegisterType<HttpClientWrapper>().AsSelf();

            builder.RegisterType<GovernorDownloadApiService>().As<IGovernorDownloadService>();
            builder.RegisterType<GovernorsReadApiService>().As<IGovernorsReadService>();
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

            builder.RegisterType<BlobService>().As<IBlobService>();

            builder.RegisterType<LayoutHelper>();

            builder.Register(c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>().InstancePerRequest();
            builder.RegisterType<BrowserClientStorage>().As<IClientStorage>().InstancePerRequest();

            builder.RegisterType<ApiRecorderSessionItemRepository>().AsSelf().SingleInstance();
            builder.RegisterType<GlossaryRepository>().AsSelf().SingleInstance();
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
            var client = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["TexunaApiBaseAddress"]),
                Timeout = TimeSpan.FromSeconds(180),
            };

            var apiUsername = ConfigurationManager.AppSettings["api:Username"];
            var apiPassword = ConfigurationManager.AppSettings["api:Password"];

            if (apiUsername != null && apiPassword != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", new BasicAuthCredentials(apiUsername, apiPassword).ToString());

            return client;
        }


    }
}