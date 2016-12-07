using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services;
using AutoMapper;
using Edubase.Services.Mapping;
using Edubase.Data.Entity;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using System.Configuration;

namespace Edubase.Web.UI
{
    public static class IocConfig
    {
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
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<AzureSearchEndPoint>().WithParameter("connectionString", 
                ConfigurationManager.ConnectionStrings["Microsoft.Azure.Search.ConnectionString"].ConnectionString).As<IAzureSearchEndPoint>();

            builder.RegisterType<ApplicationDbContext>().As<IApplicationDbContext>();
            builder.RegisterInstance(AutoMapperWebConfiguration.CreateMapper()).As<IMapper>();
            builder.RegisterType<CachedLookupService>().As<ICachedLookupService>();
            builder.RegisterType<EstablishmentReadService>().As<IEstablishmentReadService>();
            builder.RegisterType<GroupReadService>().As<IGroupReadService>();
            builder.RegisterType<LAESTABService>().As<ILAESTABService>();
        }
    }
}