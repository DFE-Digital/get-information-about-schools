using System;
using System.Threading.Tasks;
using Edubase.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

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

            var exception = context.Exception;

            if (exception is TexunaApiNotFoundException)
            {
                context.Result = new ObjectResult("Entity was not found")
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
                context.ExceptionHandled = true;
                return;
            }

            var httpContext = context.HttpContext;
            var exceptionHandler = httpContext.RequestServices.GetRequiredService<ExceptionHandler>();
            //var logMessage = exceptionHandler..Log(httpContext, exception);

            var error = new SystemErrorMessage
            {
                //ErrorCode = logMessage.Id,
                TechnicalDetails = exception.ToString()
            };

            context.Result = new ObjectResult(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
