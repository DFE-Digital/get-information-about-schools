﻿using System;
using System.Collections.Generic;
using Autofac;
using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Services;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.Admin;
using System.Threading.Tasks;
using System.Web.Mvc;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using AzureTableLogger.Services;
using Edubase.Services.Domain;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Admin"), Route("{action=Logs}"), MvcAuthorizeRoles(EdubaseRoles.ROLE_BACKOFFICE)]
    public class AdminController : EduBaseController
    {
        private readonly ILoggingService _loggingService;

        public AdminController(ILoggingService loggingService) => _loggingService = loggingService;

        [Route("Logs")]
        public async Task<ActionResult> Logs(string date, string skipToken)
        {
            var result = await _loggingService.GetAllAsync(10, UriHelper.TryDeserializeUrlToken<TableContinuationToken>(skipToken), date.ToDateTime("yyyyMMdd"));
            LogMessagesDto dto = new LogMessagesDto(result.Item1.Cast<WebLogMessage>(), UriHelper.SerializeToUrlToken(result.Item2));
            return View(new LogMessagesViewModel(dto) {DateFilter = date});
        }

        [Route("LogDetail/{id}")]
        public async Task<ActionResult> LogDetail(string id)
        {
            var message = (WebLogMessage) await _loggingService.GetAsync(id);
            if (message == null) return Content("Log message not found.  Please note it may take up to 30 seconds for a log message to become available at this URL.", "text/plain");
            return View(message);
        }
        
        [HttpGet, Route("GetPendingErrors")]
        public ActionResult GetPendingErrors(string pwd) {
            return pwd == "c7634" ? (ActionResult) Json(_loggingService.GetPending()) : new EmptyResult();
        }

        [HttpGet, Route("GetPendingErrorsId")]
        public ActionResult GetPendingErrorsId()
        {
            return Json(_loggingService.InstanceId);
        }
        
        [Route("ClearCache")]
        public async Task<ActionResult> ClearCache()
        {
            await IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<ICacheAccessor>().ClearAsync();
            return Content("Redis cache and MemoryCache cleared successfully.", "text/plain");
        }
        
        public async Task FlushErrors() => await _loggingService.FlushAsync();
    }
}