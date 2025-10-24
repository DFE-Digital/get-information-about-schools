//using System;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace Edubase.Web.UI
//{
//    public class UnauthorizedAccessAttribute : FilterAttribute, IExceptionFilter
//    {
//        public void OnException(ExceptionContext filterContext)
//        {
//            if (filterContext.Exception is UnauthorizedAccessException)
//            {
//                filterContext.Result = new RedirectResult("/Unauthorized");
//                filterContext.ExceptionHandled = true;
//            }
//        }
//    }
//}
