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

            builder.RegisterType<GroupsWriteService>().As<IGroupsWriteService>();
            builder.RegisterType<CachedLookupService>().As<ICachedLookupService>();
            builder.RegisterType<EstablishmentDownloadService>().As<IEstablishmentDownloadService>();
            builder.RegisterType<EstablishmentReadService>().As<IEstablishmentReadService>();
            builder.RegisterType<EstablishmentWriteService>().As<IEstablishmentWriteService>();
            builder.RegisterType<GroupReadService>().As<IGroupReadService>();
            builder.RegisterType<LAESTABService>().As<ILAESTABService>();
            builder.RegisterType<LookupService>().As<ILookupService>();
        }
        
    }
}