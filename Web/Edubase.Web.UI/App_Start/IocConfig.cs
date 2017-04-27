using System;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services;
using AutoMapper;
using Edubase.Data.Entity;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using System.Configuration;
using System.Net.Http;
using Edubase.Common.Cache;
using Edubase.Common;
using Edubase.Data.Repositories.Establishments;
using Edubase.Data.Repositories.LocalAuthorities;
using Edubase.Data;
using Edubase.Services.IntegrationEndPoints.Smtp;
using Edubase.Data.DbContext;
using Newtonsoft.Json;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Data.Repositories.Groups;
using Edubase.Data.Repositories.Groups.Abstract;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Governors;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Validation;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.Core;
using Edubase.Services.Nomenclature;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Groups;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Governors;
using Edubase.Web.Resources;
using Edubase.Services.Texuna.Core;
using Edubase.Services.Texuna.Security;
using Edubase.Services.Downloads;
using Edubase.Services.Texuna.Downloads;

namespace Edubase.Web.UI
{
    public static class IocConfig
    {
        private static IContainer _container;
        public static IContainer Container => _container;

        public static void Register()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            builder.RegisterModule<ValidationModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            RegisterTypes(builder);

            // Set the dependency resolver to be Autofac.
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<MockSmtpEndPoint>().As<ISmtpEndPoint>(); // use mock for now, we don't need to email error reports at the moment.

            builder.RegisterType<MessageLoggingService>()
                .As<IMessageLoggingService>()
                .As<IExceptionLogger>()
                .SingleInstance();

            builder.RegisterInstance(new JsonConverterCollection() { new DbGeographyConverter() });

            builder.RegisterType<CacheAccessor>()
                .SingleInstance().As<ICacheAccessor>()
                .UsingConstructor(typeof(JsonConverterCollection));

            builder.RegisterInstance(Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString));

            builder.RegisterType<BlobService>().As<IBlobService>();
            builder.RegisterType<CachedLookupService>().As<ICachedLookupService>();
            builder.RegisterInstance(AutoMapperWebConfiguration.CreateMapper()).As<IMapper>();
            builder.RegisterInstance(new NomenclatureService()).AsSelf();

#if (TEXAPI)
            builder.RegisterType<EstablishmentReadApiService>().As<IEstablishmentReadService>();
            builder.RegisterType<EstablishmentDownloadApiService>().As<IEstablishmentDownloadService>();
            builder.RegisterType<GroupReadApiService>().As<IGroupReadService>();
            builder.RegisterType<GroupDownloadApiService>().As<IGroupDownloadService>();
            builder.RegisterType<LookupApiService>().As<ILookupService>();
            builder.RegisterInstance(new HttpClient { BaseAddress = new Uri(ConfigurationManager.AppSettings["TexunaApiBaseAddress"]) }).SingleInstance().AsSelf();
            builder.RegisterType<HttpClientWrapper>().SingleInstance().AsSelf();

            builder.RegisterType<GovernorDownloadApiService>().As<IGovernorDownloadService>();
            builder.RegisterType<GovernorsReadApiService>().As<IGovernorsReadService>();
            builder.RegisterType<EstablishmentWriteApiService>().As<IEstablishmentWriteService>();
            builder.RegisterType<FileDownloadFactoryApiService>().As<IFileDownloadFactoryService>();
            builder.RegisterType<GovernorsWriteApiService>().As<IGovernorsWriteService>();
            builder.RegisterType<SecurityApiService>().As<ISecurityService>();
            builder.RegisterType<GroupsWriteApiService>().As<IGroupsWriteService>();
            builder.RegisterType<DownloadsApiService>().As<IDownloadsService>();
#else


            builder.RegisterType<AzureSearchEndPoint>().WithParameter("connectionString", 
                ConfigurationManager.ConnectionStrings["Microsoft.Azure.Search.ConnectionString"].ConnectionString)
                .As<IAzureSearchEndPoint>();
            
            builder.RegisterType<ApplicationDbContextFactory<ApplicationDbContext>>()
                .As<IApplicationDbContextFactory>();

            builder.RegisterType<ApplicationDbContextFactory<InMemoryApplicationDbContext>>()
                .As<IInMemoryApplicationDbContextFactory>();

            builder.RegisterType<ApplicationDbContext>().As<IApplicationDbContext>();
            
            builder.RegisterType<LAReadRepository>().As<ILAReadRepository>();
            builder.RegisterType<CachedLAReadRepository>().As<ICachedLAReadRepository>();
            
            builder.RegisterType<EstablishmentReadRepository>().As<IEstablishmentReadRepository>();
            builder.RegisterType<CachedEstablishmentReadRepository>().As<ICachedEstablishmentReadRepository>();

            builder.RegisterType<GroupReadRepository>().As<IGroupReadRepository>();
            builder.RegisterType<CachedGroupReadRepository>().As<ICachedGroupReadRepository>();

            builder.RegisterType<EstablishmentGroupReadRepository>().As<IEstablishmentGroupReadRepository>();
            builder.RegisterType<CachedEstablishmentGroupReadRepository>().As<ICachedEstablishmentGroupReadRepository>();

            builder.RegisterType<GroupsWriteService>().As<IGroupsWriteService>();
            
            builder.RegisterType<EstablishmentDownloadService>().As<IEstablishmentDownloadService>();

            builder.RegisterType<EstablishmentReadService>().As<IEstablishmentReadService>();
            builder.RegisterType<GroupReadService>().As<IGroupReadService>();
            builder.RegisterType<LookupService>().As<ILookupService>();
            

            builder.RegisterType<EstablishmentWriteService>().As<IEstablishmentWriteService>();
            builder.RegisterType<GovernorsWriteService>().As<IGovernorsWriteService>();

            builder.RegisterType<LAESTABService>().As<ILAESTABService>();
            builder.RegisterType<SecurityService>().As<ISecurityService>();
            builder.RegisterType<GroupDownloadService>().As<IGroupDownloadService>();

            builder.RegisterType<GovernorDownloadService>().As<IGovernorDownloadService>();
            builder.RegisterType<GovernorsReadService>().As<IGovernorsReadService>();
            
            builder.RegisterType<FileDownloadFactoryService>().As<IFileDownloadFactoryService>();
            builder.RegisterType<DownloadsService>().As<IDownloadsService>();
#endif

            builder.RegisterType<ResourcesHelper>().As<IResourcesHelper>();
            builder.RegisterType<CompaniesHouseService>().As<ICompaniesHouseService>();
        }
    }
}