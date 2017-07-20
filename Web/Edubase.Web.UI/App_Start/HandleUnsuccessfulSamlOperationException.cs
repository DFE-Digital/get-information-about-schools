using System;
using System.Web.Mvc;
using Kentor.AuthServices.Exceptions;

namespace Edubase.Web.UI.App_Start
{
    public class HandleUnsuccessfulSamlOperationException : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is UnsuccessfulSamlOperationException)
            {
                filterContext.Result = new RedirectResult("/Unauthorized");
                filterContext.ExceptionHandled = true;
            }
        }
    }
}