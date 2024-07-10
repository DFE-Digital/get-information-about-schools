using System;
using Autofac;
using Edubase.Common.Cache;
using Edubase.Web.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;
using AzureTableLogger.Services;
using Edubase.Data.Repositories;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Admin"), Route("{action=Logs}"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
    public class AdminController : EduBaseController
    {
        private readonly ErrorWebLogItemRepository _errorWebLogItemRepository;

        private readonly ILoggingService _loggingService;

        public AdminController(ILoggingService loggingService, ErrorWebLogItemRepository errorWebLogItemRepository)
        {
            _loggingService = loggingService;
            _errorWebLogItemRepository = errorWebLogItemRepository;
        }

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
            return Content("Redis cache and MemoryCache cleared successfully.", "text/plain");
        }

        public async Task FlushErrors() => await _loggingService.FlushAsync();


        public async Task<ActionResult> ViewErrorLogs()
        {
            var referenceDate = DateTime.Today;
            var days = 28;
            var includePurgeZeroLogsMessage = false;

            var webLogMessages = await _errorWebLogItemRepository.Get(referenceDate, days: days, includePurgeZeroLogsMessage: includePurgeZeroLogsMessage);

            ViewBag.Messages = webLogMessages;

            ViewBag.ReferenceDate = referenceDate;
            ViewBag.Days = days;
            ViewBag.IncludePurgeZeroLogsMessage = includePurgeZeroLogsMessage;

            return View();
        }
    }
}
