using Autofac;
using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Services;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Validation;
using FluentValidation.Mvc;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using AzureTableLogger;
using Newtonsoft.Json.Serialization;
using Kentor.AuthServices.Exceptions;
using Edubase.Web.UI.Areas.Establishments.Models.Search;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ValueProviders;

namespace Edubase.Web.UI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            
            GlobalConfiguration.Configure(x => 
            {
                x.MapHttpAttributeRoutes();
                IocConfig.Register(x);
                x.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                x.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
                x.Filters.Add(new ApiExceptionFilter());
            });

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<ExceptionHandler>());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<ICacheAccessor>().InitialiseIfNecessaryAsync().Wait();
            
            var fluentValidationModelValidatorProvider = new FluentValidationModelValidatorProvider(new AutofacValidatorFactory(IocConfig.AutofacDependencyResolver));
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            fluentValidationModelValidatorProvider.AddImplicitRequiredValidator = false;
            ModelValidatorProviders.Providers.Add(fluentValidationModelValidatorProvider);
            
#if (DEBUG)
            IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<IAzLogger>().ScheduleLogFlush(5);
#else
            IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<IAzLogger>().ScheduleLogFlush(10, 40);
#endif

            ModelBinders.Binders.DefaultBinder = new DefaultModelBinderEx();
            ValueProviderFactories.Factories.Add(new TokenValueProviderFactory());

            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["EnableErrorReporting"]))
            {
                var ctx = HttpContext.Current;
                var ex = ctx?.Server?.GetLastError();
                if (ex is UnsuccessfulSamlOperationException)
                {
                    Response.Redirect("/Unauthorized/LoginFailed");
                }

                if (ctx != null && ex != null)
                {
                    var msg = IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<ExceptionHandler>().Log(new HttpContextWrapper(ctx), ex);
                    ctx.Items["edubase_error_code"] = msg.Id;
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string[] headers = { "Server", "X-AspNet-Version" };

            if (!Response.HeadersWritten)
            {
                Response.AddOnSendingHeaders((c) =>
                {
                    if (c != null && c.Response != null && c.Response.Headers != null)
                    {
                        foreach (string header in headers)
                        {
                            if (c.Response.Headers[header] != null)
                            {
                                c.Response.Headers.Remove(header);
                            }
                        }
                    }
                });
            }

        }


    }
}
