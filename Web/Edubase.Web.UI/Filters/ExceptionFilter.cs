﻿using Edubase.Data.Entity;
using Edubase.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Edubase.Common;
using Edubase.Services.Exceptions;
using Autofac.Core;
using Kentor.AuthServices.Exceptions;

namespace Edubase.Web.UI.Filters
{
    public class ExceptionHandler : IExceptionFilter
    {
        public static bool EnableFriendlyErrorPage => StringUtil.Boolify(ConfigurationManager.AppSettings["EnableFriendlyErrorPage"], true);

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));

            if (filterContext.Exception is EdubaseException) // domain / purposeful exception - not logged
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/DomainError.cshtml",
                    ViewData = new ViewDataDictionary() { { "PublicErrorMessage", (filterContext.Exception as EdubaseException).Message } }
                };

                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is UnsuccessfulSamlOperationException)
            {
                filterContext.Result = new RedirectResult("/Unauthorized/LoginFailed");
                filterContext.ExceptionHandled = true;
            }
            else // unhandled/unexpected exception; log it and tell the user.
            {
                var ctx = filterContext.HttpContext;
                var msg = Log(ctx, filterContext.Exception);

                if (EnableFriendlyErrorPage)
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        ViewData = new ViewDataDictionary{ ["ErrorCode"] = msg.Id, ["IsPartialView"] = ctx.Request.Url.AbsolutePath.EndsWith("results-js") }
                    };
                }
                else // show full technical error detail
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/FullErrorDetail.cshtml",
                        ViewData = new ViewDataDictionary(filterContext.Exception) { ["ErrorCode"] = msg.Id, ["IsPartialView"] = ctx.Request.Url.AbsolutePath.EndsWith("results-js") }
                    };
                }

                ctx.Response.Clear();
                ctx.Response.TrySkipIisCustomErrors = true;
                ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                filterContext.ExceptionHandled = true;
            }
        }

        public LogMessage Log(HttpContextBase ctx, Exception exception)
        {
            string userId = null, userName = null;
            string httpMethod = ctx?.Request?.HttpMethod ?? string.Empty;

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
                HttpMethod = httpMethod,
                Level = LogMessage.eLevel.Error,
                ReferrerUrl = ctx?.Request?.UrlReferrer?.ToString(),
                Text = exception?.GetBaseException()?.Message,
                Url = ctx?.Request?.Url.ToString(),
                UserAgent = ctx?.Request?.UserAgent,
                UserId = userId,
                UserName = userName,
                RequestJsonBody = (exception as TexunaApiSystemException)?.ApiRequestJsonPayload ?? string.Empty
            };

            if (new[] { string.Empty, "POST", "GET" }.Any(x => httpMethod.Equals(x, StringComparison.OrdinalIgnoreCase))) // only log errors GET/POST or empty http method 
            {
                DependencyResolver.Current.GetService<IMessageLoggingService>().Push(msg);
            }
            
            return msg;
        }
    }
}