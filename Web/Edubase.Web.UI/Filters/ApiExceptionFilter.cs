using Edubase.Data.Entity;
using Edubase.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using AzureTableLogger.LogMessages;

namespace Edubase.Web.UI.Filters
{
    public class ApiExceptionFilter: ExceptionFilterAttribute
    {
        public class SystemErrorMessage
        {
            public string Message { get; set; } = "Sorry, there is a problem with the service.";
            public string ErrorCode { get; set; }
            public string TechnicalDetails { get; set; }
        }

        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            OnException(actionExecutedContext);
            return Task.CompletedTask;
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is TexunaApiNotFoundException) actionExecutedContext.Response = CreateNotFoundMessage();
            else actionExecutedContext.Response = GetResponseMessage(actionExecutedContext);
        }

        private static HttpResponseMessage CreateNotFoundMessage()
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Entity was not found"),
                ReasonPhrase = "Not Found"
            };
        }

        private HttpResponseMessage GetResponseMessage(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null) throw new ArgumentNullException(nameof(actionExecutedContext));
            WebLogMessage msg = IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<ExceptionHandler>().Log(actionExecutedContext.Request.Properties["MS_HttpContext"] as HttpContextWrapper, actionExecutedContext.Exception);
            var error = new SystemErrorMessage
            {
                ErrorCode = msg.Id,
                TechnicalDetails = ExceptionHandler.EnableFriendlyErrorPage ? null : actionExecutedContext.Exception.ToString()
            };
            return actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, error);
        } 

        
    }
}