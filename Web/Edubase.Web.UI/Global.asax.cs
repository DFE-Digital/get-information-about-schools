using Autofac;
using Edubase.Common.Cache;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Validation;
using FluentValidation.Mvc;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Xml;
using AzureTableLogger;
using Newtonsoft.Json.Serialization;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ValueProviders;
using Sustainsys.Saml2.Exceptions;

namespace Edubase.Web.UI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
#if DEBUG
            try
            {
                GetExternalSettings();
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException("Could not get external settings. This is probably because you haven't been issued the external settings file.");
            }
#endif

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


        private static void GetExternalSettings()
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "../../devsecrets.gias.config.alwaysignore");
            if (!File.Exists(configPath)) throw new FileNotFoundException();
            XmlDocument doc = new XmlDocument();
            using (XmlReader reader = XmlReader.Create(configPath, GetSecureXmlReaderSettings()))
            {
                doc.Load(reader);
            }

            foreach (XmlNode child in doc.ChildNodes)
            {
                if (!child.Name.Equals("configuration")) continue;
                foreach (XmlNode childNode in child.ChildNodes)
                {
                    if (childNode.Name.Equals("appSettings"))
                    {
                        foreach (XmlNode node in childNode.ChildNodes)
                        {
                            if (node.Name.Equals("add"))
                            {
                                ConfigurationManager.AppSettings[node.Attributes?["key"].Value] = node.Attributes?["value"].Value;
                            }
                        }
                    }
                    else if (childNode.Name.Equals("connectionStrings"))
                    {
                        FieldInfo configurationReadOnlyField = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                        foreach (XmlNode node in childNode.ChildNodes)
                        {
                            if (!node.Name.Equals("add")) continue;
                            if (node.Attributes == null) continue;
                            ConnectionStringSettings connection = ConfigurationManager.ConnectionStrings[node.Attributes?["name"].Value];
                            configurationReadOnlyField?.SetValue(connection, false);
                            connection.ConnectionString = node.Attributes?["connectionString"].Value;
                        }
                    }
                }

            }
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

        private static XmlReaderSettings GetSecureXmlReaderSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            };
            return settings;
        }
    }
}
