using System;
using System.Linq;
using System.Net;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Common;
using Edubase.Services.Exceptions;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Sustainsys.Saml2.Exceptions;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace Edubase.Web.UI.Filters
{
    public class ExceptionHandler : IExceptionFilter
    {
        private readonly IAzLogger _logger;
        private readonly IConfiguration _config;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public ExceptionHandler(
            IAzLogger logger,
            IConfiguration config,
            IUrlHelperFactory urlHelperFactory,
            IModelMetadataProvider modelMetadataProvider)
        {
            _logger = logger;
            _config = config;
            _urlHelperFactory = urlHelperFactory;
            _modelMetadataProvider = modelMetadataProvider;
        }

        private bool EnableFriendlyErrorPage =>
            StringUtil.Boolify(_config["EnableFriendlyErrorPage"], true);

        public void OnException(ExceptionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var httpContext = context.HttpContext;

            if (context.Exception is EdubaseException domainException)
            {
                context.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/DomainError.cshtml",
                    ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
                {
                    { "PublicErrorMessage", domainException.Message },
                    { "ExTypeName", domainException.GetType().Name }
                }
                };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is UnsuccessfulSamlOperationException)
            {
                context.Result = new RedirectResult("/Unauthorized/LoginFailed");
                context.ExceptionHandled = true;
            }
            else
            {
                var msg = Log(httpContext, context.Exception);

                var urlHelper = _urlHelperFactory.GetUrlHelper(context);
                var url = urlHelper.GetForwardedHeaderAwareUrl(_config);

                context.Result = new ViewResult
                {
                    ViewName = EnableFriendlyErrorPage
                        ? "~/Views/Shared/Error.cshtml"
                        : "~/Views/Shared/FullErrorDetail.cshtml",
                    ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
                    {
                        ["ErrorCode"] = msg.Id,
                        ["IsPartialView"] = url.AbsolutePath.EndsWith("results-js", StringComparison.InvariantCultureIgnoreCase),
                        ["Message"] = msg.Message,
                        ["Url"] = url,
                        ["Environment"] = _config["Environment"],
                        ["ShowTechnicalDetail"] = !EnableFriendlyErrorPage,
                        ["Exception"] = msg.Exception,
                        ["UserId"] = msg.UserId,
                        ["UserName"] = msg.UserName,
                        ["ClientIpAddress"] = msg.ClientIpAddress,
                        ["ReferrerUrl"] = msg.ReferrerUrl,
                        ["HttpMethod"] = msg.HttpMethod,
                        ["RequestJsonBody"] = msg.RequestJsonBody
                    }
                };

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.ExceptionHandled = true;
            }
        }

        private WebLogMessage Log(HttpContext ctx, Exception exception)
        {
            string userId = null, userName = null;
            string httpMethod = ctx?.Request?.Method ?? string.Empty;

            try
            {
                userId = ctx?.User?.GetUserId();
                userName = ctx?.User?.Identity?.GetUserName();
            }
            catch
            {
                // ignored
            }

            var msg = new WebLogMessage
            {
                ClientIpAddress = ctx?.Connection?.RemoteIpAddress?.ToString(),
                Environment = _config["Environment"],
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

            if (!new[] { "", "POST", "GET" }.Any(x => httpMethod.Equals(x, StringComparison.OrdinalIgnoreCase)))
                return msg;

            if ((msg.UserAgent ?? "").IndexOf("bot", StringComparison.OrdinalIgnoreCase) == -1)
            {
                _logger.Log(msg);
            }

            return msg;
        }
    }
}
