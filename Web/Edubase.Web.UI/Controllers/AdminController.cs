using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Edubase.Common.Cache;
using Edubase.Web.UI.Helpers;
using System.Threading.Tasks;
using AzureTableLogger.Services;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Admin"), Route("{action=Logs}"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
    public class AdminController : EduBaseController
    {
        private readonly WebLogItemRepository _webLogItemRepository;

        private readonly ILoggingService _loggingService;

        public AdminController(ILoggingService loggingService, WebLogItemRepository webLogItemRepository)
        {
            _loggingService = loggingService;
            _webLogItemRepository = webLogItemRepository;
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

        public async Task<ActionResult> ViewLogs(LogsViewModel model)
        {
            var queryString = model.Query ?? "";
            var includePurgeZeroLogsMessage = model.IncludePurgeZeroLogsMessage;

            var webLogMessages = new List<AZTLoggerMessages>();

            // First check if our query matches the ID of a specific log message
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                var searchByIdResults = await _webLogItemRepository.GetById(model.Query.ToLowerInvariant());
                if (searchByIdResults.Count == 1)
                {
                    webLogMessages = searchByIdResults;
                }
            }

            // Only if the previous check(s) didn't return any exact matches, do the broader search
            if (!webLogMessages.Any())
            {
                var startDate = model.StartDate.ToDateTime();
                var endDate = model.EndDate.ToDateTime();
                if (model.StartDate.IsValid() && model.EndDate.IsValid() && startDate.HasValue && endDate.HasValue)
                {
                    // Note: Add one day to the end date to include the end date in the search results
                    webLogMessages = await _webLogItemRepository.GetWithinDateRange(startDate.Value, endDate.Value.AddDays(1));
                    webLogMessages = WebLogItemRepository.FilterByAllTextColumns(webLogMessages, queryString);
                    webLogMessages = WebLogItemRepository.FilterPurgeZeroLogsMessage(webLogMessages, includePurgeZeroLogsMessage);
                }
            }

            var viewModel = new LogsViewModel
            {
                Messages = webLogMessages,
                Query = queryString,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IncludePurgeZeroLogsMessage = includePurgeZeroLogsMessage
            };

            return View(viewModel);
        }

        private bool DateIsValid(int year, int month, int day)
        {
            return DateTime.TryParse($"{year}-{month}-{day}", out _);
        }

    }
}
