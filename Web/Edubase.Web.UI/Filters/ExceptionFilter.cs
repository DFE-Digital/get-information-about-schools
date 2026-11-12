using System;
using System.Configuration;
using System.Linq;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Common;
using Edubase.Services.Exceptions;
using Edubase.Services.Texuna;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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

            if (filterContext.Exception is EdubaseException domainException)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/DomainError.cshtml",
                    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                        metadataProvider: new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                        modelState: filterContext.ModelState)
                    {
                        ["PublicErrorMessage"] = domainException.Message,
                        ["ExTypeName"] = domainException.GetType().Name
                    }
                };

                filterContext.ExceptionHandled = true;
            }
            else if (string.Equals(filterContext.Exception.GetType().Name, "UnsuccessfulSamlOperationException", StringComparison.Ordinal))
            {
                filterContext.Result = new RedirectResult("/Unauthorized/LoginFailed");
                filterContext.ExceptionHandled = true;
            }
            else
            {
                var ctx = filterContext.HttpContext;
                var msg = Log(ctx, filterContext.Exception);

                var request = ctx.Request;
                var isPartialView = request.Path.Value != null &&
                                    request.Path.Value.EndsWith("results-js", StringComparison.InvariantCultureIgnoreCase);

                filterContext.Result = new ViewResult
                {
                    ViewName = EnableFriendlyErrorPage ? "~/Views/Shared/Error.cshtml" : "~/Views/Shared/FullErrorDetail.cshtml",
                    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                        metadataProvider: new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                        modelState: filterContext.ModelState)
                    {
                        ["ErrorCode"] = msg.Id,
                        ["IsPartialView"] = isPartialView,
                        ["Message"] = msg.Message,
                        ["Url"] = request.Path + request.QueryString,
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
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                filterContext.ExceptionHandled = true;
            }
        }

        public WebLogMessage Log(HttpContext ctx, Exception exception)
        {
            var httpMethod = ctx?.Request?.Method ?? string.Empty;

            var msg = new WebLogMessage
            {
                ClientIpAddress = ctx?.Connection?.RemoteIpAddress?.ToString(),
                Environment = ConfigurationManager.AppSettings["Environment"],
                Exception = exception?.ToString(),
                HttpMethod = httpMethod,
                Level = LogMessage.LogLevel.ERROR,
                ReferrerUrl = ctx?.Request?.Headers["Referer"].ToString(),
                Message = exception?.GetBaseException().Message,
                Url = (ctx?.Request?.Path.ToString() ?? string.Empty) + (ctx?.Request?.QueryString.Value ?? string.Empty),
                UserAgent = ctx?.Request?.Headers["User-Agent"].ToString(),
                UserId = ctx?.User?.GetUserId(),
                UserName = ctx?.User?.Identity?.Name,
                RequestJsonBody = (exception as TexunaApiSystemException)?.ApiRequestJsonPayload ?? string.Empty
            };

            if (!new[] { string.Empty, "POST", "GET" }.Any(x => httpMethod.Equals(x, StringComparison.OrdinalIgnoreCase)))
            {
                return msg;
            }

            if ((msg.UserAgent ?? string.Empty).IndexOf("bot", StringComparison.OrdinalIgnoreCase) == -1)
            {
                _logger.Log(msg);
            }

            return msg;
        }
    }
}
