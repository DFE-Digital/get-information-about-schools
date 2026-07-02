using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Edubase.Common;
using Edubase.Services.Exceptions;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Web.UI.Helpers;
using Sustainsys.Saml2.Exceptions;

namespace Edubase.Web.UI.Filters
{
    public class ExceptionHandler : IExceptionFilter
    {
        private readonly IAzLogger _logger;

        public ExceptionHandler(IAzLogger logger) => _logger = logger;

        public static bool EnableFriendlyErrorPage => StringUtil.Boolify(ConfigurationManager.AppSettings["EnableFriendlyErrorPage"], true);

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            var ctx = filterContext.HttpContext;
            var exception = filterContext.Exception;

            if (!ShouldLogException(exception, ctx))
            {
                filterContext.ExceptionHandled = true;
                return;
            }

            if (filterContext.Exception is EdubaseException) // domain / purposeful exception - not logged
            {
                var domainException = filterContext.Exception as EdubaseException;
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/DomainError.cshtml",
                    ViewData = new ViewDataDictionary() { { "PublicErrorMessage", domainException.Message }, { "ExTypeName", domainException.GetType().Name } }
                };

                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is UnsuccessfulSamlOperationException)
            {
                filterContext.Result = new RedirectResult("/Unauthorized/LoginFailed");
                filterContext.ExceptionHandled = true;
            }
            else // unhandled/unexpected exception; log it and tell the user.
            {
                var msg = Log(ctx, exception);

                // Making this "forwarded-header-aware" is not strictly required,
                // but it's easier and safer to be consistent and just do it everywhere.
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                var url = urlHelper.GetForwardedHeaderAwareUrl();

                filterContext.Result = new ViewResult
                {
                    // Show either the "simple" user-facing error page, or the full error page with technical detail.
                    ViewName = EnableFriendlyErrorPage ? "~/Views/Shared/Error.cshtml" : "~/Views/Shared/FullErrorDetail.cshtml",
                    ViewData = new ViewDataDictionary
                    {
                        ["ErrorCode"] = msg.Id,
                        ["IsPartialView"] = url.AbsolutePath.EndsWith("results-js", StringComparison.InvariantCultureIgnoreCase),
                        ["Message"] = msg.Message,
                        ["Url"] = url,
                        ["Environment"] = ConfigurationManager.AppSettings["Environment"],
                        ["ShowTechnicalDetail"] = !EnableFriendlyErrorPage,
                        ["Exception"] = msg.Exception,
                        ["UserId"] = msg.UserId,
                        ["UserName"] = msg.UserName,
                        ["UserAgent"] = msg.UserAgent,
                        ["ClientIpAddress"] = msg.ClientIpAddress,
                        ["ReferrerUrl"] = msg.ReferrerUrl,
                        ["HttpMethod"] = msg.HttpMethod,
                        ["RequestJsonBody"] = msg.RequestJsonBody
                    }
                };

                ctx.Response.Clear();
                ctx.Response.TrySkipIisCustomErrors = true;
                ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                filterContext.ExceptionHandled = true;
            }
        }

        public WebLogMessage Log(HttpContextBase ctx, Exception exception)
        {
            string userId = null, userName = null;
            string httpMethod = ctx?.Request?.HttpMethod ?? string.Empty;

            try
            {
                userId = ctx?.User?.Identity?.GetUserId();
                userName = ctx?.User?.Identity?.GetUserName();
            }
            catch
            {
                // ignored
            }

            var logLevel = GetLogLevel(exception);

            var msg = new WebLogMessage {
                ClientIpAddress = ctx?.Request?.UserHostAddress,
                Environment = ConfigurationManager.AppSettings["Environment"],
                Exception = exception?.ToString(),
                HttpMethod = httpMethod,
                Level = logLevel,
                ReferrerUrl = ctx?.Request.UrlReferrer?.ToString(),
                Message = exception?.GetBaseException().Message,
                Url = ctx?.Request.Url?.ToString(),
                UserAgent = ctx?.Request.UserAgent,
                UserId = userId,
                UserName = userName,
                RequestJsonBody = (exception as TexunaApiSystemException)?.ApiRequestJsonPayload ?? string.Empty
            };

            if (!new[] {string.Empty, "POST", "GET"}.Any(x => httpMethod.Equals(x, StringComparison.OrdinalIgnoreCase))) return msg; // only log errors GET/POST or empty http method
            if ((msg.UserAgent ?? string.Empty).IndexOf("bot", StringComparison.OrdinalIgnoreCase) == -1)
            {
                _logger.Log(msg);
            }

            return msg;
        }

        private LogMessage.LogLevel GetLogLevel(Exception exception)
        {
            LogMessage.LogLevel loglevel;

            switch (exception)
            {
                case TaskCanceledException _:
                    loglevel = LogMessage.LogLevel.INFORMATION;
                    break;
                case ArgumentException _:
                    loglevel = LogMessage.LogLevel.WARNING;
                    break;
                case InvalidOperationException _:
                    loglevel = LogMessage.LogLevel.WARNING;
                    break;
                case HttpAntiForgeryException _:
                    loglevel = LogMessage.LogLevel.ERROR;
                    break;
                default:
                    loglevel = LogMessage.LogLevel.ERROR;
                    break;
            }

            return loglevel;
        }

        private bool ShouldLogException(Exception ex, HttpContextBase context)
        {
            if (ex is HttpAntiForgeryException)
            {
                // dont log HttpAntiForgeryException if a session timeout
                return context?.Session != null && context.Session.IsNewSession;
            }

            if (ex is TaskCanceledException)
            {
                return false;
            }

            return true;
        }
    }
}
