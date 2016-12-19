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

            var jsonConverterCollection = new JsonConverterCollection();
            jsonConverterCollection.Add(new DbGeographyConverter());
            builder.RegisterInstance(jsonConverterCollection);

            builder.RegisterType<CacheAccessor>().As<ICacheAccessor>()
                .UsingConstructor(typeof(JsonConverterCollection)).SingleInstance().AsSelf();

            builder.RegisterType<AzureSearchEndPoint>().WithParameter("connectionString", 
                ConfigurationManager.ConnectionStrings["Microsoft.Azure.Search.ConnectionString"].ConnectionString)
                .As<IAzureSearchEndPoint>();

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
            builder.RegisterType<EstablishmentReadService>().As<IEstablishmentReadService>();
            builder.RegisterType<GroupReadService>().As<IGroupReadService>();
            builder.RegisterType<LAESTABService>().As<ILAESTABService>();
        }
        
    }
}