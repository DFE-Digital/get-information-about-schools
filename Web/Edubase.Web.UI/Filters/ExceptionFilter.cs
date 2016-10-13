using Edubase.Data.Entity;
using Edubase.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Edubase.Web.UI.Filters
{
    public class ExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));
            var msg = Log(filterContext.HttpContext, filterContext.Exception);

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary() { { "ErrorCode", msg.Id } }
            };

            filterContext.ExceptionHandled = true;
        }

        public LogMessage Log(HttpContextBase ctx, Exception exception)
        {
            string userId = null, userName = null;

            try
            {
                userId = ctx?.User?.Identity?.GetUserId();
                userName = ctx?.User?.Identity?.GetUserName();
            }
            catch { }

            var msg = new LogMessage
            {
                ClientIPAddress = ctx?.Request?.UserHostAddress,
                Environment = ConfigurationManager.AppSettings["Environment"],
                Exception = exception?.ToString(),
                HttpMethod = ctx?.Request?.HttpMethod,
                Level = LogMessage.eLevel.Error,
                ReferrerUrl = ctx?.Request?.UrlReferrer?.ToString(),
                Text = exception?.GetBaseException()?.Message,
                Url = ctx?.Request?.Url.ToString(),
                UserAgent = ctx?.Request?.UserAgent,
                UserId = userId,
                UserName = userName
            };

            MessageLoggingService.Instance.Push(msg);
            return msg;
        }
    }
}