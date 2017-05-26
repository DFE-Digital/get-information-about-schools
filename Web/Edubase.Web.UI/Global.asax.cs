using Autofac;
using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Services;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Validation;
using FluentValidation.Mvc;
using StackExchange.Profiling;
using System;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Edubase.Web.UI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            GlobalConfiguration.Configure(x => 
            {
                x.MapHttpAttributeRoutes();
                IocConfig.Register(x);
                x.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                x.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            });

            using (var scope = IocConfig.Container.BeginLifetimeScope())
            {
                scope.Resolve<ICacheAccessor>().InitialiseIfNecessaryAsync().Wait();
                scope.Resolve<IBlobService>().Initialise("downloads");
            }

            var fluentValidationModelValidatorProvider = new FluentValidationModelValidatorProvider(new AutofacValidatorFactory(IocConfig.Container));
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            fluentValidationModelValidatorProvider.AddImplicitRequiredValidator = false;
            ModelValidatorProviders.Providers.Add(fluentValidationModelValidatorProvider);

            FlushLogMessages();

            ModelBinders.Binders.DefaultBinder = new Helpers.ModelBinding.DefaultModelBinderEx();

            MiniProfiler.Settings.Results_Authorize = IsUserAllowedToSeeMiniProfilerUI;
            MiniProfiler.Settings.Results_List_Authorize = IsUserAllowedToSeeMiniProfilerUI;

        }

        private void FlushLogMessages(CacheEntryRemovedArguments arguments = null)
        {
            using (var scope = IocConfig.Container.BeginLifetimeScope())
            {
                var task = scope.Resolve<IMessageLoggingService>().FlushAsync();
            }

            var interval = RandomNumber.Next(10, 30); // random so that in a webfarm, where nodes start simultaneously, flushing is staggered across a 30 second time window.
#if (DEBUG)
            interval = 5;
#endif
            MemoryCache.Default.Set(new CacheItem(nameof(FlushLogMessages), 0), new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTime.UtcNow.AddSeconds(interval), 
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

        protected void Application_BeginRequest()
        {
            MiniProfiler.Start();
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

        private bool IsUserAllowedToSeeMiniProfilerUI(HttpRequest httpRequest) => true; // TODO: TEXCHANGE: SECURE THIS

        //public static bool IsRunningOnAzure => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
    }
}
