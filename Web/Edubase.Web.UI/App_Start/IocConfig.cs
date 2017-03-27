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
using Edubase.Services.IntegrationEndPoints.ServiceBus;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.Core;
using Edubase.Services.Nomenclature;

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
#if QA
            builder.RegisterType<MockSmtpEndPoint>().As<ISmtpEndPoint>();
#else
            builder.RegisterType<SmtpEndPoint>().As<ISmtpEndPoint>();
#endif

            builder.RegisterType<MessageLoggingService>()
                .As<IMessageLoggingService>()
                .As<IExceptionLogger>()
                .SingleInstance();

            builder.RegisterInstance(new JsonConverterCollection() { new DbGeographyConverter() });

            builder.RegisterType<CacheAccessor>()
                .SingleInstance().As<ICacheAccessor>()
                .UsingConstructor(typeof(JsonConverterCollection));

            builder.RegisterType<AzureSearchEndPoint>().WithParameter("connectionString", 
                ConfigurationManager.ConnectionStrings["Microsoft.Azure.Search.ConnectionString"].ConnectionString)
                .As<IAzureSearchEndPoint>();

            builder.RegisterInstance(Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString));

            builder.RegisterType<BlobService>().As<IBlobService>();

            builder.RegisterType<ApplicationDbContextFactory<ApplicationDbContext>>()
                .As<IApplicationDbContextFactory>();

            builder.RegisterType<ApplicationDbContextFactory<InMemoryApplicationDbContext>>()
                .As<IInMemoryApplicationDbContextFactory>();

            builder.RegisterType<ApplicationDbContext>().As<IApplicationDbContext>();
            builder.RegisterInstance(AutoMapperWebConfiguration.CreateMapper()).As<IMapper>();

            builder.RegisterType<LAReadRepository>().As<ILAReadRepository>();
            builder.RegisterType<CachedLAReadRepository>().As<ICachedLAReadRepository>();
            
            builder.RegisterType<EstablishmentReadRepository>().As<IEstablishmentReadRepository>();
            builder.RegisterType<CachedEstablishmentReadRepository>().As<ICachedEstablishmentReadRepository>();

            builder.RegisterType<GroupReadRepository>().As<IGroupReadRepository>();
            builder.RegisterType<CachedGroupReadRepository>().As<ICachedGroupReadRepository>();

            builder.RegisterType<EstablishmentGroupReadRepository>().As<IEstablishmentGroupReadRepository>();
            builder.RegisterType<CachedEstablishmentGroupReadRepository>().As<ICachedEstablishmentGroupReadRepository>();

            builder.RegisterType<GroupsWriteService>().As<IGroupsWriteService>();
            builder.RegisterType<CachedLookupService>().As<ICachedLookupService>();
            builder.RegisterType<EstablishmentDownloadService>().As<IEstablishmentDownloadService>();

#if QA
            builder.RegisterType<EstablishmentReadApiService>().As<IEstablishmentReadService>();
            builder.RegisterType<GroupReadApiService>().As<IGroupReadService>();
            builder.RegisterType<LookupApiService>().As<ILookupService>();
#else
            builder.RegisterType<EstablishmentReadService>().As<IEstablishmentReadService>();
            builder.RegisterType<GroupReadService>().As<IGroupReadService>();
            builder.RegisterType<LookupService>().As<ILookupService>();
#endif

            builder.RegisterType<EstablishmentWriteService>().As<IEstablishmentWriteService>();
            builder.RegisterType<GovernorsWriteService>().As<IGovernorsWriteService>();

            builder.RegisterType<LAESTABService>().As<ILAESTABService>();
            builder.RegisterType<SecurityService>().As<ISecurityService>();
            builder.RegisterType<GroupDownloadService>().As<IGroupDownloadService>();

            builder.RegisterType<GovernorDownloadService>().As<IGovernorDownloadService>();
            builder.RegisterType<GovernorsReadService>().As<IGovernorsReadService>();
            builder.RegisterType<CompaniesHouseService>().As<ICompaniesHouseService>();

            builder.RegisterType<ServiceBusEndPoint>().As<IServiceBusEndPoint>();
            builder.RegisterType<FileDownloadFactoryService>().As<IFileDownloadFactoryService>();

            builder.RegisterInstance(new NomenclatureService()).AsSelf();

            builder.RegisterInstance(new HttpClient {BaseAddress = new Uri(ConfigurationManager.AppSettings["TexunaApiBaseAddress"])}).SingleInstance().AsSelf();
            builder.RegisterType<HttpClientWrapper>().SingleInstance().AsSelf();

        }

    }
}