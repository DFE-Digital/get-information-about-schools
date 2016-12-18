using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Data.Entity;
using Edubase.Data.Migrations;
using Edubase.Services;
using Edubase.Web.UI.Filters;
using FluentValidation.Mvc;
using System;
using System.Data.Entity;
using System.Runtime.Caching;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Edubase.Web.UI.Helpers;
using Autofac;
using Edubase.Data.DbContext;

namespace Edubase.Web.UI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
#if (QA)
            GlobalConfiguration.Configure(ODataConfig.Register);
#endif
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            IocConfig.Register();

            using (var scope = IocConfig.Container.BeginLifetimeScope())
                scope.Resolve<ICacheAccessor>().InitialiseIfNecessaryAsync().Wait();
            
            // REMOVE WHEN IN WEBFARM!!!!!
            var m = new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>();
            Database.SetInitializer(m);

            FluentValidationModelValidatorProvider.Configure();
            FlushLogMessages();
            ModelBinders.Binders.DefaultBinder = new Helpers.ModelBinding.DefaultModelBinderEx();
        }

        private void FlushLogMessages(CacheEntryRemovedArguments arguments = null)
        {
            using (var scope = IocConfig.Container.BeginLifetimeScope())
            {
                var task = scope.Resolve<IMessageLoggingService>().FlushAsync();
            }

            var interval = RandomNumber.Next(10, 30);
#if (DEBUG)
            interval = 5;
#endif
            MemoryCache.Default.Set(new CacheItem(nameof(FlushLogMessages), 0), new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTime.UtcNow.AddSeconds(interval), // random so that in a webfarm, where nodes start simultaneously, flushing is staggered across a 30 second time window.
                RemovedCallback = FlushLogMessages
            });
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["EnableErrorReporting"]))
            {
                var ctx = HttpContext.Current;
                var ex = ctx?.Server?.GetLastError();
                if (ctx != null && ex != null) new ExceptionHandler().Log(new HttpContextWrapper(ctx), ex);
            }
        }

        public static bool IsRunningOnAzure => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
    }
}
