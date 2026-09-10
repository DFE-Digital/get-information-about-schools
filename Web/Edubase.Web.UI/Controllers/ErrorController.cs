using System;
using System.Web;
using Azure.Core;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Exceptions;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Edubase.Web.UI.Controllers
{
    public class ErrorController : Controller
    {
        private ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("Error/{statusCode?}")]
        public IActionResult Index(int? statusCode)
        {
            if (statusCode.HasValue)
            {
                return statusCode.Value switch
                {
                    404 => View("NotFound"),
                    403 => View("AccessDenied"),
                    _ => View("GenericError")
                };
            }

            //500 Errors caught by UseExceptionHandler
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature != null)
            {
                var ex = exceptionFeature.Error;
                // var path = exceptionFeature.Path;

                if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["EnableErrorReporting"]))
                {
                    var ctx = HttpContext;

                    if (ex is UnsuccessfulSamlOperationException)
                    {
                        Response.Redirect("/Unauthorized/LoginFailed");
                    }

                    if (ctx != null && ex != null)
                    {
                        var id = Guid.NewGuid();
                        var clientIpAddress = ctx.Connection.RemoteIpAddress;
                        var httpMethod = ctx.Request.Method;
                        var requestJsonBody = "";
                        var url = ctx.Request.Path;
                        var userAgent = ctx.Request.GetUserAgent();
                        var dateUtc = DateTime.UtcNow;
                        var exception = ex;
                        var message = ex.GetBaseException().Message;
                        var referrerUrl = ctx.Request.Path;
                        var userId = ctx?.User?.Identity?.GetUserId();
                        var userName = ctx?.User?.Identity?.GetUserName();

                        logger.LogCritical("Unhandled Exception Id: {id}, ClientIpAddress: {clientIpAddress}, HttpMethod: {httpMethod}, RequestJsonBody:{requestJsonBody}, URL: {url}, UserAgent: {userAgent}, DateUtc: {dateUtc}, Exception: {exception},  Message: {message}, ReferrerUrl: {referrerUrl}, UserId: {userId}, UserName: {userName}",
                            id,
                            ctx.Connection.RemoteIpAddress,
                            ctx.Request.Method,
                            requestJsonBody,
                            url,
                            userAgent,
                            dateUtc,
                            exception,                            
                            message,
                            referrerUrl,
                            userId,
                            userName
                            );
                        ViewBag.EdubaseErrorCode = id;

                        return View("ServerError");
                    }
                }
            }

            return View("ServerError");
        }
    }
}
