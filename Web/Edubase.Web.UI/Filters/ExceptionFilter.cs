using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Edubase.Common;
using Edubase.Services.Exceptions;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
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
                var ctx = filterContext.HttpContext;
                var msg = Log(ctx, filterContext.Exception);

                var urlHelper = new UrlHelper(filterContext.RequestContext);

                if (EnableFriendlyErrorPage)
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        // TODO: Consider if this use of `Request.Url` requires editing to account for Azure Front Door proxy URL
                        ViewData = new ViewDataDictionary{ ["ErrorCode"] = msg.Id, ["IsPartialView"] = ctx.Request.Url.AbsolutePath.EndsWith("results-js") }
                    };
                }
                else // show full technical error detail
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/FullErrorDetail.cshtml",
                        // TODO: Consider if this use of `Request.Url` requires editing to account for Azure Front Door proxy URL
                        ViewData = new ViewDataDictionary(filterContext.Exception) { ["ErrorCode"] = msg.Id, ["IsPartialView"] = ctx.Request.Url.AbsolutePath.EndsWith("results-js") }
                    };
                }

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

            WebLogMessage msg = new WebLogMessage {
                // TODO: Consider if we should also log the x-forwarded-for IP address
                ClientIpAddress = ctx?.Request?.UserHostAddress,
                Environment = ConfigurationManager.AppSettings["Environment"],
                Exception = exception?.ToString(),
                HttpMethod = httpMethod,
                Level = LogMessage.LogLevel.ERROR,
                // TODO: Consider if this use of `Request.UrlReferrer` requires editing to account for Azure Front Door proxy URL
                ReferrerUrl = ctx?.Request.UrlReferrer?.ToString(),
                Message = exception?.GetBaseException().Message,
                // TODO: Consider if this use of `Request.Url` requires editing to account for Azure Front Door proxy URL
                // TODO: Consider if we should log both the request URL and the x-forwarded-host URL
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
    }
}
