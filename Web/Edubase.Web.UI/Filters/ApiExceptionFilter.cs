using Edubase.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Edubase.Web.UI.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;
        private readonly bool _enableFriendlyErrorPage;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IConfiguration configuration)
        {
            _logger = logger;
            _enableFriendlyErrorPage = bool.TryParse(configuration["EnableFriendlyErrorPage"], out var enabled)
                ? enabled
                : true;
        }

        public class SystemErrorMessage
        {
            public string Message { get; set; } = "Sorry, there is a problem with the service.";
            public string ErrorCode { get; set; }
            public string TechnicalDetails { get; set; }
        }

        public void OnException(ExceptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Exception is TexunaApiNotFoundException)
            {
                context.Result = new NotFoundObjectResult("Entity was not found");
                context.ExceptionHandled = true;
                return;
            }

            _logger.LogError(context.Exception, "Unhandled API exception in {Path}", context.HttpContext.Request.Path);
            var error = new SystemErrorMessage
            {
                ErrorCode = context.HttpContext.TraceIdentifier,
                TechnicalDetails = _enableFriendlyErrorPage ? null : context.Exception.ToString()
            };

            context.Result = new ObjectResult(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
