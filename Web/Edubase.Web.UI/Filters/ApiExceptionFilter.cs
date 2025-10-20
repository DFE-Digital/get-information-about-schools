using System;
using System.Threading.Tasks;
using Autofac;
using AzureTableLogger.LogMessages;
using Edubase.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Edubase.Web.UI.Filters
{
    public class ApiExceptionFilter : IAsyncExceptionFilter
    {
        public class SystemErrorMessage
        {
            public string Message { get; set; } = "Sorry, there is a problem with the service.";
            public string ErrorCode { get; set; }
            public string TechnicalDetails { get; set; }
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Exception is TexunaApiNotFoundException)
            {
                context.Result = new NotFoundObjectResult("Entity was not found");
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var httpContext = context.HttpContext;
            var exceptionHandler = IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<ExceptionHandler>();
            WebLogMessage msg = exceptionHandler.Log(httpContext, context.Exception);

            var error = new SystemErrorMessage
            {
                ErrorCode = msg.Id,
                TechnicalDetails = ExceptionHandler.EnableFriendlyErrorPage ? null : context.Exception.ToString()
            };

            context.Result = new ObjectResult(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            await Task.CompletedTask;
        }
    }
}
