using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Utils;
using Edubase.Web.UI.Identity;

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
            builder.RegisterType<SchoolPermissions>().As<ISchoolPermissions>();
            builder.RegisterType<UserIdentity>().As<IUserIdentity>();
            builder.RegisterType<RedirectAfterLoginHelper>().As<IRedirectAfterLoginHelper>();
            builder.RegisterType<RequestContextWrapper>().As<IRequestContext>();
        }
    }
}