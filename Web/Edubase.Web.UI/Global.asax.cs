using Autofac;
using Edubase.Common.Cache;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Validation;
using FluentValidation.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Xml;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.ExternalLookup;
using Newtonsoft.Json.Serialization;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ValueProviders;
using Microsoft.Ajax.Utilities;
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
                const string pathToConfigFile = "../../devsecrets.gias.config.alwaysignore";
                GetExternalSettings(pathToConfigFile);
            }
            catch
            {
                throw new UnauthorizedAccessException("Could not get external settings. This is probably because you haven't been issued the external settings file.");
            }
#endif

            // Self-signed certificates fail validation for HTTPS connections - optionally override this for
            // local development if a SHA1 hash has been provided within configuration.
            AddTrustedCertificatesFromConfiguration();


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

            var logger = IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<IAzLogger>();
            logger.ScheduleLogFlush();
            logger.ScheduleLogPurge();

            IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<IExternalLookupService>();

            ModelBinders.Binders.DefaultBinder = new DefaultModelBinderEx();
            ValueProviderFactories.Factories.Add(new TokenValueProviderFactory());

            MvcHandler.DisableMvcResponseHeader = true;
        }


        private static void GetExternalSettings(string pathToConfigFile)
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, pathToConfigFile);
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException();
            }

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
                            if (!node.Name.Equals("add"))
                            {
                                continue;
                            }

                            if (node.Attributes == null)
                            {
                                continue;
                            }

                            ConnectionStringSettings connection = ConfigurationManager.ConnectionStrings[node.Attributes?["name"].Value];
                            configurationReadOnlyField?.SetValue(connection, false);
                            connection.ConnectionString = node.Attributes?["connectionString"].Value;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// <p>
        ///     This is used to (optionally) add a known-good self-signed certificate to the allow list.
        /// </p><p>
        ///     An allow-list specified within local configuration is preferable to alternatives
        ///     such as disabling SSL verification entirely, or adding the certificate to the
        ///     trusted root store.
        /// </p><p>
        ///     Strongly discouraged for any environment other than local development.
        ///     (currently enforced, where <c>Environment</c> must be <c>localdev</c>)
        /// </p><p>
        ///     <list type="bullet">
        ///         <item>The SHA1 thumbprint of the certificate to add to the allow list.</item>
        ///         <item>Non-alphanumeric characters are removed (<c>[^A-Za-z0-9_]</c>) and case is normalised.</item>
        ///         <item>This can be obtained from the web browser.</item>
        ///     </list>
        /// </p><p>
        ///     Adapted from: https://stackoverflow.com/a/44140506
        /// </p>
        /// </summary>
        private static void AddTrustedCertificatesFromConfiguration()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate(
                object sender,
                X509Certificate cert,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                {
                    // If no SSL verification issues under normal circumstances, no need to check further.
                    return true;
                }

                var environment = ConfigurationManager.AppSettings["Environment"];
                if (!environment.Equals("localdev"))
                {
                    // If not running locally, don't allow any override of SSL verification and reject the connection.
                    return false;
                }

                var knownGoodSslCertificateThumbprintSha1 = ConfigurationManager.AppSettings["ApiCertificateSha1"];
                if (knownGoodSslCertificateThumbprintSha1.IsNullOrWhiteSpace())
                {
                    // If no known-good certificate is specified, reject the connection.
                    return false;
                }

                // SHA1 thumbprint is normally hexadecimal, but software might include additional characters
                // when presenting the value for display (e.g. spaces, colons, dashes) so remove them
                // and normalise to a consistent case (not strictly required if string comparison ignores case).
                var normalisedKnownGoodThumbprintSha1 = Regex
                    .Replace(knownGoodSslCertificateThumbprintSha1, @"[^A-Za-z0-9]+", "")
                    .ToUpperInvariant();
                if (knownGoodSslCertificateThumbprintSha1.IsNullOrWhiteSpace())
                {
                    // If the certificate's thumbprint is empty after normalisation (e.g., maybe it contained only special characters which got removed),
                    // therefore throw an exception because this is a developer/configuration error.
                    throw new ArgumentException(
                        "Invalid configuration - The API SSL certificate thumbprint has been specified, but normalises to an empty value."
                    );
                }

                var actualCertHashStringSha1 = cert.GetCertHashString();
                if (string.Equals(
                        actualCertHashStringSha1,
                        normalisedKnownGoodThumbprintSha1,
                        StringComparison.OrdinalIgnoreCase
                    ))
                {
                    // Certificate would normally be rejected, but it matches the given known-good certificate therefore allow it.
                    return true;
                }

                // Default to rejecting the connection if the certificate is not in the allow list.
                return false;
            };
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
