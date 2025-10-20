using System;
using System.Linq;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Common;
using Edubase.Services.Exceptions;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Sustainsys.Saml2.Exceptions;

namespace Edubase.Web.UI.Filters
{
    public class ExceptionHandler : IExceptionFilter
    {
        private readonly IAzLogger _logger;
        private readonly IConfiguration _configuration;

        public ExceptionHandler(IAzLogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public bool EnableFriendlyErrorPage =>
            StringUtil.Boolify(_configuration["EnableFriendlyErrorPage"], true);

        public void OnException(ExceptionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var exception = context.Exception;

            if (exception is EdubaseException domainException)
            {
                context.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/DomainError.cshtml",
                    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(), context.ModelState)
                    {
                        { "PublicErrorMessage", domainException.Message },
                        { "ExTypeName", domainException.GetType().Name }
                    }
                };
                context.ExceptionHandled = true;
                return;
            }

            if (exception is UnsuccessfulSamlOperationException)
            {
                context.Result = new RedirectResult("/Unauthorized/LoginFailed");
                context.ExceptionHandled = true;
                return;
            }

            var httpContext = context.HttpContext;
            var msg = Log(httpContext, exception);

            var url = httpContext.Request.GetForwardedHeaderAwareUrl();

            context.Result = new ViewResult
            {
                ViewName = EnableFriendlyErrorPage ? "~/Views/Shared/Error.cshtml" : "~/Views/Shared/FullErrorDetail.cshtml",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(), context.ModelState)
                {
                    ["ErrorCode"] = msg.Id,
                    ["IsPartialView"] = url.AbsolutePath.EndsWith("results-js", StringComparison.InvariantCultureIgnoreCase),
                    ["Message"] = msg.Message,
                    ["Url"] = url,
                    ["Environment"] = _configuration["Environment"],
                    ["ShowTechnicalDetail"] = !EnableFriendlyErrorPage,
                    ["Exception"] = msg.Exception,
                    ["UserId"] = msg.UserId,
                    ["UserName"] = msg.UserName,
                    ["UserAgent"] = msg.Headers["User-Agent"].ToString(),
                    ["ClientIpAddress"] = msg.ClientIpAddress,
                    ["ReferrerUrl"] = msg.ReferrerUrl,
                    ["HttpMethod"] = msg.HttpMethod,
                    ["RequestJsonBody"] = msg.RequestJsonBody
                }
            };

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.ExceptionHandled = true;
        }

        public WebLogMessage Log(HttpContext ctx, Exception exception)
        {
            string userId = null, userName = null;
            string httpMethod = ctx?.Request?.Method ?? string.Empty;

            try
            {
                userId = ctx?.User?.Identity?.GetUserId();
                userName = ctx?.User?.Identity?.GetUserName();
            }
            catch
            {
                // ignored
            }

            var msg = new WebLogMessage
            {
                ClientIpAddress = ctx?.Connection?.RemoteIpAddress?.ToString(),
                Environment = _configuration["Environment"],
                Exception = exception?.ToString(),
                HttpMethod = httpMethod,
                Level = LogMessage.LogLevel.ERROR,
                ReferrerUrl = ctx?.Request?.Headers["Referer"].ToString(),
                Message = exception?.GetBaseException().Message,
                Url = $"{ctx?.Request?.Scheme}://{ctx?.Request?.Host}{ctx?.Request?.Path}{ctx?.Request?.QueryString}",
                UserAgent = ctx?.Request?.Headers["User-Agent"].ToString(),
                UserId = userId,
                UserName = userName,
                RequestJsonBody = (exception as TexunaApiSystemException)?.ApiRequestJsonPayload ?? string.Empty
            };

            if (!new[] { "", "POST", "GET" }.Any(x => httpMethod.Equals(x, StringComparison.OrdinalIgnoreCase))) return msg;

            if ((msg.Headers["User-Agent"].ToString() ?? "").IndexOf("bot", StringComparison.OrdinalIgnoreCase) == -1)
            {
                _logger.Log(msg);
            }

            return msg;
        }
    }
}
