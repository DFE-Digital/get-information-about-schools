using Edubase.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace Edubase.Web.UI.Filters
{
    public class ApiExceptionFilter: ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Exception is TexunaApiNotFoundException)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity was not found"),
                    ReasonPhrase = "Not Found"
                };
                actionExecutedContext.Response = msg;
            }
            else await base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is TexunaApiNotFoundException)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity was not found"),
                    ReasonPhrase = "Not Found"
                };
                actionExecutedContext.Response = msg;
            }
            else base.OnException(actionExecutedContext);
        }
    }
}