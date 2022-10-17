using System;
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
    [RoutePrefix("Admin"), Route("{action=Logs}"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
    public class AdminController : EduBaseController
    {
        private readonly ILoggingService _loggingService;

        public AdminController(ILoggingService loggingService) => _loggingService = loggingService;
        
        [HttpGet, Route("GetPendingErrors")]
        public ActionResult GetPendingErrors(string pwd)
        {
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
            return Content("Caches cleared successfully.", "text/plain");
        }
        
        public async Task FlushErrors() => await _loggingService.FlushAsync();
    }
}
