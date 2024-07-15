using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Edubase.Common.Cache;
using Edubase.Web.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;
using AzureTableLogger.Services;
using Edubase.Data.Entity;
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
            var queryString = "";
            var includePurgeZeroLogsMessage = false;

            var referenceDate = DateTime.Today;
            var days = 28;
            var startDate = referenceDate.AddDays(-1 * days);
            var endDate = referenceDate.AddDays(1);

            var webLogMessages = await _errorWebLogItemRepository.GetWithinDateRange(
                startDate,
                endDate
            );

            // do client-side filtering on results (easier than doing it server-side)
            // data size and access frequency is small enough that this is acceptable
            webLogMessages = FilterByAllTextColumns(webLogMessages, queryString);
            webLogMessages = FilterPurgeZeroLogsMessage(webLogMessages, false);

            ViewBag.Messages = webLogMessages;

            ViewBag.Query = queryString;
            ViewBag.ReferenceDate = referenceDate;
            ViewBag.Days = days;
            ViewBag.IncludePurgeZeroLogsMessage = includePurgeZeroLogsMessage;

            return View();
        }

        private List<AZTLoggerMessages> FilterPurgeZeroLogsMessage(List<AZTLoggerMessages> webLogMessages, bool includePurgeZeroLogsMessage)
        {
            if (includePurgeZeroLogsMessage)
            {
                return webLogMessages;
            }

            webLogMessages = webLogMessages
                .Where(m => !m.Message.Equals("LOG PURGE REPORT: There were 0 logs purged from storage."))
                .ToList();

            return webLogMessages;
        }

        private static List<AZTLoggerMessages> FilterByAllTextColumns(List<AZTLoggerMessages> webLogMessages,
            string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return webLogMessages;
            }

            webLogMessages = webLogMessages.Where(m =>
                {
                    if (!string.IsNullOrWhiteSpace(m.Level) && m.Level.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Environment) && m.Environment.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Message) && m.Message.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Exception) && m.Exception.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Url) && m.Url.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserAgent) && m.UserAgent.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.ClientIpAddress) && m.ClientIpAddress.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.ReferrerUrl) && m.ReferrerUrl.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.HttpMethod) && m.HttpMethod.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.RequestJsonBody) && m.RequestJsonBody.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserId) && m.UserId.Contains(queryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserName) && m.UserName.Contains(queryString))
                    {
                        return true;
                    }


                    return false;
                }
            ).ToList();

            return webLogMessages;
        }
    }
}
